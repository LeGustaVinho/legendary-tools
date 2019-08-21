using System;
using System.Collections.Generic;
using LegendaryTools.Graph;

namespace LegendaryTools
{
    [Serializable]
    public class StateMachine : LinkedGraph<StateMachine, State, StateConnection, StateConnectionContext>
    {
        public string Name;
        
        private readonly Dictionary<string, State> statesLookup = new Dictionary<string, State>();
        private readonly Dictionary<string, StateConnection> connectionsLookup = new Dictionary<string, StateConnection>();

        public StateMachine(string name, State state = null) : base(state)
        {
            Name = name;
        }
        
        public void Trigger(string connectionName, object arg)
        {
            
        }

        public State CreateState(string stateName)
        {
            State newState = new State(stateName, this);
            newState.OnConnectionRemove += onConnectionRemove;
            cache(newState);
            Add(newState);
            return newState;
        }

        public void DestroyState(string stateName)
        {
            if (statesLookup.ContainsKey(stateName))
                DestroyState(statesLookup[stateName]);
        }
        
        public void DestroyState(State state)
        {
            if (statesLookup.ContainsKey(state.Name))
            {
                StateConnection[] allConnections = state.AllConnections;
                for (int i = 0; i < allConnections.Length; i++)
                {
                    allConnections[i].Disconnect();
                }

                state.OnConnectionRemove -= onConnectionRemove;
                statesLookup.Remove(state.Name);
                Remove(state);
            }
        }
        
        public void Start(object param)
        {
            
        }

        public void Stop()
        {
            
        }
        
        public void Update()
        {
            
        }

        public override StateConnection CreateConnection(State @from, State to, StateConnectionContext context,
            NodeConnectionDirection direction = NodeConnectionDirection.Bidirectional, float weight = 0)
        {
            StateConnection newStateConnection = new StateConnection(@from, to, context, direction, weight);
            cache(newStateConnection);
            return newStateConnection;
        }

        private void cache(State newState)
        {
            if (!statesLookup.ContainsKey(newState.Name))
            {
                statesLookup.Add(newState.Name, newState);
            }
        }
        
        private void cache(StateConnection newStateConnection)
        {
            if (!connectionsLookup.ContainsKey(newStateConnection.TriggerName))
            {
                connectionsLookup.Add(newStateConnection.TriggerName, newStateConnection);
            }
        }
        
        private void onConnectionRemove(StateConnection stateConnection)
        {
            if (connectionsLookup.ContainsKey(stateConnection.TriggerName))
                connectionsLookup.Remove(stateConnection.TriggerName);
        }
    }
}