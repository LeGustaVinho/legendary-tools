﻿using System;
using System.Collections;
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
    
    public abstract class LinkedGraph<G, N, NC, C> : HierarchicalGraph<G, N>, IGraph<N>, ICollection<N>
        where G : LinkedGraph<G, N, NC, C>
        where N : LinkedNode<G, N, NC, C>
        where NC : NodeConnection<G, N, NC, C>
    {
        public int Count => allNodes.Count;
        public bool IsReadOnly => false;

        protected readonly List<N> allNodes = new List<N>();
        
        public abstract NC CreateConnection(N @from, N to, C context, NodeConnectionDirection direction = NodeConnectionDirection.Both, float weight = 0);
        
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

        public void Clear()
        {
            allNodes.Clear();
        }

        public void CopyTo(N[] array, int arrayIndex)
        {
            allNodes.CopyTo(array, arrayIndex);
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
            return node.Neighbours;
        }

        public IEnumerator<N> GetEnumerator()
        {
            return allNodes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}