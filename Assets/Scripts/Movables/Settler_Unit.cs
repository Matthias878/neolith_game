using System.Collections.Generic;
using UnityEngine;
public class Settler_Unit : Game_Entity
{
    private int maxMovePoints = 2; // Default move points for a settler
    public Settler_Unit(string name, int x, int y) : base(1, name, x, y)
    {
        this.movePoints = 2;
        this.health = 100;
    }

    public override void presentActions()
    {
        //present users or do AI
        //return the fact move was called
        //build settlement()
    }

    public override void move()
    {
        GameObject holder = GameObject.Find("Script_Holder_Ingame");
        Display_Controller display = holder.GetComponent<Display_Controller>();

        HashSet<Vector2Int> reachableTiles = NeolithianRev.Utility.MovementAlgos.GetReachableTiles(
            ConvertToTileTypeArray_helper(display.GameMap),
            new Vector2Int(x, y),
            movePoints,
            Entity_Stats.Settler_Movement_Costs
        );
        string positions = string.Join(", ", reachableTiles);
        Debug.Log("current Movepoints are: " + movePoints + " Reachable positions: " + positions);
        foreach (var tile in reachableTiles)
        {
            display.add_Entity(new Footsteps(tile.x, tile.y, this));
        }
        // Here you can implement logic to present reachable tiles to the player

    }

    public override void Turnend()
    {
        movePoints = maxMovePoints; // Reset move points at the end of the turn
    }

    public override void move(Vector2Int endpos)
    {
        Debug.Log("Moving from " + x + y + " to " + endpos);
        //gamemap from dispay, startpos is x and y only give endpos
        GameObject holder = GameObject.Find("Script_Holder_Ingame");
        Display_Controller display = holder.GetComponent<Display_Controller>();

        List<Vector2Int> path = NeolithianRev.Utility.MovementAlgos.GetPath(
            ConvertToTileTypeArray_helper(display.GameMap),
            new Vector2Int(x, y),
            endpos,
            movePoints,
            Entity_Stats.Settler_Movement_Costs
        );

        if (path != null && path.Count > 1)
        {
            // Here you can implement logic to animate path
            int totalCost = 0;
            TileType[][] tileTypeMap = ConvertToTileTypeArray_helper(display.GameMap);
            for (int i = 1; i < path.Count; i++) // skip start tile
            {
                Vector2Int pos = path[i];
                TileType tileType = tileTypeMap[pos.x][pos.y];
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
            display.remove_Entity(this);
            display.add_Entity(this);
        }
        else
        {
            // Path not found or already at destination
        }
    }

    private static TileType[][] ConvertToTileTypeArray_helper(HexTile_Tnfo[][] hexMap)
    {
        int width = hexMap.Length;
        TileType[][] result = new TileType[width][];
        for (int x = 0; x < width; x++)
        {
            int height = hexMap[x].Length;
            result[x] = new TileType[height];
            for (int y = 0; y < height; y++)
            {
                result[x][y] = hexMap[x][y].type;
            }
        }
        return result;
    }

    private void buildSettlement()
    {
        // Implement settlement building logic here
    }
}
