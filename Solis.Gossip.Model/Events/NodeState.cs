namespace Solis.Gossip.Model.Events
{
    public enum NodeState
    {
        Incorrect,
        Initialized,
        SentGreetings,
        Infected,
        Susceptible,
        SentHeartbeat
    }
}
