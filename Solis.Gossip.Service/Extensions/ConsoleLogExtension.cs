using Appccelerate.StateMachine;
using Appccelerate.StateMachine.AsyncMachine;
using Appccelerate.StateMachine.Extensions;
using EnsureThat;
using Solis.Gossip.Model.Events;
using System;

namespace Solis.Gossip.Service
{
    public class ConsoleLogExtension : IExtension<NodeState, GossipEvent>
    {
        /// <summary>
        /// Called when the state machine is initializing.
        /// </summary>
        /// <param name="stateMachine">The state machine.</param>
        /// <param name="initialState">The initial state. Can be replaced by the extension.</param>
        public void InitializingStateMachine(IStateMachineInformation<NodeState, GossipEvent> stateMachine, ref NodeState initialState)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("State machine {0} initializes to state {1}.", stateMachine, initialState);
            Console.ResetColor();
        }

        /// <summary>
        /// Called when the state machine was initialized.
        /// </summary>
        /// <param name="stateMachine">The state machine.</param>
        /// <param name="initialState">The initial state.</param>
        public void InitializedStateMachine(IStateMachineInformation<NodeState, GossipEvent> stateMachine, NodeState initialState)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("State machine {0} initialized to state {1}", stateMachine, initialState);
            Console.ResetColor();
        }

        /// <summary>
        /// Called when the state machine enters the initial state.
        /// </summary>
        /// <param name="stateMachine">The state machine.</param>
        /// <param name="state">The state.</param>
        public void EnteringInitialState(IStateMachineInformation<NodeState, GossipEvent> stateMachine, NodeState state)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("State machine {0} enters initialstate {1}.", stateMachine, state);
            Console.ResetColor();
        }

        /// <summary>
        /// Called when an event is firing on the state machine.
        /// </summary>
        /// <param name="stateMachine">The state machine.</param>
        /// <param name="eventId">The event id. Can be replaced by the extension.</param>
        /// <param name="eventArgument">The event argument. Can be replaced by the extension.</param>
        public void FiringEvent(IStateMachineInformation<NodeState, GossipEvent> stateMachine, ref GossipEvent eventId, ref object eventArgument)
        {
            EnsureArg.IsNotNull(stateMachine, "stateMachine");

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(
                    "Fire event {0} on state machine {1} with current state {2} and event argument {3}.",
                    eventId,
                    stateMachine.Name,
                    stateMachine.CurrentStateId,
                    eventArgument);
            Console.ResetColor();
        }

        public void FiredEvent(IStateMachineInformation<NodeState, GossipEvent> stateMachine, ITransitionContext<NodeState, GossipEvent> context)
        {
            EnsureArg.IsNotNull(stateMachine, "stateMachine");
            EnsureArg.IsNotNull(context, "context");

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("State machine {0} performed {1}.", stateMachine.Name, context.GetRecords());
            Console.ResetColor();
        }

        public void SwitchedState(IStateMachineInformation<NodeState, GossipEvent> stateMachine, IState<NodeState, GossipEvent> oldState, IState<NodeState, GossipEvent> newState)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(
                "State machine {0} switched from state {1} to state {2}.",
                stateMachine,
                oldState,
                newState);
            Console.ResetColor();
        }

        public void StartedStateMachine(IStateMachineInformation<NodeState, GossipEvent> stateMachine)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(
                "State machine {0} started.",
                stateMachine);
            Console.ResetColor();
        }

        public void StoppedStateMachine(IStateMachineInformation<NodeState, GossipEvent> stateMachine)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(
                "State machine {0} stopped.",
                stateMachine);
            Console.ResetColor();
        }

        public void EventQueued(IStateMachineInformation<NodeState, GossipEvent> stateMachine, GossipEvent eventId, object eventArgument)
        {
            
        }

        public void EventQueuedWithPriority(IStateMachineInformation<NodeState, GossipEvent> stateMachine, GossipEvent eventId, object eventArgument)
        {
            
        }

        public void EnteredInitialState(IStateMachineInformation<NodeState, GossipEvent> stateMachine, NodeState state, ITransitionContext<NodeState, GossipEvent> context)
        {
            
        }

        public void HandlingEntryActionException(IStateMachineInformation<NodeState, GossipEvent> stateMachine, IState<NodeState, GossipEvent> state, ITransitionContext<NodeState, GossipEvent> context, ref Exception exception)
        {
            
        }

        public void HandledEntryActionException(IStateMachineInformation<NodeState, GossipEvent> stateMachine, IState<NodeState, GossipEvent> state, ITransitionContext<NodeState, GossipEvent> context, Exception exception)
        {
            
        }

        public void HandlingExitActionException(IStateMachineInformation<NodeState, GossipEvent> stateMachine, IState<NodeState, GossipEvent> state, ITransitionContext<NodeState, GossipEvent> context, ref Exception exception)
        {
            
        }

        public void HandledExitActionException(IStateMachineInformation<NodeState, GossipEvent> stateMachine, IState<NodeState, GossipEvent> state, ITransitionContext<NodeState, GossipEvent> context, Exception exception)
        {
            
        }

        public void HandlingGuardException(IStateMachineInformation<NodeState, GossipEvent> stateMachine, ITransition<NodeState, GossipEvent> transition, ITransitionContext<NodeState, GossipEvent> transitionContext, ref Exception exception)
        {
            
        }

        public void HandledGuardException(IStateMachineInformation<NodeState, GossipEvent> stateMachine, ITransition<NodeState, GossipEvent> transition, ITransitionContext<NodeState, GossipEvent> transitionContext, Exception exception)
        {
            
        }

        public void HandlingTransitionException(IStateMachineInformation<NodeState, GossipEvent> stateMachine, ITransition<NodeState, GossipEvent> transition, ITransitionContext<NodeState, GossipEvent> context, ref Exception exception)
        {
            
        }

        public void HandledTransitionException(IStateMachineInformation<NodeState, GossipEvent> stateMachine, ITransition<NodeState, GossipEvent> transition, ITransitionContext<NodeState, GossipEvent> transitionContext, Exception exception)
        {
            
        }

        public void SkippedTransition(IStateMachineInformation<NodeState, GossipEvent> stateMachineInformation, ITransition<NodeState, GossipEvent> transition, ITransitionContext<NodeState, GossipEvent> context)
        {
            
        }

        public void ExecutingTransition(IStateMachineInformation<NodeState, GossipEvent> stateMachineInformation, ITransition<NodeState, GossipEvent> transition, ITransitionContext<NodeState, GossipEvent> context)
        {
            
        }

        public void ExecutedTransition(IStateMachineInformation<NodeState, GossipEvent> stateMachineInformation, ITransition<NodeState, GossipEvent> transition, ITransitionContext<NodeState, GossipEvent> context)
        {
            
        }
    }
}
