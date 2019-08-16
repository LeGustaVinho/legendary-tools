using System;
using LegendaryTools.Graph;

namespace LegendaryTools
{
    public class State : LinkedNode<StateMachine, State, StateConnection, StateConnectionContext>
    {
        public readonly string Name;

        public event Action<object> OnStateEnterEvent;
        public event Action<object> OnStateUpdateEvent;
        public event Action<object> OnStateExitEvent;

        public State(string name, StateMachine owner) : base(owner)
        {
            Name = name;
        }

        public StateConnection ConnectTo(State state, string triggerName,  NodeConnectionDirection direction = NodeConnectionDirection.Both)
        {
            StateConnectionContext context;
            context.TriggerName = triggerName;
            return base.ConnectTo(state, context, direction);
        }

        public StateMachine CreateSubStateMachine()
        {
            StateMachine newStateMachine = new StateMachine(Name, this);
            AddSubGraph(newStateMachine);
            return newStateMachine;
        }
        
        protected virtual void OnStateEnter(object arg)
        {
            
        }
        
        protected virtual void OnStateUpdate(object arg)
        {
            
        }
        
        protected virtual void OnStateExit(object arg)
        {
            
        }
    }
}