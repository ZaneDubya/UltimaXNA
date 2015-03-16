using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InterXLib.Algorithms
{
    public class AStar
    {
        public static IEnumerable<IAStarNode> GeneratePath(IAStarNode start, IAStarNode end)
        {
            PriorityQueue<AStarPQNode> nodeQueue = new PriorityQueue<AStarPQNode>(16);
            nodeQueue.Insert(new AStarPQNode { Node = start });
            Dictionary<IAStarNode, AStarPQNode> processedNodes = new Dictionary<IAStarNode, AStarPQNode>();

            AStarPQNode current = null;

            while (!nodeQueue.IsEmpty())
            {
                current = nodeQueue.DelMin();
                if (current.Node.Equals(end)) break;

                foreach (var neighbor in current.Node.GetNeighbors())
                {
                    // convert the IAStarNode to an AStarPQNode
                    var neighborNode = new AStarPQNode
                    {
                        Node = neighbor,
                        Previous = current,
                        Remaining = neighbor.EstimateDistance(end),
                        Traveled = current.Traveled + current.Node.Distance(neighbor)
                    };

                    // if we already know a faster way to this node, continue
                    if (processedNodes.ContainsKey(neighbor) &&
                        processedNodes[neighbor].Traveled < neighborNode.Traveled)
                        continue;

                    // if we've discovered this node before, we now have a faster way, so update it
                    if (processedNodes.ContainsKey(neighbor))
                    {
                        processedNodes[neighbor].Traveled = neighborNode.Traveled;
                        processedNodes[neighbor].Previous = neighborNode.Previous;
                        continue;
                    }

                    // store this node as having been processed
                    processedNodes.Add(neighbor, neighborNode);

                    // add this node to the queue to process
                    nodeQueue.Insert(neighborNode);
                }

                current = null;
            }

            if (current == null) throw new ArgumentException("no path from start to end");

            var path = new Stack<IAStarNode>();
            var prev = current.Previous;
            while (prev != null)
            {
                path.Push(prev.Node);
                prev = prev.Previous;
            }
            return path;
        }

        private class AStarPQNode : IComparable<AStarPQNode>
        {
            public IAStarNode Node;
            public double Remaining;
            public double Traveled;
            public AStarPQNode Previous;

            public int CompareTo(AStarPQNode other)
            {
                var delta = (Traveled + Remaining) - (other.Traveled + other.Remaining);

                if (delta > 0) return 1;
                if (delta < 0) return -1;
                return 0;

            }
        }
    }

    public interface IAStarNode
    {
        List<IAStarNode> GetNeighbors();
        double EstimateDistance(IAStarNode end);
        double Distance(IAStarNode destination);
    }
}
