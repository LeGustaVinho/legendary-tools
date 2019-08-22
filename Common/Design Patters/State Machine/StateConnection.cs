using System;
using System.Collections;
using System.Collections.Generic;
using LegendaryTools.Graph;

namespace LegendaryTools
{
    [Serializable]
    public struct StateConnectionContext
    {
        public string Trigger;
    }
    
    [Serializable] 
    public class StateConnection : NodeConnection<StateMachine, State, StateConnection, StateConnectionContext>
    {
        public string Trigger => Context.Trigger;

        public StateConnection(string trigger, 
            State @from, 
            State to, 
            NodeConnectionDirection direction = NodeConnectionDirection.Bidirectional, 
            float weight = 0) : base(to, from, direction, weight)
        {
            Context.Trigger = trigger;
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