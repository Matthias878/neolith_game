using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public abstract class Movable : Game_Entity
{
    public int movePoints;
    public int health;
    public int maxMovePoints;

    public Culture culture; //culture of the unit, could be different from leader ???
    public Movable(int classId, int x, int y, Person leader) : base(classId, x, y, leader)
    {
        // ismovable = true;
    }

    //public abstract void move_to(int x, int y); // Move to a new position endpos //only moveables
    //public abstract void move_starter(); //only movables


    public void move_starter()
    {

        HashSet<Vector2Int> reachableTiles = NeolithianRev.Utility.MovementAlgos.GetReachableTiles(
            ConvertToTileTypeArray_helper(Controller.GameMap),
            new Vector2Int(x, y),
            movePoints,
            Entity_Stats.Settler_Movement_Costs
        );
        string positions = string.Join(", ", reachableTiles);
        //Debug.Log("current Movepoints are: " + movePoints + " Reachable positions: " + positions);
        foreach (var tile in reachableTiles)
        {
            Controller_GameData.AddEntity(new Footsteps(tile.x, tile.y, this));
        }
        // Here you can implement logic to present reachable tiles to the player

    }

    public void move_to(int newx, int newy)
    {
        
        //Debug.Log("Moving from " + x + y + " to " + endpos);
        //gamemap from dispay, startpos is x and y only give endpos

        List<Vector2Int> path = NeolithianRev.Utility.MovementAlgos.GetPath(
            ConvertToTileTypeArray_helper(Controller.GameMap),
            new Vector2Int(x, y),
            new Vector2Int(newx, newy),
            movePoints,
            Entity_Stats.Settler_Movement_Costs
        );

        if (path != null && path.Count > 1)
        {
            // Here you can implement logic to animate path
            int totalCost = 0;
            Terrain[][] tileTypeMap = ConvertToTileTypeArray_helper(Controller.GameMap);
            for (int i = 1; i < path.Count; i++) // skip start tile
            {
                Vector2Int pos = path[i];
                Terrain tileType = tileTypeMap[pos.x][pos.y];
                if (Entity_Stats.Settler_Movement_Costs.TryGetValue(tileType, out int cost))
                {
                    totalCost += cost;
                }
            }
            // Update position and movePoints
            Vector2Int finalPos = path[path.Count - 1];
            x = finalPos.x;
            y = finalPos.y;
            movePoints -= totalCost;
            if (movePoints < 0) movePoints = 0;

            this.haschanged_render = true; // Mark for rendering
        }
        else
        {
            // Path not found or already at destination
        }
    }

    public static Terrain[][] ConvertToTileTypeArray_helper(HexTile_Info[][] hexMap)
    {
        int width = hexMap.Length;
        Terrain[][] result = new Terrain[width][];
        for (int x = 0; x < width; x++)
        {
            int height = hexMap[x].Length;
            result[x] = new Terrain[height];
            for (int y = 0; y < height; y++)
            {
                result[x][y] = hexMap[x][y].terrain;
            }
        }
        return result;
    }

}
