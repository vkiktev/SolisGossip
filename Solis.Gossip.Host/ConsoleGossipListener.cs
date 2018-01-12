using Solis.Gossip.Model;
using Solis.Gossip.Model.Events;
using System;

namespace Solis.Gossip.Host
{
    class ConsoleGossipListener : IGossipListener
    {
        public virtual void GossipEvent(GossipPeer member, GossipPeerState state)
        {
            Console.WriteLine($"{member.Id}, {state}");
        }
    }
}
