using System.Collections.Generic;
using UnityEngine;
using System;

//something does not work as intended

namespace NeolithianRev.Utility
{
    public static class MovementAlgos
    {
        // Returns all reachable positions from startPos within movePoints, considering movement costs per tile
        public static HashSet<Vector2Int> GetReachableTiles(
            Terrain[][] map,
            Vector2Int startPos,
            int movePoints,
            Dictionary<Terrain, int> moveCosts)
        {
            int width = map.Length;
            var visited = new Dictionary<Vector2Int, int>(); // Position -> remaining movePoints
            var queue = new Queue<(Vector2Int pos, int remaining)>();
            queue.Enqueue((startPos, movePoints));
            visited[startPos] = movePoints;

            // Flat-topped hex grid directions (even-q offset)
            Vector2Int[] evenDirections = new Vector2Int[]
            {
                new Vector2Int(0, +1), new Vector2Int(0, -1),
                new Vector2Int(-1, 0), new Vector2Int(+1, 0),
                new Vector2Int(-1, -1), new Vector2Int(+1, -1)
            };
            Vector2Int[] oddDirections = new Vector2Int[]
            {
                new Vector2Int(0, +1), new Vector2Int(0, -1),
                new Vector2Int(-1, 0), new Vector2Int(+1, 0),
                new Vector2Int(-1, +1), new Vector2Int(+1, +1)
            };

            while (queue.Count > 0)
            {
                var (current, remaining) = queue.Dequeue();
                var directions = (current.x % 2 == 0) ? evenDirections : oddDirections;
                foreach (var dir in directions)
                {
                    int nx = current.x + dir.x;
                    int ny = current.y + dir.y;
                    if (nx < 0 || nx >= width)
                        continue;
                    if (ny < 0 || ny >= map[nx].Length)
                        continue;
                    var next = new Vector2Int(nx, ny);
                    Terrain tile = map[nx][ny];
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

        //Current code could return a suboptimal path if a longer sequence of cheap tiles exists versus a shorter but expensive tile.
        // Returns the shortest path from startPos to endPos within movePoints, or null if unreachable
        public static List<Vector2Int> GetPath(
            Terrain[][] map,
            Vector2Int startPos,
            Vector2Int endPos,
            int movePoints,
            Dictionary<Terrain, int> moveCosts)
        {
            int width = map.Length;
            var visited = new Dictionary<Vector2Int, int>(); // Position -> remaining movePoints
            var parent = new Dictionary<Vector2Int, Vector2Int>();
            var queue = new Queue<(Vector2Int pos, int remaining)>();
            queue.Enqueue((startPos, movePoints));
            visited[startPos] = movePoints;

            //Debug.Log($"[GetPath] Start: {startPos}, End: {endPos}, MovePoints: {movePoints}");

            // Flat-topped hex grid directions (even-q offset)
            Vector2Int[] evenDirections = new Vector2Int[]
            {
                new Vector2Int(0, +1), new Vector2Int(0, -1),
                new Vector2Int(-1, 0), new Vector2Int(+1, 0),
                new Vector2Int(-1, -1), new Vector2Int(+1, -1)
            };
            Vector2Int[] oddDirections = new Vector2Int[]
            {
                new Vector2Int(0, +1), new Vector2Int(0, -1),
                new Vector2Int(-1, 0), new Vector2Int(+1, 0),
                new Vector2Int(-1, +1), new Vector2Int(+1, +1)
            };


            while (queue.Count > 0)
            {
                var (current, remaining) = queue.Dequeue();
                //Debug.Log($"[GetPath] Exploring {current} with {remaining} points left");
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

                    //Debug.Log($"[GetPath] Path found! Length: {path.Count}, Remaining: {remaining}");
                    return path;
                }
                var directions = (current.x % 2 == 0) ? evenDirections : oddDirections;
                foreach (var dir in directions)
                {
                    int nx = current.x + dir.x;
                    int ny = current.y + dir.y;
                    if (nx < 0 || nx >= width)
                    {
                        //Debug.Log($"[GetPath] Skipping {nx},{ny} (out of bounds X)");
                        continue;
                    }
                    if (ny < 0 || ny >= map[nx].Length)
                    {
                        //Debug.Log($"[GetPath] Skipping {nx},{ny} (out of bounds Y)");
                        continue;
                    }
                    var next = new Vector2Int(nx, ny);
                    Terrain tile = map[nx][ny];
                    if (!moveCosts.TryGetValue(tile, out int cost))
                    {
                        //Debug.Log($"[GetPath] Skipping {next} (no move cost defined for {tile})");
                        continue;
                    }

                    int newRemaining = remaining - cost;
                    if (newRemaining < 0)
                    {
                        //Debug.Log($"[GetPath] Skipping {next} (not enough points: cost {cost}, had {remaining})");
                        continue;
                    }

                    if (visited.TryGetValue(next, out int prevRemaining) && prevRemaining >= newRemaining)
                    {
                        //Debug.Log($"[GetPath] Skipping {next} (already visited with {prevRemaining} >= {newRemaining})");
                        continue;
                    }

                    //Debug.Log($"[GetPath] Adding {next} to queue (cost {cost}, new remaining {newRemaining})");
                    visited[next] = newRemaining;
                    parent[next] = current;
                    queue.Enqueue((next, newRemaining));
                }
            }
            //Debug.Log("[GetPath] No path found");
            return null; // No path found
        }

        //returns the shortest path from startpos to endpos regardless of movepoints
        public static List<Vector2Int> GetPathTowardsDestination(
    Terrain[][] map,
    Vector2Int startPos,
    Vector2Int endPos, // This now acts as a *target*, not a strict destination
    int movePoints,
    Dictionary<Terrain, int> moveCosts)
        {
            int width = map.Length;
            // Visited: Position -> remaining movePoints
            var visited = new Dictionary<Vector2Int, int>();
            // Parent: Child Position -> Parent Position (for path reconstruction)
            var parent = new Dictionary<Vector2Int, Vector2Int>();
            // Queue: (Position, remaining movePoints)
            var queue = new Queue<(Vector2Int pos, int remaining)>();

            queue.Enqueue((startPos, movePoints));
            visited[startPos] = movePoints;
            parent[startPos] = startPos; // Start node has itself as parent to simplify path reconstruction later

            //Debug.Log($"[GetPathTowardsDestination] Start: {startPos}, Target: {endPos}, MovePoints: {movePoints}");

            Vector2Int[] evenDirections = new Vector2Int[]
            {
        new Vector2Int(0, +1), new Vector2Int(0, -1),
        new Vector2Int(-1, 0), new Vector2Int(+1, 0),
        new Vector2Int(-1, -1), new Vector2Int(+1, -1)
            };
            Vector2Int[] oddDirections = new Vector2Int[]
            {
        new Vector2Int(0, +1), new Vector2Int(0, -1),
        new Vector2Int(-1, 0), new Vector2Int(+1, 0),
        new Vector2Int(-1, +1), new Vector2Int(+1, +1)
            };

            Vector2Int bestReachablePos = startPos;
            float closestDistToEnd = Vector2Int.Distance(startPos, endPos); // Using Euclidean distance for simplicity

            while (queue.Count > 0)
            {
                var (current, remaining) = queue.Dequeue();
                // //Debug.Log($"[GetPathTowardsDestination] Exploring {current} with {remaining} points left");

                // --- New Logic: Track the best reachable position ---
                float distToCurrentEnd = Vector2Int.Distance(current, endPos);
                if (distToCurrentEnd < closestDistToEnd)
                {
                    closestDistToEnd = distToCurrentEnd;
                    bestReachablePos = current;
                }
                // If current == endPos, it means we reached the target within budget.
                // We still continue exploring to find if there's an even better path *to the endPos*
                // or to find other reachable hexes if the unit is allowed to stop mid-path.
                // If you only care about paths *to* the endPos and no further, you could break here
                // but for "farthest towards" we need to explore fully.

                var directions = (current.x % 2 == 0) ? evenDirections : oddDirections;
                foreach (var dir in directions)
                {
                    int nx = current.x + dir.x;
                    int ny = current.y + dir.y;

                    if (nx < 0 || nx >= width) continue; // Out of bounds X
                    if (ny < 0 || ny >= map[nx].Length) continue; // Out of bounds Y

                    var next = new Vector2Int(nx, ny);
                    Terrain tile = map[nx][ny];

                    if (!moveCosts.TryGetValue(tile, out int cost)) continue; // No move cost defined

                    int newRemaining = remaining - cost;
                    if (newRemaining < 0) continue; // Not enough points

                    // If already visited and we had more or equal points remaining, no need to revisit
                    if (visited.TryGetValue(next, out int prevRemaining) && prevRemaining >= newRemaining) continue;

                    // //Debug.Log($"[GetPathTowardsDestination] Adding {next} to queue (cost {cost}, new remaining {newRemaining})");
                    visited[next] = newRemaining;
                    parent[next] = current;
                    queue.Enqueue((next, newRemaining));
                }
            }

            // --- Path Reconstruction to the bestReachablePos ---
            if (bestReachablePos == startPos)
            {
                //Debug.Log("[GetPathTowardsDestination] No movement possible from startPos.");
                return null; // No path found, or only startPos is reachable
            }

            var path = new List<Vector2Int>();
            var step = bestReachablePos;
            // The loop condition `step != startPos` works because `parent[startPos]` was set to `startPos`.
            while (step != startPos)
            {
                path.Add(step);
                // Ensure parent key exists before accessing
                if (!parent.ContainsKey(step))
                {
                    //Debug.LogError($"[GetPathTowardsDestination] Parent for {step} not found during path reconstruction!");
                    return null; // Should not happen if algorithm worked correctly
                }
                step = parent[step];
            }
            path.Add(startPos);
            path.Reverse();

            //Debug.Log($"[GetPathTowardsDestination] Farthest reachable path found to {bestReachablePos}! Length: {path.Count}");
            return path;
        }


        //Finds the cheapest path between any two points
        public static List<(Vector2Int, int)> GetCheapestPath(Vector2Int startPos, Vector2Int endPos, GoodsType goods)
        {
            // Dijkstra over even-q offset coords (flat-top hexes)
            // gScore holds total cost from start to this node (sum of per-tile entry costs)
            var gScore = new Dictionary<Vector2Int, int> { [startPos] = 0 };
            var cameFrom = new Dictionary<Vector2Int, Vector2Int>();
            var stepCost = new Dictionary<Vector2Int, int> { [startPos] = 0 };

            // Poor man's priority queue: (node, gScore). For typical map sizes it's fine.
            var open = new List<Vector2Int> { startPos };

            // Neighbor directions (even-q)
            static IEnumerable<Vector2Int> Neighbors(Vector2Int p)
            {
                // x is the column (q), y is the row (r). Matches the other functions.
                if ((p.x & 1) == 0)
                {
                    yield return new Vector2Int(p.x + 0, p.y + 1);
                    yield return new Vector2Int(p.x + 0, p.y - 1);
                    yield return new Vector2Int(p.x - 1, p.y + 0);
                    yield return new Vector2Int(p.x + 1, p.y + 0);
                    yield return new Vector2Int(p.x - 1, p.y - 1);
                    yield return new Vector2Int(p.x + 1, p.y - 1);
                }
                else
                {
                    yield return new Vector2Int(p.x + 0, p.y + 1);
                    yield return new Vector2Int(p.x + 0, p.y - 1);
                    yield return new Vector2Int(p.x - 1, p.y + 0);
                    yield return new Vector2Int(p.x + 1, p.y + 0);
                    yield return new Vector2Int(p.x - 1, p.y + 1);
                    yield return new Vector2Int(p.x + 1, p.y + 1);
                }
            }

            int PopMin(List<Vector2Int> list)
            {
                int bestIdx = 0;
                int bestScore = gScore[list[0]];
                for (int i = 1; i < list.Count; i++)
                {
                    int s = gScore[list[i]];
                    if (s < bestScore)
                    {
                        bestScore = s;
                        bestIdx = i;
                    }
                }
                var node = list[bestIdx];
                list.RemoveAt(bestIdx);
                return bestIdx >= 0 ? 0 : 0; // no-op to keep analyzer quiet
            }

            // Pop with min gScore
            Vector2Int PopMinNode(List<Vector2Int> list)
            {
                int bestIdx = 0;
                int bestScore = gScore[list[0]];
                for (int i = 1; i < list.Count; i++)
                {
                    int s = gScore[list[i]];
                    if (s < bestScore)
                    {
                        bestScore = s;
                        bestIdx = i;
                    }
                }
                var node = list[bestIdx];
                list.RemoveAt(bestIdx);
                return node;
            }

            // Dijkstra loop
            while (open.Count > 0)
            {
                var current = PopMinNode(open);
                if (current == endPos)
                {
                    // Reconstruct path of (position, costToEnterThatTile)
                    var path = new List<(Vector2Int, int)>();
                    var step = endPos;
                    while (!step.Equals(startPos))
                    {
                        path.Add((step, stepCost[step]));
                        step = cameFrom[step];
                    }
                    path.Add((startPos, 0));
                    path.Reverse();
                    return path;
                }

                foreach (var nb in Neighbors(current))
                {
                    // Cost to enter neighbor; use default(GoodsType) since signature provides it
                    int enterCost = transportCosts(nb, goods);

                    // Treat non-positive or very large as impassable
                    if (enterCost <= 0 || enterCost >= int.MaxValue / 4)
                        continue;

                    int tentative = gScore[current] + enterCost;

                    if (!gScore.TryGetValue(nb, out int existing) || tentative < existing)
                    {
                        gScore[nb] = tentative;
                        cameFrom[nb] = current;
                        stepCost[nb] = enterCost;

                        // push if not already in open
                        bool inOpen = false;
                        for (int i = 0; i < open.Count; i++)
                            if (open[i] == nb) { inOpen = true; break; }
                        if (!inOpen) open.Add(nb);
                    }
                }
            }

            // No path
            return null;
        }


    private static int transportCosts(Vector2Int position, GoodsType goods)
        {
            //TODO later include streets, tolls?, enemy terrain and ?maula? routes
            //TODO return If your transportCosts can see impassable tiles, return int.MaxValue (or <=0) for those.
            //TODO tell player how it was calculated
            //profit of traders?
            if (position.x < 0 || position.x >= Controller.GameMap.Length || position.y < 0 || position.y >= Controller.GameMap[position.x].Length || Controller.GameMap[position.x][position.y] == null)
                return int.MaxValue;

            return (int)Math.Ceiling(((float)Controller.GameMap[position.x][position.y].roughness) * Entity_Stats.Goods_Movement_Costs_Factor.GetValueOrDefault(goods, 1.0f));
        }
    }

}