using System;
using System.Collections.Generic;
using UnityEngine;

namespace LegendaryTools.DesignPatters
{
    [System.Serializable]
    public class FSMState<TState, TTransiction, TParam>
    {
        public TState Name;

        //[NonSerialized]
        public Action<TParam> OnEnter;

        //[NonSerialized]
        public Action Update;

        //[NonSerialized]
        public Action<TParam> OnExit;

        public Dictionary<TState, FSMState<TState, TTransiction, TParam>> States = new Dictionary<TState, FSMState<TState, TTransiction, TParam>>();
        public Dictionary<TTransiction, FSMTransition<TState, TTransiction, TParam>> Transitions = new Dictionary<TTransiction, FSMTransition<TState, TTransiction, TParam>>();

        private FSMState<TState, TTransiction, TParam> m_Parent;
        public FSMState<TState, TTransiction, TParam> Parent
        {
            get { return m_Parent; }
        }

        public List<FSMState<TState, TTransiction, TParam>> Path = new List<FSMState<TState, TTransiction, TParam>>();

        public bool CanMoveBackToHere = true;

        public FSMState(TState stateName, Action<TParam> onEnter = null, Action update = null, Action<TParam> onExit = null, bool canMoveBackToHere = true, FSMState<TState, TTransiction, TParam> parent = null)
        {
            Name = stateName;
            this.OnEnter = onEnter;
            this.Update = update;
            this.OnExit = onExit;
            CanMoveBackToHere = canMoveBackToHere;
            m_Parent = parent;

            if (m_Parent != null)
            {
                foreach (FSMState<TState, TTransiction, TParam> state in parent.Path)
                {
                    Path.Add(state);
                }

                Path.Add(m_Parent);
            }
        }

        public FSMTransition<TState, TTransiction, TParam> CreateTransition(TTransiction name, FSMState<TState, TTransiction, TParam> to, Action<TParam> action = null)
        {
            if (!Transitions.ContainsKey(name))
            {
                FSMTransition<TState, TTransiction, TParam> newTransition = new FSMTransition<TState, TTransiction, TParam>(name, this, to, action);
                Transitions.Add(name, newTransition);

                return newTransition;
            }
            else
            {
                Debug.LogError("[State] -> An Transition name " + name + " in State " + Name + " already exists.");
                return null;
            }
        }

        public FSMState<TState, TTransiction, TParam> CreateState(TState stateName, Action<TParam> onEnter = null, Action update = null, Action<TParam> onExit = null, bool canMoveBackToHere = true)
        {
            if (!States.ContainsKey(stateName))
            {
                FSMState<TState, TTransiction, TParam> newState = new FSMState<TState, TTransiction, TParam>(stateName, onEnter, update, onExit, canMoveBackToHere, this);
                States.Add(stateName, newState);
                return newState;
            }
            else
            {
                Debug.LogError("[State] -> An State name " + stateName + " already exists.");
                return null;
            }
        }
    }
}