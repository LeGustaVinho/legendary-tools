using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace LegendaryTools.Graph
{
 public abstract class HierarchicalGraph<G, N>
    where G : HierarchicalGraph<G, N>
    where N : HierarchicalNode<G, N>
    {
        public Guid ID { get; protected set; }
        public N ParentNode { get; protected set; }

        public G[] GraphHierarchy
        {
            get
            {
                List<G> path = new List<G>();

                for (N parentNode = ParentNode; parentNode != null; parentNode = parentNode.Owner?.ParentNode)
                {
                    if (parentNode.Owner != null)
                        path.Add(parentNode.Owner);
                }
                
                path.Reverse();
                return path.ToArray();
            }
        }

        protected N startOrRootNode;
        public N StartOrRootNode
        {
            get => startOrRootNode;
            set
            {
                if(value == null)
                    Debug.LogError("[HierarchicalGraph:StartOrRootNode] -> StartOrRootNode cannot be null.");
                else
                    startOrRootNode = value;
            }
        }

        public HierarchicalGraph()
        {
            ID = Guid.NewGuid();
        }
        
        public HierarchicalGraph(N parentNode) : this()
        {
            ParentNode = parentNode;
        }
        
        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }
        
        public override bool Equals(object other)
        {
            if (!(other.GetType().IsSameOrSubclass(typeof(HierarchicalGraph<G, N>))))
            {
                return false;
            }

            HierarchicalGraph<G, N> node = (HierarchicalGraph<G, N>) other;
            return ID == node.ID;
        }
    }
}