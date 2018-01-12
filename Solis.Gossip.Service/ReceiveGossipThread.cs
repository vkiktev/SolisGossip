using NLog;
using Solis.Gossip.Model.Settings;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Solis.Gossip.Service
{
    /// <summary>
    /// [The passive thread: reply to incoming gossip request.] This class handles the passive cycle,
    /// where this client has received an incoming message. For now, this message is always the
    /// membership list, but if you choose to gossip additional information, you will need some logic to
    /// determine the incoming message.
    /// </summary>
    public class ReceiveGossipThread : BaseGossipThread
    {
        public static Logger Logger = SolisLogFactory.GetLogger(typeof(ReceiveGossipThread));
        
        private bool _keepRunning;
        private string _cluster;
        private GossipManager _gossipManager;
        private GossipNode _gossipNode;

        private CancellationTokenSource _cts = new CancellationTokenSource();

        public ReceiveGossipThread(GossipNode gossipNode, GossipManager gossipManager)
        {
            _gossipManager = gossipManager;
            _gossipNode = gossipNode;
            try
            {                
                Logger.Debug($"Gossip service successfully initialized on {_gossipNode.GossipPeer.EndPoint}");
                Logger.Debug($"I am {_gossipNode.GossipPeer}");

                _cluster = _gossipNode.GossipPeer.ClusterName;

                if (_cluster == null)
                {
                    throw new ArgumentNullException("cluster was null");
                }
            }
            catch (Exception ex)
            {
                Logger.Warn(ex);
                throw;
            }
            _keepRunning = true;

            TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;
        }

        private void OnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            Logger.Error(e.Exception, "OnUnobservedTaskException");
        }

        public void Run()
        {
            Task.Run( async () =>
            {
                try
                {
                    using (var socket = new UdpClient(_gossipNode.GossipPeer.EndPoint))
                    {
                        while (_keepRunning && !_cts.Token.IsCancellationRequested)
                        {
                            Logger.Debug($"I'm {_gossipNode.GossipPeer.Id} steel waiting for receive...");
                            try
                            {
                                var packet = await socket.ReceiveAsync();

                                int packet_length = packet.Buffer?.Length ?? 0;
                                if (packet_length <= GossipNode.MAX_PACKET_SIZE)
                                {
                                    try
                                    {
                                        var message = Deserialize(Encoding.UTF8.GetString(packet.Buffer, 0, packet_length));

                                        Logger.Debug($"Gossip Message of type {message.GetType()} was received");
                                        await _gossipManager.Recieve(message, packet.RemoteEndPoint);
                                    }
                                    catch (Exception ex)
                                    {
                                        //TODO trap json exception
                                        Logger.Error(ex, "Unable to process message");
                                    }
                                }
                                else
                                {
                                    Logger.Error("The received message is not of the expected size, it has been dropped.");
                                }
                            }
                            catch (Exception e)
                            {
                                Logger.Error(e);
                                _keepRunning = false;
                            }

                            Logger.Debug("Wait a second before continue receive loop...");
                            await Task.Delay(1000);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error($"Poll process is NOT started '{ex.GetType()}': {ex.Message} at {ex.StackTrace}");
                    throw;
                }
            }, _cts.Token);         
        }

        public void Shutdown()
        {
            try
            {
                _cts.Cancel();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Shutdown");
                throw;
            }
        }

        public bool WaitStop()
        {
            try
            {
                _cts.Cancel();
            }
            catch(Exception ex)
            {
                Logger.Error(ex, "WaitStop");
                return false;
            }
            return true;
        }
    }
}