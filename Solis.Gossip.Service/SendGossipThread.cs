using NLog;
using Solis.Gossip.Model.Settings;
using System;
using System.Threading;

namespace Solis.Gossip.Service
{
    /// <summary>
    /// [The active thread: periodically send gossip request.] The class handles gossiping the membership
    /// list. This information is important to maintaining a common state among all the nodes, and is
    /// important for detecting failures.
    /// </summary>
    public class SendGossipThread : BaseGossipThread
    {
        private static Logger Logger = SolisLogFactory.GetLogger(typeof(SendGossipThread));

        private long _heartbeatTimeout = 10 * 1000;

        private GossipNode _gossipNode;
        private Random _random;
        private GossipManager _gossipManager;

        private CancellationTokenSource _cts = new CancellationTokenSource();
        private bool _keepRunning;
        private Timer _replyTimer;
        private Timer _heartbeatTimer;

        public SendGossipThread(GossipNode gossipNode, GossipManager gossipManager)
        {
            _gossipNode = gossipNode;
            _random = new Random();
            _gossipManager = gossipManager;

            _keepRunning = true;

            _replyTimer = new Timer(new TimerCallback(ReplyTimerHandle));
            _heartbeatTimer = new Timer(new TimerCallback(HeartbeatTimerHandle));
        }

        private async void HeartbeatTimerHandle(object state)
        {
            
        }

        private void ReplyTimerHandle(object state)
        {

        }

        public void Run()
        {
            _heartbeatTimer.Change(0, _heartbeatTimeout);
        }

        /// <summary>
        /// Отменяем рабочий цикл
        /// </summary>
        public void Shutdown()
        {
            try
            {
                if(!_cts.IsCancellationRequested)
                    _cts.Cancel();
            }
            catch (Exception e)
            {
                Logger.Debug("Issue during shutdown" + e);
            }
        }
    }
}