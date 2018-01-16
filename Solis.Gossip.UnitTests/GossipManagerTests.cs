using NUnit.Framework;
using Solis.Gossip.Service;

namespace Solis.Gossip.UnitTests
{
    [TestFixture]
    public class GossipManagerTests
    {
        [Test]
        public void StateMachine_WalkThroghtStates_Success()
        {
            var fakeNode = new GossipNode(new GossipSettings());
            GossipManager manager = new GossipManager(fakeNode);
            manager.Fire(Model.Events.GossipEvent.SendGreetings);
        }
    }
}
