using System.Collections;
using System.Collections.Generic;
using LegendaryTools.Graph;
using UnityEngine;

namespace LegendaryTools
{
    public struct StateConnectionContext
    {
        public string TriggerName;
    }
    
    public class StateConnection : NodeConnection<StateMachine, State, StateConnection, StateConnectionContext>
    {
        private StateConnectionContext Context;

        public string TriggerName
        {
            get { return Context.TriggerName; }
        }
        
        public StateConnection(string triggerName, 
            State @from, 
            State to, 
            NodeConnectionDirection direction = NodeConnectionDirection.Both, 
            float weight = 0) : base(to, from, direction, weight)
        {
            Context.TriggerName = triggerName;
        }
        
        public StateConnection(State @from, 
            State to, 
            StateConnectionContext context,
            NodeConnectionDirection direction = NodeConnectionDirection.Both, 
            float weight = 0) : base(to, from, direction, weight)
        {
            Context = context;
        }
    }
}