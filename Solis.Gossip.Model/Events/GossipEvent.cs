namespace Solis.Gossip.Model.Events
{
    public enum GossipEvent
    {
        SendGreetings,
        StartWaitingForConnection,
        ReceiveWelcome,
        GreetingsExpired,
        HeartbeatTimerExpired,
        ReceiveHeartbeat,
        ReceiveGreetings
    }
}
