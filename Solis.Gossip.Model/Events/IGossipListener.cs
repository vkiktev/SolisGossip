namespace Solis.Gossip.Model.Events
{
    public interface IGossipListener
    {
        void GossipEvent(GossipPeer member, GossipPeerState state);
    }
}
