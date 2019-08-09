using System.Collections.Generic;
using UnityEngine;
using LegendaryTools;
using System;

namespace LegendaryTools
{
    public class Node
    {
        /// <summary>
        /// Node unique identifier
        /// </summary>
        public Guid ID; 

        private NodeGraph owner;
        /// <summary>
        /// Owner of this node, AKA which graph/tree controls this node
        /// </summary>
        public NodeGraph Owner
        {
            get { return owner; }
        }

        /// <summary>
        /// It is true if this node is the root of the tree or is marked as the start node of the graph
        /// </summary>
        public bool IsRootOrStartNode
        {
            get { return owner.StartOrRootNode == this; }
        }

        public NodeGraph SubGraph;

        public List<NodeConnection> Connections = new List<NodeConnection>();

        public List<Node> Neighbors
        {
            get
            {
                List<Node> neighbors = new List<Node>();
                for(int i = 0; i < Connections.Count; i++)
                    neighbors.Add(Connections[i].From != this ? Connections[i].From : Connections[i].To);
                return neighbors;
            }
        }

        public List<NodeGraph> GraphHierarchy
        {
            get
            {
                List<NodeGraph> path = new List<NodeGraph>();
                if(Owner.ParentNode != null)
                {
                    for (Node parentNode = Owner.ParentNode; parentNode != null; parentNode = Owner.ParentNode.Owner.ParentNode)
                    {
                        if(parentNode.Owner != null)
                            path.Add(parentNode.Owner);
                    }
                }
                path.Reverse();
                return path;
            }
        }

        public List<Node> NodeHierarchy
        {
            get
            {
                List<Node> path = new List<Node>();
                if (Owner.ParentNode != null)
                {
                    for (Node parentNode = Owner.ParentNode; parentNode != null; parentNode = Owner.ParentNode.Owner.ParentNode)
                    {
                        if (parentNode.Owner != null)
                            path.Add(parentNode);
                    }
                }
                path.Reverse();
                return path;
            }
        }

        public NodeConnection ParentConnection
        {
            get { return Connections.Find(item => item.From != this && item.Type == NodeConnectionType.Tree); }
        }

        public List<NodeConnection> ParentConnections
        {
            get { return Connections.FindAll(item => item.From != this && item.Type == NodeConnectionType.Tree); }
        }

        public List<NodeConnection> ChildConnections
        {
            get { return Connections.FindAll(item => item.To != this && item.Type == NodeConnectionType.Tree); }
        }

        public List<Node> TreeParentHierarchy
        {
            get
            {
                List<Node> path = new List<Node>();
                for(NodeConnection parent = ParentConnection; parent != null; parent = parent.From.ParentConnection)
                {
                    if(parent != null)
                        path.Add(parent.From);
                }
                path.Reverse();
                return path;
            }
        }

        public bool IsLeaf
        {
            get { return Connections.FindAll(item => item.From == this).Count == 0; }
        }

        public int ConnectionCount
        {
            get { return Connections.Count; }
        }

        public bool HasSubGraph
        {
            get { return SubGraph != null; }
        }

        public Node(NodeGraph owner)
        {
            ID = Guid.NewGuid();
            this.owner = owner;

            owner.internal_AddNode(this);
        }

        /// <summary>
        /// Connect two nodes together like a graph
        /// </summary>
        /// <param name="node"></param>
        public NodeConnection CreateConnectionTo(Node node, NodeConnectionType type = NodeConnectionType.Common, NodeConnectionDirection direction = NodeConnectionDirection.Both, float weight = 0)
        {
            if (node != null)
            {
                //check if connection already exits
                NodeConnection checkConnection = GetConnection(node);
                if (checkConnection != null)
                {
                    Debug.LogError("[Node:ConnectTo()] -> Connection already exists.");
                    return null;
                }

                NodeConnection connection = new NodeConnection(this, node, type, direction, weight);

                internal_addConnection(connection);
                node.internal_addConnection(connection);

                return connection;
            }
            else
            {
                Debug.LogError("[Node:ConnectTo()] -> Node cannot be null.");
                return null;
            }
        }

        public bool Disconnect(Node node)
        {
            NodeConnection connection = GetConnection(node);
            if (connection != null)
            {
                connection.Destroy();
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Create a node and connect to it like a tree
        /// </summary>
        public Node CreateNodeChild(NodeConnectionDirection direction = NodeConnectionDirection.Both, float weight = 0)
        {
            Node newNode = Owner.CreateNode();
            this.CreateConnectionTo(newNode, NodeConnectionType.Tree, direction, weight);
            return newNode;
        }

        /// <summary>
        /// Create a node and connect to it like a graph
        /// </summary>
        public Node CreateNode(NodeConnectionDirection direction = NodeConnectionDirection.Both, float weight = 0)
        {
            Node newNode = Owner.CreateNode();
            this.CreateConnectionTo(newNode, NodeConnectionType.Common, direction, weight);
            return newNode;
        }

        /// <summary>
        /// Destroy the node and your connections and subgraph
        /// </summary>
        /// <param name="destroyChildsRecursive">If true will destroy all Nodes and Subgraphs like a tree</param>
        public void Destroy(bool destroyChildsRecursive = false)
        {
            //Destroy subgraph
            if (SubGraph != null)
                SubGraph.Destroy();

            SubGraph = null;

            //Destroy child nodes like a tree
            if (destroyChildsRecursive)
            {
                List<NodeConnection> childs = ChildConnections;
                for (int i = 0; i < childs.Count; i++)
                    childs[i].To.Destroy(destroyChildsRecursive);
            }

            //Destroy all connections
            for (int i = 0; i < Connections.Count; i++)
                Connections[i].Destroy();

            Connections.Clear();

            owner.internal_RemoveNode(this);
        }

        public NodeGraph CreateSubGraph()
        {
            if (SubGraph == null)
                SubGraph = new NodeGraph(this);
            else
                Debug.LogWarning("[Node:AddSubGraph() -> A subgraph already exists here, please destroy it first.");

            return SubGraph;
        }
        
        public void AddSubGraph(NodeGraph nodeGraph)
        {
            if (SubGraph == null)
                SubGraph = nodeGraph;
            else
                Debug.LogWarning("[Node:AddSubGraph() -> A subgraph already exists here, please destroy it first.");
        }

        public void RemoveSubGraph()
        {
            if (SubGraph != null)
                SubGraph.Destroy();

            SubGraph = null;
        }

        public void SetAsStartOrRootNode()
        {
            if (Owner != null)
                Owner.StartOrRootNode = this;
        }

        public Node Other(NodeConnection nodeConnection)
        {
            return nodeConnection.From == this ? nodeConnection.To : nodeConnection.From;
        }

        public NodeConnection GetConnection(Node node)
        {
            return Connections.Find(item => item.From == node || item.To == node);
        }

        public bool IsConnected(Node node)
        {
            return GetConnection(node) != null;
        }

        internal void internal_addConnection(NodeConnection nodeConnection)
        {
            Connections.Add(nodeConnection);
            OnConnectionAdd(nodeConnection);
        }

        internal bool internal_removeConnection(NodeConnection nodeConnection)
        {
            OnConnectionRemove(nodeConnection);
            return Connections.Remove(nodeConnection);
        }

        protected virtual void OnConnectionAdd(NodeConnection nodeConnection)
        {

        }

        protected virtual void OnConnectionRemove(NodeConnection nodeConnection)
        {

        }
    }
}