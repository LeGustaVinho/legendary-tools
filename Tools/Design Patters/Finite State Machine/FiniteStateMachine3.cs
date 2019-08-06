using System;
using System.Collections.Generic;
using UnityEngine;

namespace LegendaryTools.DesignPatters
{
    [System.Serializable]
    public class FiniteStateMachine<TState, TTransiction, TParam>
    {
        public bool Verbose;
        public string Name;

        public Dictionary<TState, FSMState<TState, TTransiction, TParam>> States = new Dictionary<TState, FSMState<TState, TTransiction, TParam>>();

        public List<FSMState<TState, TTransiction, TParam>> CurrentStates = new List<FSMState<TState, TTransiction, TParam>>();

        public Stack<FSMState<TState, TTransiction, TParam>> Memento = new Stack<FSMState<TState, TTransiction, TParam>>();

        private bool m_Running;
        public bool Running
        {
            get { return m_Running; }
        }

        public TTransiction DefaultMoveBack;
        private TState AnyState;

        private FSMTransition<TState, TTransiction, TParam> moveBacktransition = null;

        public FiniteStateMachine(string name, TTransiction defaultMoveBack, TState anyState)
        {
            Name = name;
            DefaultMoveBack = defaultMoveBack;
            AnyState = anyState;
        }

        public void SetAnyState(TState anyState)
        {
            if(!m_Running)
                AnyState = anyState;
        }

        public void Start(FSMState<TState, TTransiction, TParam> startState, TParam onStart = default(TParam))
        {
            if (!m_Running)
            {
                m_Running = true;

                if (Verbose) Debug.Log("[FSM] -> FSM " + Name + " has been started.");

                foreach (FSMState<TState, TTransiction, TParam> state in startState.Path)
                {
                    CurrentStates.Add(state);

                    if (Verbose) Debug.Log("[FSM] -> Entering into State name " + state.Name);

                    if (state.OnEnter != null)
                        state.OnEnter.Invoke(onStart);
                }

                if (Verbose) Debug.Log("[FSM] -> Entering into State name " + startState.Name);

                CurrentStates.Add(startState);

                if (startState.OnEnter != null)
                    startState.OnEnter.Invoke(onStart);
            }
        }

        public void End(TParam onEnd = default(TParam))
        {
            if (m_Running)
            {
                CurrentStates.Reverse();

                foreach (FSMState<TState, TTransiction, TParam> state in CurrentStates)
                {
                    if (state.OnExit != null)
                        state.OnExit.Invoke(onEnd);
                }

                CurrentStates.Clear();

                m_Running = false;

                if (Verbose) Debug.Log("[FSM] -> FSM " + Name + " has ended.");
            }
        }

        public void UpdateAncestors()
        {
            foreach (FSMState<TState, TTransiction, TParam> state in CurrentStates)
            {
                if (state.Update != null)
                    state.Update();
            }
        }

        public void Update()
        {
            if (m_Running)
            {
                if (CurrentStates[CurrentStates.Count - 1].Update != null)
                    CurrentStates[CurrentStates.Count - 1].Update.Invoke();
            }
        }

        public bool HasCurrentState(TState stateName)
        {
            return (CurrentStates.Find(item => item.Name.Equals(stateName)) != null);
        }

        public List<FSMState<TState, TTransiction, TParam>> GetStates()
        {
            return CurrentStates;
        }

        public List<TState> GetStateNames()
        {
            List<TState> enumStates = new List<TState>();

            foreach (FSMState<TState, TTransiction, TParam> state in CurrentStates)
                enumStates.Add(state.Name);

            return enumStates;
        }

        public TState GetLeafStateName()
        {
            return CurrentStates[CurrentStates.Count - 1].Name;
        }

