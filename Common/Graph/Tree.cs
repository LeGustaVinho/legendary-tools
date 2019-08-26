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
        public Tree(N newNode)
        {
            newNode.Owner = this as G;
            StartOrRootNode = newNode;
            newNode.SetParent(null);
        }
        
        public Tree(N newNode)
        {
            newNode.Owner = this as G;
            StartOrRootNode = newNode;
            newNode.SetParent(null);
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