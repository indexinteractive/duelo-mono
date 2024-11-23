namespace Duelo.Gameboard
{
    using System.Collections.Generic;
    using UnityEngine;

    public static class Util
    {
        public static Vector3[] CardinalDirections = new Vector3[]
        {
            Vector3.forward,
            Vector3.back,
            Vector3.left,
            Vector3.right
        };

        public static Vector3[] AllDirections = new Vector3[]
        {
            Vector3.forward,
            Vector3.back,
            Vector3.left,
            Vector3.right,
            Vector3.forward + Vector3.left,
            Vector3.forward + Vector3.right,
            Vector3.back + Vector3.left,
            Vector3.back + Vector3.right
        };
    }

    public interface ITraversable<T>
    {
        List<T> GetNeighbors(bool includeUnreachable, bool useAllDirections);
    }

    public class BFSNode<T>
    {
        public T Data { get; private set; }
        public bool visited { get; set; }
        public int distance { get; set; }
        public BFSNode<T> parent { get; set; }

        public BFSNode(T data)
        {
            Data = data;
        }
    }

    public class BFSHelper<T> where T : ITraversable<T>
    {
        private static Dictionary<T, List<BFSNode<T>>> _neighborCache = new Dictionary<T, List<BFSNode<T>>>();

        /// <param name="root">Starting node</param>
        /// <param name="limit">Max distance that can be traversed</param>
        public static List<T> FindNodes(T root, int limit)
        {
            var nodes = new List<T>();
            var bfsRoot = new BFSNode<T>(root);

            var Q = new Queue<BFSNode<T>>();
            bfsRoot.visited = true;

            Q.Enqueue(bfsRoot);

            while (Q.Count > 0)
            {
                var v = Q.Dequeue();
                nodes.Add(v.Data);

                if (v.distance < limit)
                {
                    var neighbors = GetNeighbors(v.Data);
                    foreach (var edge in neighbors)
                    {
                        if (!edge.visited)
                        {
                            edge.parent = v;
                            edge.visited = true;
                            edge.distance = 1 + v.distance;
                            Q.Enqueue(edge);
                        }
                    }
                }
            }

            // TODO: Important!
            //
            // It is unclear at this point if the _neighborCache should be done away with altogether
            // or if it is better to keep this call:
            //
            // - Option A: no cache, spend time performing raycasts on every node
            // - Option B: use the node cache, but iterate through the nodes multiple times
            //
            // A performance analysis should be done to find a good answer
            ResetCacheNodes();

            return nodes;
        }

        private static void ResetCacheNodes()
        {
            foreach (var list in _neighborCache)
            {
                foreach (var node in list.Value)
                {
                    node.visited = false;
                    node.distance = 0;
                    node.parent = null;
                }
            }
        }

        private static List<BFSNode<T>> GetNeighbors(T root)
        {
            if (_neighborCache.ContainsKey(root))
            {
                return _neighborCache[root];
            }
            else
            {
                var neighbors = new List<BFSNode<T>>();
                var items = root.GetNeighbors(includeUnreachable: true, useAllDirections: false);
                foreach (var item in items)
                {
                    neighbors.Add(new BFSNode<T>(item));
                }

                _neighborCache.Add(root, neighbors);
                return neighbors;
            }
        }
    }
}