using System.Collections.Generic;

namespace Solis.Gossip.Model.Messages
{
    public class HeartbeatResponse : Response
    {
        public HeartbeatResponse()
        {
            Members = new List<GossipPeer>();
        }

        public bool IsWelcome { get; set; }

        public List<GossipPeer> Members { get; private set; }
    }
}
