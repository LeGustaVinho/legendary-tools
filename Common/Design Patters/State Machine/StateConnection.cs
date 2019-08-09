using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LegendaryTools
{
    public class StateConnection : NodeConnection
    {
        public readonly string TriggerName;
        
        public StateConnection(string triggerName, State @from, State to, NodeConnectionType type, NodeConnectionDirection direction = NodeConnectionDirection.Both, float weight = 0) : base(@from, to, type, direction, weight)
        {
            TriggerName = triggerName;
        }
    }
}