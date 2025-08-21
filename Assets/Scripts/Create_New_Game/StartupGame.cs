using UnityEngine;

public class StartupGame : MonoBehaviour
{
    public Input_Manager_Controller inputManagerController;
    public Display_Controller displayController;
    void Start()
    {
        GameObject ScriptHolder = GameObject.Find("Script_Holder_Ingame");
        displayController.SetGameMap(YourInitFunction(14, 16));
        displayController.SetEntities(EntitiesInit());
        inputManagerController.SetGameState(Input_Manager_State.Base_Tile_Layer);
    }

    Game_Entity[] EntitiesInit()
    {
        //TODO plant scout on 10 7 and check path via mountains
        
        Game_Entity[] entities = new Game_Entity[1];
        entities[0] = new Settler_Unit("Hans",10,7);
        return entities;
    }

    HexTile_Tnfo[][] YourInitFunction(int mapWidth, int mapHeight)
    {
        Debug.Log("Init after PlayGame");

        // TODO LATER: CALLFUNCTION CREATE MAP (SIZE, other params...)

        // TEMP: Initialize the array
        HexTile_Tnfo[][] tileMap = new HexTile_Tnfo[mapHeight][]; // Example: 12 rows
        for (int i = 0; i < tileMap.Length; i++)
        {
            tileMap[i] = new HexTile_Tnfo[mapWidth]; // Example: 7 columns per row
            for (int j = 0; j < tileMap[i].Length; j++)
            {
                // Initialize each tile with default values
                tileMap[i][j] = new HexTile_Tnfo(i, j, TileType.Plains); // Example values
            }
        }

    //buggy position of tiles maybe first remove old tiles? only mountains
        //Shells Goods
        tileMap[4][1] = new HexTile_Tnfo(4, 1, TileType.Plains, GoodsType.Shells); // Example values
        tileMap[4][2] = new HexTile_Tnfo(4, 2, TileType.Plains, GoodsType.Shells); // Example values
        tileMap[5][1] = new HexTile_Tnfo(5, 1, TileType.Plains, GoodsType.Shells); // Example values
        tileMap[5][2] = new HexTile_Tnfo(5, 2, TileType.Plains, GoodsType.Shells); // Example values

        // Flax Goods
        tileMap[7][3] = new HexTile_Tnfo(7, 3, TileType.Plains, GoodsType.Flax); // Example values
        tileMap[7][4] = new HexTile_Tnfo(7, 4, TileType.Plains, GoodsType.Flax); // Example values
        tileMap[7][5] = new HexTile_Tnfo(7, 5, TileType.Plains, GoodsType.Flax); // Example values
        tileMap[7][6] = new HexTile_Tnfo(7, 6, TileType.Plains, GoodsType.Flax); // Example values

        //Mountains
        tileMap[9][8] = new HexTile_Tnfo(9, 8, TileType.Mountains); // Example values
        tileMap[9][5] = new HexTile_Tnfo(9, 5, TileType.Mountains); // Example values
        tileMap[9][6] = new HexTile_Tnfo(9, 6, TileType.Mountains); // Example values
        tileMap[9][7] = new HexTile_Tnfo(9, 7, TileType.Mountains); // Example values
        tileMap[11][7] = new HexTile_Tnfo(11, 7, TileType.Mountains); // Example values
        tileMap[11][6] = new HexTile_Tnfo(11, 6, TileType.Mountains, GoodsType.Flax); // Example values

    


        return tileMap;
    }
}