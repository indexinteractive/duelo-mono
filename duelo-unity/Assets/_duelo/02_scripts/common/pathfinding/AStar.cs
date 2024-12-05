namespace Duelo.Common.Pathfinding
{
    using System.Collections.Generic;
    using Duelo.Gameboard;
    using Ind3x.Collection;
    using UnityEngine;

    public class AStar
    {
        #region Pathfinding
        public static Path<MapTile> FindPathToTile(MapTile root, MapTile destination)
        {
            if (root != null && destination != null)
            {
                var path = FindPathAStar(root, destination);

                // !Important!
                // This line is crucial to knowing which direction the bend arrows need to face
                path.SetNextNodes();

                return path;
            }

            return null;
        }

        /// <summary>
        /// A★ Pathfinding Algorithm
        /// https://en.wikipedia.org/wiki/A*_search_algorithm
        /// </summary>
        private static Path<MapTile> FindPathAStar(MapTile start, MapTile goal)
        {
            var closed = new HashSet<MapTile>();
            var queue = new PriorityQueue<double, Path<MapTile>>();

            queue.Enqueue(0, new Path<MapTile>(start));

            while (!queue.IsEmpty)
            {
                var path = queue.Dequeue();

                if (closed.Contains(path.Data))
                {
                    continue;
                }
                else if (path.Data == goal)
                {
                    return path;
                }
                else
                {
                    closed.Add(path.Data);
                    foreach (MapTile n in path.Data.GetNeighbors(includeUnreachable: true, includeDiagonals: false))
                    {
                        float d = Vector3.Distance(path.Data.transform.position, n.transform.position);

                        var newPath = path.AddStep(n, d);

                        var estimated = Vector3.Distance(path.Data.transform.position, goal.transform.position);
                        queue.Enqueue(newPath.TotalCost + estimated, newPath);
                    }
                }
            }

            return null;
        }
        #endregion
    }
}