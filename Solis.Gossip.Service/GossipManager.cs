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
        private ConcurrentDictionary<string, BaseMessage> requests;
        private TaskFactory service;
        private CancellationTokenSource cts;

        public GossipManager(GossipNode gossipNode)
        {
            _gossipNode = gossipNode;
            requests = new ConcurrentDictionary<string, BaseMessage>();
            service = new TaskFactory();
            cts = new CancellationTokenSource();
        }

        public void Shutdown()
        {
            try
            {
                cts.Cancel();
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
                if(message is HeartbeatRequest)
                {                    
                    var request = ((HeartbeatRequest)message);
                    if (request.IsGreetings)
                    {
                        _gossipNode.AddPeer(request.Peer);
                    }
                    else
                    {
                        var needReply = MergeLists(request.Peer, request.Members);

                        if (needReply)
                        {
                            HeartbeatResponse o = new HeartbeatResponse();
                            o.Members.Add(_gossipNode.GossipPeer);
                            o.Members.AddRange(_gossipNode.RemotePeers);
                            o.UriFrom = message.UriFrom;
                            o.Id = message.Id;
                            o.RequestMessageId = message.Id;

                            await SendOneWay(o, remoteEndPoint);
                        }
                    }
                }
            }
            else if (message is HeartbeatResponse)
            {
                HeartbeatResponse heartbeatResponse = (HeartbeatResponse)message;
                GossipPeer senderPeer = heartbeatResponse.Members?.FirstOrDefault();

                MergeLists(senderPeer, heartbeatResponse.Members);
            }
        }

        private async Task SendInternal(BaseMessage message, IPEndPoint endPoint)
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
                    Logger.Error(e, "SendInternal");
                    throw;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="uri"></param>
        /// <returns></returns>
        public async Task<Response> SendAsync(BaseMessage message, IPEndPoint endPoint)
        {
            await SendInternal(message, endPoint);

            Task<Response> task = Task.Run(async () =>
               {
                   while (true)
                   {
                       BaseMessage baseMsg = null;
                       var success = requests.TryRemove($"{message.Id}/{message.UriFrom}", out baseMsg);
                       if (success)
                       {
                           return (Response)baseMsg;
                       }

                       if(requests.Count == 0)
                       {
                           return null;
                       }

                       await Task.Delay(1000);
                   }
               });

            Response response = null;
            try
            {
                response = await task;
            }
            catch (Exception e)
            {
                Logger.Debug(e, e.Message);
                return null;
            }
            finally
            {
                BaseMessage baseMsg = null;
                requests.TryRemove($"{message.Id}/{message.UriFrom}", out baseMsg);
            }

            return response;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="u"></param>
        /// <returns></returns>
        public async Task SendOneWay(BaseMessage message, IPEndPoint endPoint)
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
                    using (UdpClient udpClient = new UdpClient())
                    {
                        await udpClient.SendAsync(json_bytes, packet_length, endPoint);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "SendOneWay");
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
