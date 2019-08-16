using System;
using System.Collections;
using System.Collections.Generic;
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

        public N DepthSearch(Predicate<N> match)
        {
            return null;
        }
        
        public N WidthSearch(Predicate<N> match)
        {
            return null;
        }

        public IEnumerator<N> GetDepthEnumerator()
        {
            throw new NotImplementedException();
        }
        
        public IEnumerator<N> GetWidthEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}