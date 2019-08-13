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

        public G[] GraphHierachy
        {
            get
            {
                List<G> path = new List<G>();
                if (ParentNode != null)
                {
                    for (N parentNode = ParentNode; parentNode != null; parentNode = ParentNode.Owner.ParentNode)
                    {
                        if (parentNode.Owner != null)
                            path.Add(parentNode.Owner);
                    }
                }

                path.Reverse();
                return path.ToArray();
            }
        }
        
        public N StartOrRootNode { get; set; }

        public HierarchicalGraph()
        {
            ID = Guid.NewGuid();
        }
    }
}