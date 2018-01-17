using Appccelerate.StateMachine;
using Newtonsoft.Json;
using NLog;
using Solis.Gossip.Model;
using Solis.Gossip.Model.Events;
using Solis.Gossip.Model.Messages;
using Solis.Gossip.Model.Settings;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Solis.Gossip.Service
{
    public class GossipManager
    {
        public static Logger Logger = SolisLogFactory.GetLogger(typeof(GossipManager));
        private GossipNode _gossipNode;
        private CancellationTokenSource _cts;
        private ConcurrentDictionary<string, BaseMessage> _requests;

        private const int ResponseWaitTimeout = 60 * 1000;

        private AsyncPassiveStateMachine<NodeState, GossipEvent> _fsm;
        private event EventHandler SendHelloEvent;
        private event EventHandler SendHeartbeatEvent;

        private CurrentStateExtension _currentStateExtension;

        private Timer _helloTimer;
        private Timer _heartbeatTimer;

        public GossipManager(GossipNode gossipNode)
        {
            _gossipNode = gossipNode;
            _cts = new CancellationTokenSource();
            _currentStateExtension = new CurrentStateExtension();

            _requests = new ConcurrentDictionary<string, BaseMessage>();
            _helloTimer = new Timer(new TimerCallback(HelloTimerHandle));
            _heartbeatTimer = new Timer(new TimerCallback(HeartbeatTimerHandle));

            _fsm = new AsyncPassiveStateMachine<NodeState, GossipEvent>("GossipManager");
            _fsm.AddExtension(new ConsoleLogExtension());
            _fsm.AddExtension(_currentStateExtension);

            _fsm.Initialize(NodeState.Initialized);

            _fsm.In(NodeState.Initialized)
                .On(GossipEvent.HelloSend)
                    .Goto(NodeState.HelloSent)
                .Execute(async () => 
                {
                    // set hello timer
                    await SendHello();
                });

            _fsm.In(NodeState.HelloSent)
                .On(GossipEvent.HelloAnswer)
                    .Goto(NodeState.Infected)
                .Execute<HelloResponse>((msg) => 
                {
                    ReceiveHelloAnswer(msg);
                })
                .On(GossipEvent.HelloExpired)
                    .Goto(NodeState.Initialized)
                .Execute(async () => 
                    { 
                        // hello timer re-set
                        await SendHello();
                    });

            _fsm.In(NodeState.Infected)
                .On(GossipEvent.HeartbeatExpired)
                    .Goto(NodeState.Susceptible)
                .Execute(async () =>
                {
                    await SendHeartbeat();
                })
                .On(GossipEvent.HeartbeatAnswer)
                    .Goto(NodeState.Infected)
                .Execute<HeartbeatResponse>((msg) => {
                    GossipPeer senderPeer = msg.Members?.FirstOrDefault();
                    MergeLists(senderPeer, msg.Members);
                });

            _fsm.In(NodeState.Infected)
                .On(GossipEvent.HelloReceive)
                .Execute<HelloRequest>(async (msg) =>
                {
                    await SendHelloAnswer(msg);
                });

            _fsm.In(NodeState.Susceptible)
                .On(GossipEvent.HelloReceive)
                .Execute<HelloRequest>(async (msg) =>
                {
                    await SendHelloAnswer(msg);
                })
                .On(GossipEvent.HeartbeatReceive)
                    .If<DateTime>((dt) =>
                    {
                        return CheckHeartbeat(dt);
                    })
                    .Goto(NodeState.Infected)
                    .Execute<HeartbeatRequest>(async (msg) =>
                    {
                        await ReceiveHeartbeat(msg);
                    })
                .On(GossipEvent.HeartbeatAnswer)
                    .If<HeartbeatResponse>((msg) =>
                    {
                        return msg.Members[0].Heartbeat > _gossipNode.GossipPeer.Heartbeat;
                    }).Goto(NodeState.Infected);

            _fsm.Start();
        }

        private async Task ReceiveHeartbeat(HeartbeatRequest msg)
        {
            var needReply = MergeLists(msg.Peer, msg.Members);

            if (needReply)
            {
                HeartbeatResponse o = new HeartbeatResponse();
                o.Members.Add(_gossipNode.GossipPeer);
                o.Members.AddRange(_gossipNode.RemotePeers);
                o.RemoteEndPoint = _gossipNode.GossipPeer.EndPoint;
                o.Id = Guid.NewGuid().ToString();
                o.RequestMessageId = msg.Id;

                await SendAsync(o, msg.RemoteEndPoint);
            }
        }

        private async void HeartbeatTimerHandle(object state)
        {
            await _fsm.Fire(GossipEvent.HeartbeatElapsed);
        }

        private void ReceiveHelloAnswer(HelloResponse msg)
        {
            var needReply = MergeLists(msg.Members[0], msg.Members);
            // do nothing
        }

        private async Task SendHello()
        {
            await SendAsync(new HelloRequest(_gossipNode.GossipPeer), new IPEndPoint(IPAddress.Broadcast, _gossipNode.GossipPeer.EndPoint.Port));
            _helloTimer.Change(0, ResponseWaitTimeout);
        }

        private async void HelloTimerHandle(object state)
        {
            _helloTimer.Change(0, Timeout.Infinite);
            await _fsm.Fire(GossipEvent.HelloExpired);
        }

        private async Task SendHelloAnswer(HelloRequest msg)
        {
            _gossipNode.AddPeer(msg.Peer);

            HelloResponse o = new HelloResponse();
            o.Members.Add(_gossipNode.GossipPeer);
            o.Members.AddRange(_gossipNode.RemotePeers);
            o.RemoteEndPoint = _gossipNode.GossipPeer.EndPoint;
            o.Id = Guid.NewGuid().ToString();
            o.RequestMessageId = msg.Id;

            await SendAsync(o, msg.RemoteEndPoint);
        }

        private async Task SendHeartbeat()
        {
            var remotePeer = SelectNeighbor(_gossipNode.RemotePeers);
            if (remotePeer == null)
            {
                Logger.Debug("SendHeartbeat is called without action");
                return;
            }
            else
            {
                Logger.Debug($"SendHeartbeat is called to {remotePeer.ToString()}");
            }

            try
            {
                HeartbeatRequest message = new HeartbeatRequest(remotePeer);
                message.Id = Guid.NewGuid().ToString();
                message.RemoteEndPoint = _gossipNode.GossipPeer.EndPoint;
                message.Members.Add(_gossipNode.GossipPeer);
                message.Members.AddRange(_gossipNode.RemotePeers);

                await SendAsync(message, remotePeer.EndPoint);
            }
            catch (Exception e)
            {
                Logger.Warn(e);
            }
        }

        protected GossipPeer SelectNeighbor(List<GossipPeer> memberList)
        {
            GossipPeer peer = null;
            if (memberList.Count > 0)
            {
                int tryCount = 0;
                Random random = new Random();

                do
                {
                    int randomNeighborIndex = random.Next(0, memberList.Count - 1);
                    peer = memberList[randomNeighborIndex];

                    if (++tryCount == 5)
                    {
                        peer = null;
                        break;
                    }
                } while (peer != null && peer.Id == _gossipNode.GossipPeer.Id);
            }
            else
            {
                Logger.Debug("I am alone.");
            }

            return peer;
        }

        private bool CheckHeartbeat(DateTime remoteHeartbeat)
        {
            return _gossipNode.GossipPeer.Heartbeat > remoteHeartbeat;
        }

        public void Shutdown()
        {
            try
            {
                _fsm.Stop();
                _cts.Cancel();
            }
            catch (Exception e)
            {
                Logger.Warn(e);
            }
        }

        /// <summary>
        /// Receive message from members
        /// </summary>
        /// <param name="message"></param>
        /// <param name="remoteEndPoint"></param>
        /// <returns></returns>
        public async Task Recieve(BaseMessage message, IPEndPoint remoteEndPoint)
        {
            if (message is Request)
            {
                if (message is HelloRequest)
                {
                    await _fsm.Fire(GossipEvent.HelloReceive, message as HelloRequest);
                }
                else if (message is HeartbeatRequest)
                {
                    await _fsm.Fire(GossipEvent.HeartbeatReceive, message as HeartbeatRequest);
                }
            }
            else if (message is Response)
            {
                if (message is HelloResponse)
                {
                    await _fsm.Fire(GossipEvent.HelloAnswer, message as HelloResponse);
                }
                else if (message is HeartbeatResponse)
                {
                    await _fsm.Fire(GossipEvent.HeartbeatAnswer, message as HeartbeatResponse);
                }
                else if (message is ErrorResponse)
                {
                    Console.WriteLine(((ErrorResponse)message).ErrorMessage);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="uri"></param>
        /// <returns></returns>
        public async Task SendAsync(BaseMessage message, IPEndPoint endPoint)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            };

            byte[] json_bytes = message.Serialize();

            int packet_length = json_bytes.Length;
            if (packet_length < GossipNode.MAX_PACKET_SIZE)
            {
                try
                {
                    using (UdpClient socket = new UdpClient())
                    {
                        await socket.SendAsync(json_bytes, packet_length, endPoint);
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e, "SendAsync");
                    throw;
                }
            }
        }

        /// <summary>
        /// Merge remote list (received from peer), and our local member list. Simply, we must update the
        /// heartbeats that the remote list has with our list.Also, some additional logic is needed to
        /// make sure we have not timed out a member and then immediately received a list with that member.
        /// </summary>
        /// <param name="senderPeer"></param>
        /// <param name="remoteList"></param>
        protected bool MergeLists(GossipPeer senderPeer, List<GossipPeer> remoteList)
        {
            var needReply = false;
            foreach (GossipPeer remotePeer in remoteList)
            {
                if (remotePeer.Id == _gossipNode.GossipPeer.Id)
                {
                    continue;
                }

                var peer = _gossipNode.RemotePeers.FirstOrDefault(p => p.Id == remotePeer.Id);
                if (peer != null)
                {
                    if (remotePeer.Heartbeat > peer.Heartbeat)
                    {
                        peer.Heartbeat = remotePeer.Heartbeat;

                        if(peer.State == GossipPeerState.Online 
                                && remotePeer.State == GossipPeerState.Offline)
                        {
                            _gossipNode.DownPeer(remotePeer);
                        }
                        else if (peer.State == GossipPeerState.Offline
                                && remotePeer.State == GossipPeerState.Online)
                        {
                            _gossipNode.WakeUpPeer(remotePeer);
                        }
                    }
                    else
                    {
                        needReply = true;
                    }
                }
                else
                {
                    _gossipNode.AddPeer(remotePeer);
                }
            }

            return needReply;
        }
    }
}
