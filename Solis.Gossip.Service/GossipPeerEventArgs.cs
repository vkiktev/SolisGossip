using System;
using Solis.Gossip.Model;

namespace Solis.Gossip.Service
{
    public class GossipPeerEventArgs : EventArgs
    {
        private IGossipPeer _peer;
        public GossipPeerEventArgs(IGossipPeer peer)
        {
            _peer = peer;
        }

        public IGossipPeer Peer
        {
            get
            {
                return _peer;
            }
        }
    }
}
