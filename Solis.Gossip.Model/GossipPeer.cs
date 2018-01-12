using System;
using System.Net;
using Solis.Gossip.Model.Events;

namespace Solis.Gossip.Model
{
    /// <summary>
    /// The peer of the claster 
    /// </summary>
    public class GossipPeer : IGossipPeer
    {
        public GossipPeer(string clusterName, IPEndPoint endPoint, string id, DateTime heartbeat)
        {
            ClusterName = clusterName;
            EndPoint = endPoint;
            Id = id;

            Heartbeat = heartbeat;
        }

        public GossipPeerState State { get; set; }

        public IPEndPoint EndPoint { get; set; }

        public string Id { get; private set; }

        public string ClusterName { get; set; }
        
        public DateTime Heartbeat { get; set; }

        public void DisableTimer()
        {
            throw new System.NotImplementedException();
        }

        public void StartTimer()
        {
            throw new System.NotImplementedException();
        }

        public override string ToString()
        {
            return $"Cluster: {ClusterName}, Id: {Id}, EndPoint: {EndPoint}";
        }
    }
}
