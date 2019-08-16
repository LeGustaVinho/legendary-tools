using System.Collections.Generic;

namespace LegendaryTools.Pathfinding
{
    public interface IAStar<T>
    {
        T[] Neighbors(T node);

        float Heuristic(T nodeA, T nodeB);
    }

    public class AStar<T>
    {
        private class AStarNode : IPriorityQueueNode
        {
            public float Priority
            {
                get => Score;
                set => Score = value;
            }

            public readonly T Location;

            public float Score;

            public AStarNode(T location)
            {
                Location = location;
            }

            public override int GetHashCode()
            {
                return Location.GetHashCode();
            }

            public void Clean()
            {
                Score = 0;
            }
        }

        private static readonly Dictionary<T, AStarNode> cachedNodes = new Dictionary<T, AStarNode>();
        private readonly IAStar<T> map;

        public AStar(IAStar<T> map)
        {
            this.map = map;
        }

        public T[] FindPath(T startLocation, T endLocation)
        {
            Dictionary<AStarNode, AStarNode> cameFrom = new Dictionary<AStarNode, AStarNode>();
            Dictionary<AStarNode, float> costSoFar = new Dictionary<AStarNode, float>();
            PriorityQueue<AStarNode> open = new PriorityQueue<AStarNode>();

            cleanNodesData();

            AStarNode startNode = getFromCache(startLocation);
            AStarNode endNode = getFromCache(endLocation);

            open.Enqueue(startNode);
            cameFrom[startNode] = startNode;
            costSoFar[startNode] = 0;

            AStarNode currentNode;
            while (open.Count > 0)
            {
                currentNode = open.Dequeue();

                if (currentNode.Location.Equals(endLocation))
                {
                    List<T> path = new List<T>();
                    currentNode = endNode;

                    while (currentNode != startNode)
                    {
                        path.Add(currentNode.Location);
                        currentNode = cameFrom[currentNode];
                    }

                    path.Reverse();
                    return path.ToArray();
                }

                T[] neighbours = map.Neighbors(currentNode.Location);
                AStarNode currentNeighborsNode = null;
                for (int i = 0; i < neighbours.Length; i++)
                {
                    currentNeighborsNode = getFromCache(neighbours[i]);
                    float newCost = costSoFar[currentNode];
                    if (!costSoFar.ContainsKey(currentNeighborsNode) || newCost < costSoFar[currentNeighborsNode])
                    {
                        costSoFar[currentNeighborsNode] = newCost;
                        currentNeighborsNode.Score =
                            newCost + map.Heuristic(currentNeighborsNode.Location, endLocation);
                        open.Enqueue(currentNeighborsNode);
                        cameFrom[currentNeighborsNode] = currentNode;
                    }
                }
            }

            return null;
        }

        private static void cleanNodesData()
        {
            foreach (KeyValuePair<T, AStarNode> pair in cachedNodes)
            {
                pair.Value.Clean();
            }
        }

        private static AStarNode getFromCache(T node)
        {
            if (!cachedNodes.ContainsKey(node))
                cachedNodes.Add(node, new AStarNode(node));

            return cachedNodes[node];
        }
    }
}
