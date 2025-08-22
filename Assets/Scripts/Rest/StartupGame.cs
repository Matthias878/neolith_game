using UnityEngine;
public class StartupGame : MonoBehaviour
{
    public Input_Manager_Controller inputManagerController;
    public Controller displayController;
    void Start()
    {
        GameObject ScriptHolder = GameObject.Find("Script_Holder_Ingame");
        displayController.SetGameMap(YourInitFunction(60, 20));
        displayController.AddEntity(new Settler_Unit(10, 7));
        displayController.AddSettlement(new Settlement(12, 7));
        displayController.AddSettlement(new Settlement(5, 5));
        displayController.SetPlayers(new Person[] { new Person("Player1_Matthias"), new Person("Player2_null") });
        inputManagerController.SetGameState(Input_Manager_State.Base_Tile_Layer);

    }
    HexTile_Info[][] YourInitFunction(int mapWidth, int mapHeight)
    {
        Debug.Log("Init after PlayGame");

        // TODO LATER: CALLFUNCTION CREATE MAP (SIZE, other params...)

        // TEMP: Initialize the array
        HexTile_Info[][] tileMap = new HexTile_Info[mapHeight][]; // Example: 12 rows
        for (int i = 0; i < tileMap.Length; i++)
        {
            tileMap[i] = new HexTile_Info[mapWidth]; // Example: 7 columns per row
            for (int j = 0; j < tileMap[i].Length; j++)
            {
                // Initialize each tile with default values
                tileMap[i][j] = new HexTile_Info(i, j, TileType.Land, new GoodsType[0]); // Example values
            }
        }

        //Sea
        tileMap[9][8] = new HexTile_Info(9, 8, TileType.Sea, new GoodsType[0]); // Example values
        tileMap[9][5] = new HexTile_Info(9, 5, TileType.Sea, new GoodsType[0]); // Example values
        tileMap[9][6] = new HexTile_Info(9, 6, TileType.Sea, new GoodsType[0]); // Example values
        tileMap[9][7] = new HexTile_Info(9, 7, TileType.Sea, new GoodsType[0]); // Example values
        tileMap[11][7] = new HexTile_Info(11, 7, TileType.Sea, new GoodsType[0]); // Example values
        tileMap[11][6] = new HexTile_Info(11, 6, TileType.Sea, new GoodsType[] { GoodsType.Flax }); // Example values

        tileMap[0][14] = new HexTile_Info(0, 14, TileType.Sea, new GoodsType[0]); // Example values
        tileMap[18][14] = new HexTile_Info(18, 14, TileType.Sea, new GoodsType[0]); // Example values



        return tileMap;
    }
}