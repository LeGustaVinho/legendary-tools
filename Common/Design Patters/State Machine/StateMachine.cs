using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace LegendaryTools
{
    public class StateMachine : NodeGraph
    {
        public Dictionary<string, State> statesLookup = new Dictionary<string, State>();
        public Dictionary<string, StateConnection> connectionssLookup = new Dictionary<string, StateConnection>();
        
        public void Trigger(string connectionName)
        {
            
        }

        public State CreateState(string stateName)
        {
            return new State(stateName, this);
        }
        
        public void Update()
        {
            
        }

        protected override void OnAddNode(Node newNode)
        {
            State newState = newNode as State;
            if (newState == null)
            {
                Debug.LogError("[StateMachine:OnAddNode("+newNode.ID+")] -> Node is not State.");
                return;
            }
            
            if(!statesLookup.ContainsKey(newState.Name))
                statesLookup.Add(newState.Name, newState);
            else
                Debug.LogError("[StateMachine:OnAddNode("+newNode.ID+")] -> StateMachine already contains a state with name " + newState.Name);
        }

        protected override void OnRemoveNode(Node node)
        {
            State newState = node as State;
            if (newState == null)
            {
                Debug.LogError("[StateMachine:OnRemoveNode("+node.ID+")] -> Node is not State.");
                return;
            }
            
            if(statesLookup.ContainsKey(newState.Name))
                statesLookup.Remove(newState.Name);
            else
                Debug.LogError("[StateMachine:OnRemoveNode("+node.ID+")] -> StateMachine does not contains a state with name " + newState.Name);
        }

        protected override void OnAddConnection(NodeConnection newConnection)
        {
            StateConnection newStateConnection = newConnection as StateConnection;
            if (newStateConnection == null)
            {
                Debug.LogError("[StateMachine:OnAddConnection("+newConnection.ID+")] -> NodeConnection is not StateConnection.");
                return;
            }
            
            if(!connectionssLookup.ContainsKey(newStateConnection.TriggerName))
                connectionssLookup.Add(newStateConnection.TriggerName, newStateConnection);
            else
                Debug.LogError("[StateMachine:OnAddConnection("+newConnection.ID+")] -> StateMachine already contains a state connection with name " + newStateConnection.TriggerName);
        }

        protected override void OnRemoveConnection(NodeConnection connection)
        {
            StateConnection stateConnection = connection as StateConnection;
            if (stateConnection == null)
            {
                Debug.LogError("[StateMachine:OnRemoveConnection("+connection.ID+")] -> NodeConnection is not StateConnection.");
                return;
            }
            
            if(connectionssLookup.ContainsKey(stateConnection.TriggerName))
                connectionssLookup.Remove(stateConnection.TriggerName);
            else
                Debug.LogError("[StateMachine:OnRemoveConnection("+connection.ID+")] -> StateMachine does not contains a state connection with name " + stateConnection.TriggerName);
        }
    }
}