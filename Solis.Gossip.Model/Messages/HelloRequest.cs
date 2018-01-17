using System;
using System.Collections.Generic;
using System.Text;

namespace Solis.Gossip.Model.Messages
{
    public class HelloRequest : HeartbeatRequest
    {
        public HelloRequest(IGossipPeer peer) 
            : base(peer)
        {
        }
    }
}
