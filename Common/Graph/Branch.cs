using System.Collections.Generic;
using UnityEngine;

namespace LegendaryTools.Graph
{
    public class Branch<G, N> : HierarchicalNode<G, N>, INode<N>
        where G : Tree<G, N>
        where N : Branch<G, N>
    {
        public N[] Neighbours { get; }

        public int Count
        {
            get { return childs.Count; }
        }
        public N[] BranchHierachy { get; }

        public Branch<G, N> Parent { get; protected set; }
        
        public List<Branch<G, N>> TreeParentHierarchy
        {
            get
            {
                List<Branch<G, N>> path = new List<Branch<G, N>>();
                for(Branch<G, N> parent = Parent; parent != null; parent = parent.Parent)
                {
                    if(parent != null)
                        path.Add(parent);
                }
                path.Reverse();
                return path;
            }
        }
        
        private readonly HashSet<N> childs = new HashSet<N>();

        public void AddChild(N newBranch)
        {
            if (!childs.Contains(newBranch))
            {
                childs.Add(newBranch);
                newBranch.Parent = this;
            }
            else
            {
                Debug.LogError("[Branch:Add()] -> Already contains this branch.");
            }
        }

        public bool RemoveChild(N newBranch)
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
    }
}