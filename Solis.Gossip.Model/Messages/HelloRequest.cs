using System;
using System.Collections.Generic;
using System.Text;

namespace Solis.Gossip.Model.Messages
{
    public class HelloRequest : HeartbeatRequest
    {
        public HelloRequest(GossipPeer peer) 
            : base(peer)
        {
        }
    }
}
