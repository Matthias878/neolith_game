using UnityEngine;
public class StartupGame : MonoBehaviour
{
    public Input_Manager_Controller inputManagerController;
    public Controller displayController;
    void Start()
    {
        GameObject ScriptHolder = GameObject.Find("Script_Holder_Ingame");
        displayController.SetGameMap(YourInitFunction(15, 19));
        displayController.SetEntities(EntitiesInit());
        displayController.add_Settlement(new Settlement(12, 7));
        displayController.add_Settlement(new Settlement(5, 5));
        inputManagerController.SetGameState(Input_Manager_State.Base_Tile_Layer);

    }

    Game_Entity[] EntitiesInit()
    {
        //TODO plant scout on 10 7 and check path via Sea
        
        Game_Entity[] entities = new Game_Entity[1];
        entities[0] = new Settler_Unit(10, 7);
        return entities;
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
                tileMap[i][j] = new HexTile_Info(i, j, TileType.Land); // Example values
            }
        }

    //buggy position of tiles maybe first remove old tiles? only Sea
        //Shells Goods
        tileMap[4][1] = new HexTile_Info(4, 1, TileType.Land, GoodsType.Shells); // Example values
        tileMap[4][2] = new HexTile_Info(4, 2, TileType.Land, GoodsType.Shells); // Example values
        tileMap[5][1] = new HexTile_Info(5, 1, TileType.Land, GoodsType.Shells); // Example values
        tileMap[5][2] = new HexTile_Info(5, 2, TileType.Land, GoodsType.Shells); // Example values

        // Flax Goods
        tileMap[7][3] = new HexTile_Info(7, 3, TileType.Land, GoodsType.Flax); // Example values
        tileMap[7][4] = new HexTile_Info(7, 4, TileType.Land, GoodsType.Flax); // Example values
        tileMap[7][5] = new HexTile_Info(7, 5, TileType.Land, GoodsType.Flax); // Example values
        tileMap[7][6] = new HexTile_Info(7, 6, TileType.Land, GoodsType.Flax); // Example values

        //Sea
        tileMap[9][8] = new HexTile_Info(9, 8, TileType.Sea); // Example values
        tileMap[9][5] = new HexTile_Info(9, 5, TileType.Sea); // Example values
        tileMap[9][6] = new HexTile_Info(9, 6, TileType.Sea); // Example values
        tileMap[9][7] = new HexTile_Info(9, 7, TileType.Sea); // Example values
        tileMap[11][7] = new HexTile_Info(11, 7, TileType.Sea); // Example values
        tileMap[11][6] = new HexTile_Info(11, 6, TileType.Sea, GoodsType.Flax); // Example values

        tileMap[0][14] = new HexTile_Info(0, 14, TileType.Sea); // Example values
        tileMap[18][14] = new HexTile_Info(18, 14, TileType.Sea); // Example values



        return tileMap;
    }
}