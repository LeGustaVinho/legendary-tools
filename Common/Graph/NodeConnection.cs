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
        where G : LinkedGraph<G, N, NC, C>
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
        
        public NodeConnection(N @from, N to, NodeConnectionDirection direction = NodeConnectionDirection.Both, float weight = 0 ) : this()
        {
            From = @from;
            To = to;
            Direction = direction;
            Weight = weight;
        }
        
        public NodeConnection(N @from, N to, C context, NodeConnectionDirection direction = NodeConnectionDirection.Both, float weight = 0 ) : this(@from, to, direction, weight)
        {
            Context = context;
        }
    }
}