using Solis.Gossip.Model.Messages;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Solis.Gossip.Service.Contracts
{
    public interface IGossipManager
    {
        Task Recieve(BaseMessage message, IPEndPoint remoteEndPoint);

        void Shutdown();
    }
}
