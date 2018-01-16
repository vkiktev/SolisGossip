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
    /// <summary>
    /// 
    /// </summary>
    public class GossipManager
    {
        public static Logger Logger = SolisLogFactory.GetLogger(typeof(GossipManager));
        private GossipNode _gossipNode;
<<<<<<< HEAD
        private CancellationTokenSource _cts;
        private ConcurrentDictionary<string, BaseMessage> _requests;

        private AsyncPassiveStateMachine<NodeState, GossipEvent> _fsm;
        private event EventHandler SendGreetingsEvent;
        private event EventHandler SendHeartbeatEvent;

        private NodeState _state;
=======
        private TaskFactory service;
        private CancellationTokenSource cts;
        private ConcurrentDictionary<string, BaseMessage> _requests;
>>>>>>> ef6c834203f8f9d0c44ff7a0910d7b76ab35e8da

        public GossipManager(GossipNode gossipNode)
        {
            _gossipNode = gossipNode;
<<<<<<< HEAD
            _cts = new CancellationTokenSource();

            _requests = new ConcurrentDictionary<string, BaseMessage>();

            _fsm = new AsyncPassiveStateMachine<NodeState, GossipEvent>("GossipManager");
            _fsm.AddExtension(new ConsoleLogExtension());

            _fsm.Initialize(NodeState.Initialized);

            _state = NodeState.Initialized;

            _fsm.In(NodeState.Initialized)
                .On(GossipEvent.SendGreetings)
                    .Goto(NodeState.SentGreetings)
                .Execute(() => 
                {
                    SendGreetings();
                    _state = NodeState.SentGreetings;
                });

            _fsm.In(NodeState.SentGreetings)
                .On(GossipEvent.ReceiveWelcome)
                    .Goto(NodeState.Infected)
                .Execute(() => 
                {
                    ReceiveWelcome();
                    _state = NodeState.Infected;
                })
                .On(GossipEvent.GreetingsExpired)
                    .Goto(NodeState.Initialized)
                .Execute(() => 
                    { 
                        SendGreetings();
                        _state = NodeState.Initialized;
                    });

            _fsm.In(NodeState.Infected)
                .On(GossipEvent.HeartbeatTimerExpired)
                    .Goto(NodeState.Susceptible)
                .Execute(() => 
                {
                    SendHeartbeat();
                    _state = NodeState.Susceptible;
                });

            _fsm.In(NodeState.Infected)
                .On(GossipEvent.ReceiveGreetings)
                .Execute(() =>
                {
                    SendHeartbeat();
                    _state = NodeState.Infected;
                });

            _fsm.In(NodeState.Susceptible)
                .On(GossipEvent.ReceiveGreetings)
                .Execute(() =>
                {
                    SendHeartbeat();
                    _state = NodeState.Susceptible;
                })
                .On(GossipEvent.ReceiveHeartbeat)
                    .If<DateTime>((dt) =>
                    {
                        return CheckHeartbeat(dt);
                    })
                    .Goto(NodeState.Infected)
                    .Execute(() => {
                        _state = NodeState.Infected;
                    });

            _fsm.Start();
        }

        public void Fire(GossipEvent @event)
        {
            _fsm.Fire(@event);
        }

        public void Fire(GossipEvent @event, object eventArgument)
        {
            _fsm.Fire(@event, eventArgument);
        }

        private void ReceiveWelcome()
        {
            //log here
        }

        private void SendGreetings()
        {
            SendGreetingsEvent?.Invoke(this, EventArgs.Empty);
        }

        private void SendHeartbeat()
        {
            SendHeartbeatEvent?.Invoke(this, EventArgs.Empty);
        }

        private bool CheckHeartbeat(DateTime remoteHeartbeat)
        {
            return _gossipNode.GossipPeer.Heartbeat > remoteHeartbeat;
=======
            service = new TaskFactory();
            cts = new CancellationTokenSource();

            _requests = new ConcurrentDictionary<string, BaseMessage>();
>>>>>>> ef6c834203f8f9d0c44ff7a0910d7b76ab35e8da
        }

        public void AddRequest(string peerId, BaseMessage message)
        {
            _requests.TryAdd(peerId, message);
        }

        public void RemoveRequest(string peerId, BaseMessage message)
        {
            _requests.TryAdd(peerId, message);
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
                if (message is HeartbeatRequest)
                {
<<<<<<< HEAD
                    bool needReply = true;

=======
>>>>>>> ef6c834203f8f9d0c44ff7a0910d7b76ab35e8da
                    var request = ((HeartbeatRequest)message);
                    if (request.IsGreetings)
                    {
                        _gossipNode.AddPeer(request.Peer);
                        await _fsm.Fire(GossipEvent.ReceiveGreetings);
                    }
                    else
                    {
                        needReply = MergeLists(request.Peer, request.Members);
                        await _fsm.Fire(GossipEvent.ReceiveHeartbeat);
                    }

<<<<<<< HEAD
                    if (needReply)
                    {
                        HeartbeatResponse o = new HeartbeatResponse();
                        o.Members.Add(_gossipNode.GossipPeer);
                        o.Members.AddRange(_gossipNode.RemotePeers);
                        o.UriFrom = message.UriFrom;
                        o.Id = message.Id;
                        o.RequestMessageId = message.Id;

                        await SendAsync(o, remoteEndPoint);
=======
                        if (needReply)
                        {
                            HeartbeatResponse o = new HeartbeatResponse();
                            o.Members.Add(_gossipNode.GossipPeer);
                            o.Members.AddRange(_gossipNode.RemotePeers);
                            o.UriFrom = message.UriFrom;
                            o.Id = message.Id;
                            o.RequestMessageId = message.Id;

                            await SendAsync(o, remoteEndPoint);
                        }
>>>>>>> ef6c834203f8f9d0c44ff7a0910d7b76ab35e8da
                    }


                }
            }
            else if (message is WelcomeResponse)
            {

            }
            else if (message is HeartbeatResponse)
            {
                HeartbeatResponse heartbeatResponse = (HeartbeatResponse)message;

                GossipPeer senderPeer = heartbeatResponse.Members?.FirstOrDefault();

                MergeLists(senderPeer, heartbeatResponse.Members);
            }
            else if (message is ErrorResponse)
            {
                Console.WriteLine(((ErrorResponse)message).ErrorMessage);
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
            if (!_requests.Any(x => x.Key == message.Id))
            {
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All
                };

                byte[] json_bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message, settings));

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

                _requests.Add(_gossipNode.GossipPeer, "Greatings");
                // _gossipNode.GossipPeer.StartTimer();
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
