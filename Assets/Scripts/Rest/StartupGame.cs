using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using System;
public class StartupGame : MonoBehaviour
{
    public static int entryMode; //0 tutorial 1 use savebelow

    public static string savePath;
    public Input_Manager_Controller inputManagerController;
    public Controller displayController;
    void Start()
    {
        if (entryMode == 0) {
            StartTutorial();
        }
        else {
            StartLoadingGame(savePath);
        }

    }

    public void StartTutorial() {
        Person.SetController(displayController);
        displayController.SetGameMap(YourInitFunction(60, 20));
        Culture gothCulture = new Culture("Goth");
        Person player1 = new Person("Player1_Matthias", gothCulture);
        displayController.SetPlayers(new Person[] { player1, new Person("Player2_null", gothCulture) });
        Person a = new Person("Randu", gothCulture);
        displayController.AddPerson(a);
        displayController.AddPerson(new Person("Maaaer", gothCulture));
        displayController.AddPerson(new Person("Fooder", gothCulture));
        displayController.AddEntity(new Settler_Unit(10, 7, player1));
        Settlement.foundSettlement(12, 7, a);
        Settlement.foundSettlement(5, 5, player1);
        Settlement.foundSettlement(5, 8, player1);

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

            foreach (var person in t.people)
            {
                displayController.AddPerson(person);
            }
            foreach (var j in t.GameMap) {
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