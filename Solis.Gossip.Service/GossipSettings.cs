using Solis.Gossip.Model;
using System;
using System.Collections.Generic;

namespace Solis.Gossip.Service
{
    /// <summary>
    /// Settings for GossipService
    /// </summary>
    public class GossipSettings
    {
        public string Id { get; set; }

        public int Port { get; set; }

        public string ClusterName { get; set; }
        
        /// <summary>
        /// The cleanup interval. This is the time in ms between the last heartbeat received from a member and when it will be marked as dead
        /// </summary>
        public int CleanupInterval { get; set; } = 1000;

        /// <summary>
        /// The gossip interval. This is the time in ms between a gossip message is send
        /// </summary>
        public int GossipInterval { get; set; } = 1000;

        public List<GossipPeer> Members
        {
            get;
            set;
        }
    }
}