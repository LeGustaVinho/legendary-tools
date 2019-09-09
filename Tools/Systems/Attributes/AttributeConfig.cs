using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LegendaryTools.Systems
{
    public class AttributeConfig<T> : ScriptableObject
    {
        public T ID;
        
        public Vector2 MinMaxValue;
        
        public bool HasFlags => FlagOptions.Count > 0;

        public List<string> FlagOptions = new List<string>();

        public int FlagOptionEverythingValue => (int)Mathf.Pow(2, FlagOptions.Count) - 1;

        public bool HasStackPenault => StackPenaults != null && StackPenaults.Length > 0;

        public float[] StackPenaults;

        public bool HasCapacity;

        public bool AllowExceedCapacity;

        public float MinCapacity;
    }
}