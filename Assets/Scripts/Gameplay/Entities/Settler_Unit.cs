using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class Settler_Unit : Game_Entity
{
    private int movePoints;
    protected int health;
    private int maxMovePoints = 6; // Default move points for a settler
    public Settler_Unit(int x, int y) : base(1, x, y)
    {
        this.movePoints = maxMovePoints;
        this.health = 100;
    }

    public override void presentActions_and_Data()
    {

        Button newButton = Controller_GameData.inputManagerController.addUIButton();
        newButton.GetComponentInChildren<TMP_Text>().text = "Click this button to found a new settlement.";
        newButton.onClick.AddListener(() => foundSettlement());

        
        Button newButtontwo = Controller_GameData.inputManagerController.addUIButton();
        newButtontwo.GetComponentInChildren<TMP_Text>().text = "fortify here.";
        newButtontwo.onClick.AddListener(() => fortify());

    }

    private void foundSettlement()
    {
        //logic
        Controller_GameData.AddSettlement(new Settlement(x, y));
        //change into city management
        Controller_GameData.inputManagerController.SetGameState(Input_Manager_State.Settlement_Management_Layer);
    }

    private void fortify()
    {
        Controller_GameData.inputManagerController.SetGameState(Input_Manager_State.Settlement_Management_Layer);
        Debug.Log("Fortifying at position: " + x + ", " + y);
    }

    public override void move_starter()
    {

        HashSet<Vector2Int> reachableTiles = NeolithianRev.Utility.MovementAlgos.GetReachableTiles(
            ConvertToTileTypeArray_helper(Controller_GameData.GameMap),
            new Vector2Int(x, y),
            movePoints,
            Entity_Stats.Settler_Movement_Costs
        );
        string positions = string.Join(", ", reachableTiles);
        Debug.Log("current Movepoints are: " + movePoints + " Reachable positions: " + positions);
        foreach (var tile in reachableTiles)
        {
            Controller_GameData.AddEntity(new Footsteps(tile.x, tile.y, this));
        }
        // Here you can implement logic to present reachable tiles to the player

    }

    public override void Turnend()
    {
        movePoints = maxMovePoints; // Reset move points at the end of the turn
    }

    public override void move_to(Vector2Int endpos)
    {
        Debug.Log("Moving from " + x + y + " to " + endpos);
        //gamemap from dispay, startpos is x and y only give endpos

        List<Vector2Int> path = NeolithianRev.Utility.MovementAlgos.GetPath(
            ConvertToTileTypeArray_helper(Controller_GameData.GameMap),
            new Vector2Int(x, y),
            endpos,
            movePoints,
            Entity_Stats.Settler_Movement_Costs
        );

        if (path != null && path.Count > 1)
        {
            // Here you can implement logic to animate path
            int totalCost = 0;
            TileType[][] tileTypeMap = ConvertToTileTypeArray_helper(Controller_GameData.GameMap);
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

            this.haschanged_render = true; // Mark for rendering
        }
        else
        {
            // Path not found or already at destination
        }
    }

    private static TileType[][] ConvertToTileTypeArray_helper(HexTile_Info[][] hexMap)
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
