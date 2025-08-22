using System.Collections.Generic;
using UnityEngine;

//something does not work as intended

namespace NeolithianRev.Utility
{
    public static class MovementAlgos
    {
        // Returns all reachable positions from startPos within movePoints, considering movement costs per tile
        public static HashSet<Vector2Int> GetReachableTiles(
            TileType[][] map,
            Vector2Int startPos,
            int movePoints,
            Dictionary<TileType, int> moveCosts)
        {
            int width = map.Length;
            var visited = new Dictionary<Vector2Int, int>(); // Position -> remaining movePoints
            var queue = new Queue<(Vector2Int pos, int remaining)>();
            queue.Enqueue((startPos, movePoints));
            visited[startPos] = movePoints;

            // Flat-topped hex grid directions (even-q offset)
            Vector2Int[] evenDirections = new Vector2Int[]
            {
                new Vector2Int(+1, 0), new Vector2Int(-1, 0),
                new Vector2Int(0, +1), new Vector2Int(0, -1),
                new Vector2Int(-1, +1), new Vector2Int(-1, -1)
            };
            Vector2Int[] oddDirections = new Vector2Int[]
            {
                new Vector2Int(+1, 0), new Vector2Int(-1, 0),
                new Vector2Int(0, +1), new Vector2Int(0, -1),
                new Vector2Int(+1, +1), new Vector2Int(+1, -1)
            };

            while (queue.Count > 0)
            {
                var (current, remaining) = queue.Dequeue();
                var directions = (current.y % 2 == 0) ? evenDirections : oddDirections;
                foreach (var dir in directions)
                {
                    int nx = current.x + dir.x;
                    int ny = current.y + dir.y;
                    if (nx < 0 || nx >= width)
                        continue;
                    if (ny < 0 || ny >= map[nx].Length)
                        continue;
                    var next = new Vector2Int(nx, ny);
                    TileType tile = map[nx][ny];
                    if (!moveCosts.TryGetValue(tile, out int cost))
                        continue; // Impassable or undefined
                    int newRemaining = remaining - cost;
                    if (newRemaining < 0)
                        continue;
                    if (visited.TryGetValue(next, out int prevRemaining) && prevRemaining >= newRemaining)
                        continue; // Already visited with more or equal points
                    visited[next] = newRemaining;
                    queue.Enqueue((next, newRemaining));
                }
            }
            visited.Remove(startPos); // Optionally exclude the start tile
            return new HashSet<Vector2Int>(visited.Keys);
        }

        // Returns the shortest path from startPos to endPos within movePoints, or null if unreachable
        public static List<Vector2Int> GetPath(
            TileType[][] map,
            Vector2Int startPos,
            Vector2Int endPos,
            int movePoints,
            Dictionary<TileType, int> moveCosts)
        {
            int width = map.Length;
            var visited = new Dictionary<Vector2Int, int>(); // Position -> remaining movePoints
            var parent = new Dictionary<Vector2Int, Vector2Int>();
            var queue = new Queue<(Vector2Int pos, int remaining)>();
            queue.Enqueue((startPos, movePoints));
            visited[startPos] = movePoints;

            // Flat-topped hex grid directions (even-q offset)
            Vector2Int[] evenDirections = new Vector2Int[]
            {
                new Vector2Int(+1, 0), new Vector2Int(-1, 0),
                new Vector2Int(0, +1), new Vector2Int(0, -1),
                new Vector2Int(-1, +1), new Vector2Int(-1, -1)
            };
            Vector2Int[] oddDirections = new Vector2Int[]
            {
                new Vector2Int(+1, 0), new Vector2Int(-1, 0),
                new Vector2Int(0, +1), new Vector2Int(0, -1),
                new Vector2Int(+1, +1), new Vector2Int(+1, -1)
            };

            while (queue.Count > 0)
            {
                var (current, remaining) = queue.Dequeue();
                if (current == endPos)
                {
                    // Reconstruct path
                    var path = new List<Vector2Int>();
                    var step = endPos;
                    while (step != startPos)
                    {
                        path.Add(step);
                        step = parent[step];
                    }
                    path.Add(startPos);
                    path.Reverse();
                    return path;
                }
                var directions = (current.y % 2 == 0) ? evenDirections : oddDirections;
                foreach (var dir in directions)
                {
                    int nx = current.x + dir.x;
                    int ny = current.y + dir.y;
                    if (nx < 0 || nx >= width)
                        continue;
                    if (ny < 0 || ny >= map[nx].Length)
                        continue;
                    var next = new Vector2Int(nx, ny);
                    TileType tile = map[nx][ny];
                    if (!moveCosts.TryGetValue(tile, out int cost))
                        continue;
                    int newRemaining = remaining - cost;
                    if (newRemaining < 0)
                        continue;
                    if (visited.TryGetValue(next, out int prevRemaining) && prevRemaining >= newRemaining)
                        continue;
                    visited[next] = newRemaining;
                    parent[next] = current;
                    queue.Enqueue((next, newRemaining));
                }
            }
            return null; // No path found
        }
    }
}