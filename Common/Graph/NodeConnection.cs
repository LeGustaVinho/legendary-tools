﻿using System;

namespace LegendaryTools.Graph
{
    public enum NodeConnectionDirection
    {
        Foward, //Graph can only move From -> To
        Backward, //Graph can only move From <- To
        Both //Graph can move From <-> To
    }

    public enum NodeConnectionType
    {
        Common, //Default connect between two nodes like a graph
        Tree, //Connection between two nodes like a tree, so From is Parent and To is Child
    }

    public class NodeConnection
    {
        /// <summary>
        /// NodeConnection unique identifier
        /// </summary>
        public Guid ID;

        public Node From; //Parent if graph is tree

        public Node To; //Child if graph is tree

        public NodeConnectionType Type = NodeConnectionType.Common;

        public NodeConnectionDirection Direction = NodeConnectionDirection.Both;

        public float Weight;

        public NodeConnection(Node from, Node to, NodeConnectionType type = NodeConnectionType.Common, NodeConnectionDirection direction = NodeConnectionDirection.Both, float weight = 0)
        {
            ID = Guid.NewGuid();
            From = from;
            To = to;
            Type = type;
            Direction = direction;
            Weight = weight;

            from.Owner.internal_AddConnections(this);
        }

        public void Destroy()
        {
            From.Owner.internal_RemoveConnections(this);

            From.internal_removeConnection(this);
            To.internal_removeConnection(this);
        }
    }
}