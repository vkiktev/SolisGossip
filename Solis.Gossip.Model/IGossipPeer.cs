using Solis.Gossip.Model.Events;
using System;
using System.Net;

namespace Solis.Gossip.Model
{
    public interface IGossipPeer
    {
        GossipPeerState State { get; set; }

        IPEndPoint EndPoint { get; set; }

        string Id { get; }

        string ClusterName { get; set; }

        DateTime Heartbeat { get; set; }

        void StartTimer();

        void DisableTimer();
    }
}
