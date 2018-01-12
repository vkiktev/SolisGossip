using System;
using Solis.Gossip.Model;

namespace Solis.Gossip.Service
{
    public class GossipPeerEventArgs : EventArgs
    {
        private GossipPeer _peer;
        public GossipPeerEventArgs(GossipPeer peer)
        {
            _peer = peer;
        }

        public GossipPeer Peer
        {
            get
            {
                return _peer;
            }
        }
    }
}
