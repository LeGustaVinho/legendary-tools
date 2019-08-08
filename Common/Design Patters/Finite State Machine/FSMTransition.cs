using System;
using System.Collections.Generic;

namespace LegendaryTools.DesignPatters
{
    [System.Serializable]
    public class FSMTransition<TState, TTransiction, TParam>
    {
        public TTransiction Name;
        public FSMState<TState, TTransiction, TParam> Origin;
        public FSMState<TState, TTransiction, TParam> Destination;

        [NonSerialized]
        public Action<TParam> Action;

        public List<FSMState<TState, TTransiction, TParam>> ExitPath = new List<FSMState<TState, TTransiction, TParam>>();
        public List<FSMState<TState, TTransiction, TParam>> EnterPath = new List<FSMState<TState, TTransiction, TParam>>();

        public FSMTransition(TTransiction name, FSMState<TState, TTransiction, TParam> fromState, FSMState<TState, TTransiction, TParam> toState, Action<TParam> action = null)
        {
            Name = name;
            Origin = fromState;
            Destination = toState;
            Action = action;

            ExitPath.Add(Origin);
            foreach (FSMState<TState, TTransiction, TParam> state in Origin.Path)
            {
                if (!Destination.Path.Contains(state))
                {
                    ExitPath.Add(state);
                }
            }

            foreach (FSMState<TState, TTransiction, TParam> state in Destination.Path)
            {
                if (!Origin.Path.Contains(state))
                {
                    EnterPath.Add(state);
                }
            }
            EnterPath.Add(Destination);
        }
    }
}