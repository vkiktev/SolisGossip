using Solis.Gossip.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Solis.Gossip.Service.Contracts
{
    public interface IGossipNode
    {
        IGossipPeer GossipPeer
        {
            get;
        }

        void AddPeer(IGossipPeer peer);

        void WakeUpPeer(IGossipPeer peer);

        void DownPeer(IGossipPeer peer);

        void Init();

        void Shutdown();
    }
}
