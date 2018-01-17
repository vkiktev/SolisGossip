using System.Collections.Generic;

namespace Solis.Gossip.Model.Messages
{
    public class HeartbeatResponse : Response
    {
        public HeartbeatResponse()
        {
            Members = new List<IGossipPeer>();
        }

        public bool IsWelcome { get; set; }

        public IList<IGossipPeer> Members { get; private set; }
    }
}