        public void SendEvent(TTransiction eventName, TParam param = default(TParam))
        {
            if (Verbose) Debug.Log("[FSM] -> Receive event name: " + eventName);

            if (m_Running)
            {
                if (CurrentStates.Count > 0)
                {
                    if (eventName.Equals(DefaultMoveBack))
                    {
                        MoveBack(param);
                        if (Verbose) Debug.Log("[FSM] -> Moving back.");
                    }
                    else
                    {
                        FSMState<TState, TTransiction, TParam> stateLeaf = CurrentStates[CurrentStates.Count - 1];
                        FSMTransition<TState, TTransiction, TParam> transition = null;

                        if (States.ContainsKey(AnyState) && States[AnyState].Transitions.ContainsKey(eventName))
                        {
                            transition = new FSMTransition<TState, TTransiction, TParam>(eventName, stateLeaf, States[AnyState].Transitions[eventName].Destination, null); //create a new fake temporary transition
                            Memento.Push(stateLeaf);
                        }
                        else if (stateLeaf.Transitions.ContainsKey(eventName))
                        {
                            transition = stateLeaf.Transitions[eventName];
                            Memento.Push(transition.Origin);
                        }

                        if (transition == null)
                        {
                            if (Verbose) Debug.LogError("[FSM] -> Transition " + eventName.ToString() + " not found.");
                            return;
                        }

                        if (transition.Destination.States.Count > 0)
                        {
                            if (Verbose) Debug.LogError("[FSM] -> You cannot transit to a state that has sub states. " + stateLeaf.Name.ToString() + " -> " + transition.Destination.Name.ToString());
                            return;
                        }

                        Transit(transition, param);
                    }
                }
                else
                    Debug.LogError("[FSM] -> FSM has no states. Event received: " + eventName);
            }
        }

        public void MoveBack(TParam param = default(TParam))
        {
            if (Memento.Count > 0)
            {
                FSMState<TState, TTransiction, TParam> origin = CurrentStates[CurrentStates.Count - 1];
                FSMState<TState, TTransiction, TParam> destination = Memento.Pop();

                if (destination.CanMoveBackToHere)
                {
                    moveBacktransition = new FSMTransition<TState, TTransiction, TParam>(DefaultMoveBack, origin, destination, null);
                    Transit(moveBacktransition, param);
                }
            }
        }

        /// <summary>
        /// Estado para ser criado da m�quina de estados
        /// </summary>
        /// <param name="stateName">Nome do estado</param>
        /// <param name="onEnter">Fun��o quando entrar neste estado</param>
        /// <param name="update">Fun��o de atualiza��o do estado</param>
        /// <param name="onExit">Fun��o ao sair do estado</param>
        /// <param name="canMoveBack">true = Pode voltar para este estado ; False = N�o pode voltar para esse estado</param>
        /// <returns>Estado criado</returns>
        public FSMState<TState, TTransiction, TParam> CreateState(TState stateName, Action<TParam> onEnter = null, Action update = null, Action<TParam> onExit = null, bool canMoveBack = true)
        {
            if (!States.ContainsKey(stateName))
            {
                FSMState<TState, TTransiction, TParam> newState = new FSMState<TState, TTransiction, TParam>(stateName, onEnter, update, onExit, canMoveBack);
                States.Add(stateName, newState);
                return newState;
            }
            else
            {
                Debug.LogError("[FSM] -> An State name " + stateName + " in FSM " + Name + " already exists.");
                return null;
            }
        }

        private void Transit(FSMTransition<TState, TTransiction, TParam> transition, TParam param = default(TParam))
        {
            foreach (FSMState<TState, TTransiction, TParam> state in transition.ExitPath)
            {
                if (state.OnExit != null)
                    state.OnExit.Invoke(param);

                CurrentStates.Remove(state);

                if (Verbose) Debug.Log("[FSM] -> Leaving State name " + state.Name);
            }

            if (transition.Action != null)
                transition.Action.Invoke(param);

            if (Verbose) Debug.Log("[FSM] -> Executing transition name " + transition.Name);

            foreach (FSMState<TState, TTransiction, TParam> state in transition.EnterPath)
            {
                CurrentStates.Add(state);

                if (state.OnEnter != null)
                    state.OnEnter.Invoke(param);

                if (Verbose) Debug.Log("[FSM] -> Entering into State name " + state.Name);
            }
        }

        public void ForceState(TState newState, TParam param = default(TParam))
        {
            if (States.ContainsKey(newState))
            {
                if (CurrentStates.Count > 0)
                    CurrentStates[0].OnExit.Invoke(param);

                CurrentStates.Clear();

                var currentState = States[newState];

                CurrentStates.Add(currentState);

                if (currentState.OnEnter != null)
                    currentState.OnEnter.Invoke(param);
            }
        }

        public void CreateTransitionFromAnyState(TTransiction name, FSMState<TState, TTransiction, TParam> to, Action<TParam> action = null)
        {
            if (!States.ContainsKey(AnyState))
                CreateState(AnyState, null, null, null, false);

            States[AnyState].CreateTransition(name, to, action);
        }
    }
}