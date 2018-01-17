using System.Collections.Generic;

namespace Solis.Gossip.Model.Messages
{
    public class HeartbeatRequest : BaseMessage
    {
        public HeartbeatRequest(IGossipPeer peer)
        {
            Peer = peer;
            Members = new List<IGossipPeer>();
        }

        public IList<IGossipPeer> Members { get; private set; }

        public IGossipPeer Peer { get; set; }
    }
}
