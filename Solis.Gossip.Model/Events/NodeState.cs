namespace Solis.Gossip.Model.Events
{
    public enum NodeState
    {
        Incorrect,
        Initialized,
        HelloSent,
        Infected,
        Susceptible,
        HeartbeatSent
    }
}
