using System;
using System.Collections.Generic;
using UnityEngine;

namespace LegendaryTools.Graph
{
    public interface IGraph<N>
    {
        N StartOrRootNode { get; set; }

        void Add(N newNode);

        bool Remove(N node);

        bool Contains(N node);

        N[] Neighbours(N node);
    }
    
    public abstract class Graph<G, N, NC, C> : HierarchicalGraph<G, N>, IGraph<N>
        where G : Graph<G, N, NC, C>
        where N : LinkedNode<G, N, NC, C>
        where NC : NodeConnection<G, N, NC, C>
    {
        protected readonly HashSet<N> allNodes = new HashSet<N>();
        
        public abstract NC CreateConnection(LinkedNode<G, N, NC, C> @from, N to, float weight, C context);
        
        public void Add(N newNode)
        {
            if (!allNodes.Contains(newNode))
            {
                allNodes.Add(newNode);

                if (StartOrRootNode == null)
                    StartOrRootNode = newNode;
            }
            else
            {
                Debug.LogError("[HierarchicalGraph:Add()] -> Already contains this node.");
            }
        }

        public bool Remove(N node)
        {
            return allNodes.Remove(node);
        }

        public bool Contains(N node)
        {
            return allNodes.Contains(node);
        }

        public N[] Neighbours(N node)
        {
            return null;
        }
    }
}