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
        public G SubGraph { get; protected internal set; }

        public G Owner { get; protected internal set; }

        public bool HasSubGraph => SubGraph != null;
        
        public N[] NodeHierarchy
        {
            get
            {
                List<N> path = new List<N>();

                for (N parentNode = Owner.ParentNode; parentNode != null; parentNode = parentNode.Owner?.ParentNode)
                {
                    if (parentNode != null)
                        path.Add(parentNode);
                }

                path.Reverse();
                return path.ToArray();
            }
        }

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
            SubGraph.ParentNode = this as N;
        }

        public void RemoveSubGraph()
        {
            if (SubGraph == null) return;
            
            SubGraph.ParentNode = null;
            SubGraph = null;
        }

        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }
        
        public override bool Equals(object other)
        {
            if (!(other.GetType().IsSameOrSubclass(typeof(HierarchicalNode<G, N>))))
            {
                return false;
            }

            HierarchicalNode<G, N> node = (HierarchicalNode<G, N>) other;
            return ID == node.ID;
        }
    }
}