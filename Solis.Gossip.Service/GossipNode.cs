using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using NLog;
using Solis.Gossip.Model;
using Solis.Gossip.Model.Settings;

namespace Solis.Gossip.Service
{
    /// <summary>
    /// The gossip node
    /// </summary>
    public class GossipNode 
    {
        public static Logger Logger = SolisLogFactory.GetLogger(typeof(GossipNode));
        public static int MAX_PACKET_SIZE = 102400;

        private ConcurrentDictionary<string, GossipPeer> _remotePeers;        
        private GossipPeer _gossipPeer;
        private GossipSettings _settings;
        private bool _gossipServiceRunning;

        private SendGossipThread _activeGossipThread;
        private ReceiveGossipThread _passiveGossipThread;

        private GossipManager _gossipManager;

        public event EventHandler<GossipPeerEventArgs> NewPeerFound;
        public event EventHandler<GossipPeerEventArgs> PeerWakeUp;
        public event EventHandler<GossipPeerEventArgs> PeerDown;

        public GossipNode(GossipSettings settings)
        {
            _settings = settings;
            _gossipManager = new GossipManager(this);
            _remotePeers = new ConcurrentDictionary<string, GossipPeer>();
            
            _gossipServiceRunning = true;
        }

        public void AddPeer(GossipPeer peer)
        {
            _remotePeers.TryAdd(peer.Id, peer);
            NewPeerFound?.Invoke(this, new GossipPeerEventArgs(peer));
        }

        internal void WakeUpPeer(GossipPeer peer)
        {
            _remotePeers.AddOrUpdate(peer.Id, peer, (id, p) =>
            {
                p.State = peer.State;
                return p;
            });

            PeerWakeUp?.Invoke(this, new GossipPeerEventArgs(peer));
        }

        public void DownPeer(GossipPeer peer)
        {
            _remotePeers.AddOrUpdate(peer.Id, peer, (id, p) =>
            {
                p.State = peer.State;
                return p;
            });

            PeerDown?.Invoke(this, new GossipPeerEventArgs(peer));
        }

        public GossipSettings Settings
        {
            get
            {
                return _settings;
            }
        }

        /// <summary>
        /// Read only list of remote peers
        /// </summary>
        public List<GossipPeer> RemotePeers
        {
            get
            {
                return _remotePeers.Values.ToList();
            }
        }

        public GossipPeer GossipPeer
        {
            get
            {
                return _gossipPeer;
            }
        }

        /// <summary>
        /// Initialize the client. 
        /// Start the gossip active thread and start the passive thread.
        /// </summary>
        public void Init()
        {
            _passiveGossipThread = new ReceiveGossipThread(this, _gossipManager);
            _passiveGossipThread.Run();

            _activeGossipThread = new SendGossipThread(this, _gossipManager);
            _activeGossipThread.Run();

            Logger.Debug("The Gossip Node is initialized.");
        }

        /// <summary>
        /// Shutdown the gossip service.
        /// </summary>
        public void Shutdown()
        {
            _gossipServiceRunning = false;
            _gossipManager.Shutdown();

            if (_passiveGossipThread != null)
            {
                _passiveGossipThread.Shutdown();
            }

            if (_activeGossipThread != null)
            {
                _activeGossipThread.Shutdown();
            }
        }
    }
}