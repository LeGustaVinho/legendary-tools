using System;
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
        where G : LinkedGraph<G, N, NC, C>
        where N : LinkedNode<G, N, NC, C>
        where NC : NodeConnection<G, N, NC, C>
    {
        public virtual N[] Neighbours
        {
            get
            {
                List<N> neighbours = new List<N>();

                for (int i = 0; i < Connections.Count; i++)
                {
                    neighbours.Add(Connections[i].To == this ? Connections[i].From : Connections[i].To);
                }
                
                return neighbours.ToArray();
            }
        }
        public int Count => Connections.Count;

        protected readonly List<NC> Connections = new List<NC>();
        
        protected LinkedNode(G owner) : base(owner)
        {
        }
        
        public NC ConnectTo(N to, C context, NodeConnectionDirection direction = NodeConnectionDirection.Both, float weight = 0)
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
            
            NC newConnection = Owner.CreateConnection(this as N, to, context, direction, weight);
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
        
        public NC FindConnection(Predicate<NC> predicate)
        {
            return Connections.Find(predicate);
        }
    }
}