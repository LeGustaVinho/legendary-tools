using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LegendaryTools.Graph
{

    
    public class HierarchicalNode<G, N>
        where G : HierarchicalGraph<G, N>
        where N : HierarchicalNode<G, N>
    {
        public Guid ID { get; protected set; }
        public G SubGraph { get; protected set; }
        public G Owner { get; protected set; }

        protected HierarchicalNode()
        {
            ID = Guid.NewGuid();
        }
        
        public HierarchicalNode(G owner) : this()
        {
            Owner = owner;
        }

        public void AddSubGraph (G subGraph)
        {
            SubGraph = subGraph;
        }

        public void RemoveSubGraph()
        {
            SubGraph = null;
        }
    }
}