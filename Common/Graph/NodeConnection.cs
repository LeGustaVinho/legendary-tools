using System;

namespace LegendaryTools.Graph
{
    public enum NodeConnectionDirection
    {
        Foward, //Graph can only move From -> To
        Backward, //Graph can only move From <- To
        Both //Graph can move From <-> To
    }
    
    public class NodeConnection<G, N, NC, C>
        where G : Graph<G, N, NC, C>
        where N : LinkedNode<G, N, NC, C>
        where NC : NodeConnection<G, N, NC, C>
    {
        public Guid ID { get; protected set; }
        public N From { get; protected set; }
        public N To { get; protected set; }
        public NodeConnectionDirection Direction = NodeConnectionDirection.Both;
        public float Weight;
        public C Context;
        
        public NodeConnection()
        {
            ID = Guid.NewGuid();
        }
        
        public NodeConnection(N @from, N to, float weight, C context) : this()
        {
            From = @from;
            To = to;
            Weight = weight;
            Context = context;
        }
    }
}