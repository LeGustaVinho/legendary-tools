using System;

namespace LegendaryTools.DesignPatters
{
    [System.Serializable]
    public class FSMStateData<TEnumState, TEnumTransiction, TParam>
        where TEnumState : struct, IConvertible, IComparable, IFormattable
        where TEnumTransiction : struct, IConvertible, IComparable, IFormattable
    {
        public FSMState<TEnumState, TEnumTransiction, TParam> State;
        public TParam Data;

        public FSMStateData(FSMState<TEnumState, TEnumTransiction, TParam> state, TParam data)
        {
            State = state;
            Data = data;
        }
    }
}