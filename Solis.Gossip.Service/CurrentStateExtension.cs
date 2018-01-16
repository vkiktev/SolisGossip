using Appccelerate.StateMachine;
using Appccelerate.StateMachine.AsyncMachine;
using Solis.Gossip.Model.Events;
using System;

namespace Solis.Gossip.Service
{
    public class CurrentStateExtension : IExtension<NodeState, GossipEvent>
    {
        public NodeState CurrentState { get; private set; }

        public void EnteredInitialState(IStateMachineInformation<NodeState, GossipEvent> stateMachine, NodeState state, ITransitionContext<NodeState, GossipEvent> context)
        {
        }

        public void EnteringInitialState(IStateMachineInformation<NodeState, GossipEvent> stateMachine, NodeState state)
        {
        }

        public void EventQueued(IStateMachineInformation<NodeState, GossipEvent> stateMachine, GossipEvent eventId, object eventArgument)
        {
        }

        public void EventQueuedWithPriority(IStateMachineInformation<NodeState, GossipEvent> stateMachine, GossipEvent eventId, object eventArgument)
        {
        }

        public void ExecutedTransition(IStateMachineInformation<NodeState, GossipEvent> stateMachineInformation, ITransition<NodeState, GossipEvent> transition, ITransitionContext<NodeState, GossipEvent> context)
        {
        }

        public void ExecutingTransition(IStateMachineInformation<NodeState, GossipEvent> stateMachineInformation, ITransition<NodeState, GossipEvent> transition, ITransitionContext<NodeState, GossipEvent> context)
        {
        }

        public void FiredEvent(IStateMachineInformation<NodeState, GossipEvent> stateMachine, ITransitionContext<NodeState, GossipEvent> context)
        {
        }

        public void FiringEvent(IStateMachineInformation<NodeState, GossipEvent> stateMachine, ref GossipEvent eventId, ref object eventArgument)
        {
        }

        public void HandledEntryActionException(IStateMachineInformation<NodeState, GossipEvent> stateMachine, IState<NodeState, GossipEvent> state, ITransitionContext<NodeState, GossipEvent> context, Exception exception)
        {
        }

        public void HandledExitActionException(IStateMachineInformation<NodeState, GossipEvent> stateMachine, IState<NodeState, GossipEvent> state, ITransitionContext<NodeState, GossipEvent> context, Exception exception)
        {
        }

        public void HandledGuardException(IStateMachineInformation<NodeState, GossipEvent> stateMachine, ITransition<NodeState, GossipEvent> transition, ITransitionContext<NodeState, GossipEvent> transitionContext, Exception exception)
        {
        }

        public void HandledTransitionException(IStateMachineInformation<NodeState, GossipEvent> stateMachine, ITransition<NodeState, GossipEvent> transition, ITransitionContext<NodeState, GossipEvent> transitionContext, Exception exception)
        {
        }

        public void HandlingEntryActionException(IStateMachineInformation<NodeState, GossipEvent> stateMachine, IState<NodeState, GossipEvent> state, ITransitionContext<NodeState, GossipEvent> context, ref Exception exception)
        {
        }

        public void HandlingExitActionException(IStateMachineInformation<NodeState, GossipEvent> stateMachine, IState<NodeState, GossipEvent> state, ITransitionContext<NodeState, GossipEvent> context, ref Exception exception)
        {
        }

        public void HandlingGuardException(IStateMachineInformation<NodeState, GossipEvent> stateMachine, ITransition<NodeState, GossipEvent> transition, ITransitionContext<NodeState, GossipEvent> transitionContext, ref Exception exception)
        {
        }

        public void HandlingTransitionException(IStateMachineInformation<NodeState, GossipEvent> stateMachine, ITransition<NodeState, GossipEvent> transition, ITransitionContext<NodeState, GossipEvent> context, ref Exception exception)
        {
        }

        public void InitializedStateMachine(IStateMachineInformation<NodeState, GossipEvent> stateMachine, NodeState initialState)
        {
        }

        public void InitializingStateMachine(IStateMachineInformation<NodeState, GossipEvent> stateMachine, ref NodeState initialState)
        {
        }

        public void SkippedTransition(IStateMachineInformation<NodeState, GossipEvent> stateMachineInformation, ITransition<NodeState, GossipEvent> transition, ITransitionContext<NodeState, GossipEvent> context)
        {
        }

        public void StartedStateMachine(IStateMachineInformation<NodeState, GossipEvent> stateMachine)
        {
        }

        public void StoppedStateMachine(IStateMachineInformation<NodeState, GossipEvent> stateMachine)
        {
        }

        public void SwitchedState(IStateMachineInformation<NodeState, GossipEvent> stateMachine, IState<NodeState, GossipEvent> oldState, IState<NodeState, GossipEvent> newState)
        {
            this.CurrentState = newState.Id;
        }
    }
}
