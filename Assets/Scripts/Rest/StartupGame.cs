using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using System;
public class StartupGame : MonoBehaviour
{
    public static int entryMode; //0 tutorial 1 use savebelow
    //2 generate small world, 3 medium, 4 large
    public static string savePath;
    public Input_Manager_Controller inputManagerController;
    public Controller displayController;
    void Start()
    {
        if (entryMode == 0)
        {
            StartTutorial();
        }
        else if (entryMode == 1)
        {
            StartLoadingGame(savePath);
        }
        else if (entryMode == 2)
        {
            Debug.Log("Generating small world.");
            Culture gothCulture = new Culture("Goth");
            Person player1 = new Person("Player1_Matthias", gothCulture, 6, 7);
            StartUpGame(YourInitFunction(60, 40), new Person[] { player1 }, new (int, int, Person)[0], new Game_Entity[0]);
        }
        else if (entryMode == 3)
        {
            Debug.Log("Generating medium world.");
            Culture gothCulture = new Culture("Goth");
            Person player1 = new Person("Player1_Matthias", gothCulture, 4 ,8);
            StartUpGame(YourInitFunction(150, 60), new Person[] { player1 }, new (int, int, Person)[0], new Game_Entity[0]);
        } else if (entryMode == 4)
        {
            Debug.Log("Generating large world.");
            Culture gothCulture = new Culture("Goth");
            Person player1 = new Person("Player1_Matthias", gothCulture, 66 ,40);
            StartUpGame(YourInitFunction(200, 80), new Person[] { player1 }, new (int, int, Person)[0], new Game_Entity[0]);
        }
        else
        {
            Debug.LogError("Invalid entry mode: " + entryMode);
        }

    }

    public void StartTutorial()
    {
        HexTile_Info[][] gameMap = YourInitFunction(60, 20);

        Culture gothCulture = new Culture("Goth");

        Person player1 = new Person("Player1_Matthias", gothCulture, 3 ,6);
        
        Person personRandu = new Person("Randu", gothCulture, 1, 7);

        //Players
        var a = new Person[] {
            player1,
            new Person("Player2_null", gothCulture, 15, 6)
            }; 


        //settlements
        var b = new (int, int, Person)[] {
            (12, 7, personRandu),
            (5, 5, player1),
            (5, 8, player1)
        };

        //Game entities
        var c = new Game_Entity[] {
            new Settler_Unit(10, 7, player1),
            personRandu,
            new Person("Maaaer", gothCulture, 6, 5),
            new Person("Fooder", gothCulture, 9, 8, player1)
        }; 

        StartUpGame(gameMap, a, b, c);
    }

    private void StartUpGame(HexTile_Info[][] gameMap, Movable[] players, (int, int, Person)[] settlements, Game_Entity[] entities)
    {
        displayController.SetGameMap(gameMap);
        displayController.SetPlayers(players);
        foreach (var settlement in settlements)
        {
            Settlement.foundSettlement(settlement.Item1, settlement.Item2, settlement.Item3);
        }
        foreach (var entity in entities)
        {
            displayController.AddEntity(entity);
        }
        inputManagerController.SetGameState(Input_Manager_State.Base_Tile_Layer);
    }

    public void StartLoadingGame(string savePath)
    {//is creating many duplicates? check give id to everything?
        var t = SaveGame.LoadGame(savePath);
        if (t != null)
        {
            displayController.SetPlayers(t.players);//allow change
            foreach (var entity in t.movables)
            {
                entity.haschanged_render = true;
                if (!Array.Exists(Controller.movables, e => e.id == entity.id))
                {
                    displayController.AddEntity(entity);
                }
                else
                {
                    Debug.Log("Entity with id " + entity.id + " already exists in movables, skipping addition.");
                }
            }

            foreach (var j in t.GameMap)
            {
                foreach (var tile in j)
                {
                    tile.haschanged_render = true;
                }
            }
            displayController.SetGameMap(t.GameMap);
            inputManagerController.SetGameState(Input_Manager_State.Base_Tile_Layer);
        }
    }

    HexTile_Info[][] YourInitFunction(int mapWidth_xvalue, int mapHeight_yvalue)
    {
        //x geht nach rechts, y geht nach oben

        HexTile_Info[][] tileMap;
        tileMap = new HexTile_Info[mapWidth_xvalue][];
        for (int x = 0; x < mapWidth_xvalue; x++)
        {
            tileMap[x] = new HexTile_Info[mapHeight_yvalue];
            for (int y = 0; y < mapHeight_yvalue; y++)
            {
                tileMap[x][y] = new HexTile_Info(x, y, new GoodsType[0]);
            }
        }

        //Sea
        tileMap[9][8] = new HexTile_Info(9, 8,   new GoodsType[0], true); // Example values
        tileMap[9][5] = new HexTile_Info(9, 5,   new GoodsType[0], true); // Example values
        tileMap[9][6] = new HexTile_Info(9, 6,   new GoodsType[0], true); // Example values
        tileMap[9][7] = new HexTile_Info(9, 7,   new GoodsType[0], true); // Example values
        tileMap[11][7] = new HexTile_Info(11, 7,   new GoodsType[0], true); // Example values
        tileMap[11][6] = new HexTile_Info(11, 6,   new GoodsType[0], true); // Example values

        tileMap[0][14] = new HexTile_Info(0, 14,   new GoodsType[0], true); // Example values
        tileMap[18][14] = new HexTile_Info(18, 14,   new GoodsType[0], true); // Example values



        return tileMap;
    }
}