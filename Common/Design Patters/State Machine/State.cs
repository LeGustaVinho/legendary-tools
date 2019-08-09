using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LegendaryTools
{
    public class State : Node
    {
        public readonly string Name;

        public event Action OnStateEnterEvent;
        public event Action OnStateUpdateEvent;
        public event Action OnStateExitEvent;

        public State(string name, StateMachine owner) : base(owner)
        {
            Name = name;
        }

        public StateConnection ConnectTo(State state, string triggerName, NodeConnectionType type = NodeConnectionType.Common, NodeConnectionDirection direction = NodeConnectionDirection.Both)
        {
            return new StateConnection(triggerName,this, state, type, direction);
        }

        protected virtual void OnStateEnter()
        {
            
        }
        
        protected virtual void OnStateUpdate()
        {
            
        }
        
        protected virtual void OnStateExit()
        {
            
        }
    }
}