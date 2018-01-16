using System.Collections.Generic;

namespace Solis.Gossip.Model.Messages
{
    public class HeartbeatRequest : BaseMessage
    {
        public HeartbeatRequest(GossipPeer peer)
        {
            Peer = peer;
            Members = new List<GossipPeer>();
        }

        public List<GossipPeer> Members { get; private set; }

        public GossipPeer Peer { get; set; }
    }
}
