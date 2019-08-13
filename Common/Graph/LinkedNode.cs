using System.Collections.Generic;
using UnityEngine;

namespace LegendaryTools.Graph
{
    public interface INode<N>
    {
        N[] Neighbours { get; }

        int Count { get; }
    }

    public abstract class LinkedNode<G, N, NC, C> : HierarchicalNode<G, N>, INode<N>
        where G : Graph<G, N, NC, C>
        where N : LinkedNode<G, N, NC, C>
        where NC : NodeConnection<G, N, NC, C>
    {
        public N[] Neighbours { get; }
        public int Count
        {
            get { return Connections.Count; }
        }

        public readonly List<NC> Connections = new List<NC>();

        public NC ConnectTo(N to, float weight, C context)
        {
            if (to == null)
            {
                Debug.LogError("[LinkedNode:ConnectTo()] -> Target node cannot be null.");
                return null;
            }

            if (to == this)
            {
                Debug.LogError("[LinkedNode:ConnectTo()] -> You cant connect to yourself.");
                return null;
            }
            
            NC newConnection = Owner.CreateConnection(this, to, weight, context);
            Connections.Add(newConnection);
            to.Connections.Add(newConnection);
            return newConnection;
        }

        public bool Disconnect(N node)
        {
            NC nodeCon = GetConnection(node);
            return Connections.Remove(nodeCon) && node.Connections.Remove(nodeCon);
        }

        public NC GetConnection(N node)
        {
            return Connections.Find(item => item.From == node || item.To == node);
        }
    }
}