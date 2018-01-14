using Newtonsoft.Json;
using NLog;
using Solis.Gossip.Model;
using Solis.Gossip.Model.Messages;
using Solis.Gossip.Model.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Solis.Gossip.Service
{
    /// <summary>
    /// [The active thread: periodically send gossip request.] The class handles gossiping the membership
    /// list. This information is important to maintaining a common state among all the nodes, and is
    /// important for detecting failures.
    /// </summary>
    public class SendGossipThread : BaseGossipThread
    {
        private static Logger Logger = SolisLogFactory.GetLogger(typeof(SendGossipThread));

        private long _heartbeatTimeout = 10 * 1000;

        private GossipNode _gossipNode;
        private Random _random;
        private GossipManager _gossipManager;

        private CancellationTokenSource _cts = new CancellationTokenSource();

        private bool _keepRunning;
        private Timer _replyTimer;
        private Timer _heartbeatTimer;

        public SendGossipThread(GossipNode gossipNode, GossipManager gossipManager)
        {
            _gossipNode = gossipNode;
            _random = new Random();
            _gossipManager = gossipManager;

            _keepRunning = true;

            _replyTimer = new Timer(new TimerCallback(ReplyTimerHandle));
            _heartbeatTimer = new Timer(new TimerCallback(HeartbeatTimerHandle));
        }

        private async void HeartbeatTimerHandle(object state)
        {
            if (!_gossipNode.RemotePeers.Any())
            {
                await SendGreetings(_gossipNode.GossipPeer);
            }
            else
            {
                try
                {
                    await SendMembershipList(_gossipNode.GossipPeer, _gossipNode.RemotePeers);
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                    _keepRunning = false;
                }
            }

            Logger.Debug("Wait a second before continue send loop...");
            await Task.Delay(1000);
        }

        private void ReplyTimerHandle(object state)
        {

        }
        
        private Task SendGreetings(GossipPeer peer)
        {
            return _gossipManager.SendOneWay(new HeartbeatRequest(peer, true), new IPEndPoint(IPAddress.Broadcast, peer.EndPoint.Port));
        }

        public void Run()
        {
            _heartbeatTimer.Change(0, _heartbeatTimeout);
        }

        /// <summary>
        /// Отменяем рабочий цикл
        /// </summary>
        public void Shutdown()
        {
            try
            {
                if(!_cts.IsCancellationRequested)
                    _cts.Cancel();
            }
            catch (Exception e)
            {
                Logger.Debug("Issue during shutdown" + e);
            }
        }

        /// <summary>
        /// Отправляем список известных members включая себя
        /// Performs the sending of the membership list, after we have incremented our own heartbeat.
        /// </summary>
        /// <param name="peer"></param>
        /// <param name="memberList"></param>
        /// <returns></returns>
        protected async Task SendMembershipList(GossipPeer peer, List<GossipPeer> memberList)
        {
            // peer.Heartbeat = DateTime.Now;
            var remotePeer = SelectNeighbor(memberList);
            if (peer == null)
            {
                Logger.Debug("SendMembershipList is called without action");
                return;
            }
            else
            {
                Logger.Debug($"SendMembershipList is called to {peer.ToString()}");
            }

            try
            {
                HeartbeatRequest message = new HeartbeatRequest(peer);
                message.Id = Guid.NewGuid().ToString();
                message.UriFrom = _gossipNode.GossipPeer.EndPoint.ToString();
                message.Members.Add(peer);
                foreach (GossipPeer other in memberList)
                {
                    message.Members.Add(other);
                }

                byte[] json_bytes = Encoding.UTF8.GetBytes(Serialize(message));
                int packet_length = json_bytes.Length;
                if (packet_length < GossipNode.MAX_PACKET_SIZE)
                {
                    Response r = await _gossipManager.SendAsync(message, peer.EndPoint);
                    if (r is HeartbeatResponse)
                    {
                        Logger.Info($"Message {message} generated response {r}");
                    }
                    else if(r is ErrorResponse)
                    {
                        Logger.Warn($"Message {message} generated response {r}");
                    }
                }
                else
                {
                    Logger.Error($"The length of the to be send message is too large ({packet_length} > {GossipNode.MAX_PACKET_SIZE}).");
                }
            }
            catch (Exception e1)
            {
                Logger.Warn(e1);
            }
        }

        /// <summary>
        /// Выбираем случайного member для коммуникаций с ним
        /// </summary>
        /// <param name="memberList">The list of members which are stored in the local list of members</param>
        /// <returns>The chosen GossipPeer to gossip with</returns>
        protected GossipPeer SelectNeighbor(List<GossipPeer> memberList)
        {
            GossipPeer peer = null;
            if (memberList.Count > 0)
            {
                int tryCount = 0;
                do
                {
                    Random random = new Random();

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
    }
}