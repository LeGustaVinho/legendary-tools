using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LegendaryTools.Systems
{
    [Serializable]
    public class AttributeModifierCondition<T>
    {
        public T AttributeName;
        public AttributeModOperator Operator;
        public float Value;
    }
}


