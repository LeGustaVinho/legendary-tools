﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LegendaryTools.Graph
{
    public class Branch<G, N> : HierarchicalNode<G, N>, INode<N>, ICollection<N>
        where G : Tree<G, N>
        where N : Branch<G, N>
    {
        public virtual N[] Neighbours
        {
            get
            {
                List<N> neighbours = new List<N>();
                neighbours.AddRange(childs);
                neighbours.Add(Parent);
                return neighbours.ToArray();
            }
        }

        public int Count => childs.Count;

        public bool IsReadOnly { get; }
        public N[] BranchHierachy { get; }

        public N Parent { get; protected set; }
        
        public List<N> TreeParentHierarchy
        {
            get
            {
                List<N> path = new List<N>();
                for(N parent = Parent; parent != null; parent = parent.Parent)
                {
                    if(parent != null)
                        path.Add(parent);
                }
                path.Reverse();
                return path;
            }
        }
        
        private readonly List<N> childs = new List<N>();

        public void Add(N newBranch)
        {
            if (!childs.Contains(newBranch))
            {
                childs.Add(newBranch);
                newBranch.Parent = this as N;
            }
            else
            {
                Debug.LogError("[Branch:Add()] -> Already contains this branch.");
            }
        }

        public void Clear()
        {
            childs.Clear();
        }

        public void CopyTo(N[] array, int arrayIndex)
        {
            childs.CopyTo(array, arrayIndex);
        }

        public bool Remove(N newBranch)
        {
            return childs.Remove(newBranch);
        }
        
        public bool Contains(N newBranch)
        {
            return childs.Contains(newBranch);
        }

        public void SetParent(N newParent)
        {
            Parent = newParent;
        }

        public IEnumerator<N> GetEnumerator()
        {
            return childs.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}