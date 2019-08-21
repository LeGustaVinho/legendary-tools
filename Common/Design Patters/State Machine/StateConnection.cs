using System;
using System.Collections;
using System.Collections.Generic;
using LegendaryTools.Graph;

namespace LegendaryTools
{
    [Serializable]
    public struct StateConnectionContext
    {
        public string TriggerName;
    }
    
    [Serializable]
    public class StateConnection : NodeConnection<StateMachine, State, StateConnection, StateConnectionContext>
    {
        public string TriggerName
        {
            get { return Context.TriggerName; }
        }
        
        public StateConnection(string triggerName, 
            State @from, 
            State to, 
            NodeConnectionDirection direction = NodeConnectionDirection.Bidirectional, 
            float weight = 0) : base(to, from, direction, weight)
        {
            Context.TriggerName = triggerName;
        }
        
        public StateConnection(State @from, 
            State to, 
            StateConnectionContext context,
            NodeConnectionDirection direction = NodeConnectionDirection.Bidirectional, 
            float weight = 0) : base(to, from, direction, weight)
        {
            Context = context;
        }
    }
}