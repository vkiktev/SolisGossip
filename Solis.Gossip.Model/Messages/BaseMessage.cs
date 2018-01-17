using System.Net;

namespace Solis.Gossip.Model.Messages
{
    public class BaseMessage 
    {
        public string Id { get; set; }

        public IPEndPoint RemoteEndPoint { get; set; }
    }
}