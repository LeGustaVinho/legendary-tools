using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using LegendaryTools.Graph;

namespace LegendaryTools
{
    public class StateMachine : LinkedGraph<StateMachine, State, StateConnection, StateConnectionContext>
    {
        public string Name;
        
        private readonly Dictionary<string, State> statesLookup = new Dictionary<string, State>();
        private readonly Dictionary<string, StateConnection> connectionsLookup = new Dictionary<string, StateConnection>();

        public StateMachine(string name, State state = null)
        {
            Name = name;
        }
        
        public void Trigger(string connectionName, object arg)
        {
            
        }

        public State CreateState(string stateName)
        {
            return new State(stateName, this);
        }
        
        public void Update()
        {
            
        }

        public override StateConnection CreateConnection(State @from, State to, StateConnectionContext context,
            NodeConnectionDirection direction = NodeConnectionDirection.Both, float weight = 0)
        {
            return new StateConnection(@from, to, context, direction, weight);
        }
    }
}