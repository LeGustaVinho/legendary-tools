using System;
using LegendaryTools.Graph;

namespace LegendaryTools
{
    [Serializable]
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
        
        public State(string name, StateMachine owner, StateMachine subStateMachine) : this(name, owner)
        {
            SubGraph = subStateMachine;
        }
        
        public StateConnection ConnectTo(State state, string triggerName,  NodeConnectionDirection direction = NodeConnectionDirection.Bidirectional)
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