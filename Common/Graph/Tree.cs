using System;
using UnityEngine;

namespace LegendaryTools.Graph
{
    public class Tree<G, N> : HierarchicalGraph<G, N>
        where G : Tree<G, N>
        where N : Branch<G, N>
    {
        public void AddToRoot(N newNode)
        {
            if (StartOrRootNode == null)
            {
                StartOrRootNode = newNode;
                newNode.SetParent(null);
            }
            else
            {
                Debug.LogError("[Tree:AddToRoot()] -> Is already busy.");
            }
        }
    }
}