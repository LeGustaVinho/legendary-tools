using System.Collections.Generic;
using System;

namespace LegendaryTools.Graph
{
    public class NodeGraph
    {
        /// <summary>
        /// NodeGraph unique identifier
        /// </summary>
        public Guid ID;

        /// <summary>
        /// points to who is the root of the tree or the first node to be created
        /// </summary>
        public Node StartOrRootNode;

        /// <summary>
        /// Points to the node in case the graph is contained inside the node
        /// </summary>
        public Node ParentNode;

        /// <summary>
        /// Enumerates all nodes that this graph / tree has
        /// </summary>
        public List<Node> AllNodes = new List<Node>();

        /// <summary>
        /// List all connections that this graph / tree has
        /// </summary>
        public List<NodeConnection> AllConnections = new List<NodeConnection>();

        /// <summary>
        /// Returns true if all connections in the graph are treated as a tree
        /// </summary>
        public bool IsTree
        {
            get { return AllConnections.TrueForAll(item => item.Type == NodeConnectionType.Tree); }
        }

        public bool IsSubGraph
        {
            get { return ParentNode != null; }
        }

        public List<NodeGraph> GraphHierarchy
        {
            get
            {
                List<NodeGraph> path = new List<NodeGraph>();
                if (ParentNode != null)
                {
                    for (Node parentNode = ParentNode; parentNode != null; parentNode = ParentNode.Owner.ParentNode)
                    {
                        if (parentNode.Owner != null)
                            path.Add(parentNode.Owner);
                    }
                }
                path.Reverse();
                return path;
            }
        }

        public NodeGraph(Node parentNode = null)
        {
            ID = Guid.NewGuid();
            ParentNode = parentNode;
        }

        /// <summary>
        /// Adds a node in the graph
        /// </summary>
        /// <returns>Node created</returns>
        public Node AddNode()
        {
            Node newNode = new Node(this);

            return newNode;
        }

        /// <summary>
        /// Destroy all nodes and their connectionse and reset graph to clear state
        /// </summary>
        public void Destroy()
        {
            for (int i = 0; i < AllConnections.Count; i++)
                AllConnections[i].Destroy();

            for (int i = 0; i < AllNodes.Count; i++)
                AllNodes[i].Destroy();

            StartOrRootNode = ParentNode = null;
            AllConnections.Clear();
            AllNodes.Clear();
        }

        internal void internal_AddNode(Node newNode)
        {
            AllNodes.Add(newNode);

            if (AllNodes.Count == 1)
                StartOrRootNode = AllNodes.First();
        }

        internal void internal_RemoveNode(Node node)
        {
            if (StartOrRootNode == node)
                StartOrRootNode = null;

            if (ParentNode == node)
                ParentNode = null;

            AllNodes.Remove(node);
        }

        internal void internal_AddConnections(NodeConnection newConnection)
        {
            AllConnections.Add(newConnection);
        }

        internal void internal_RemoveConnections(NodeConnection connection)
        {
            AllConnections.Remove(connection);
        }
    }
}