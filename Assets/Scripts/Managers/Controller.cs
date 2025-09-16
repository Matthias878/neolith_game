using UnityEngine;
using System;
using System.Linq;
using Newtonsoft.Json;
using System.IO;
//TODO create for GameMap, create a new Gameobject at game start and only rerender it haschanged is true 
public class Controller : MonoBehaviour
{

    public int inGameTime = 0;
    public Input_Manager_Controller inputManagerController;

    //GameData to save/load
    public static HexTile_Info[][] GameMap; public static HexTile_Info[][] GameMap_toremove;

    //TODO INTEGRATE movables and settlements
    public static Game_Entity[] movables = new Game_Entity[0]; public static Game_Entity[] movables_toremove = new Game_Entity[0]; private static bool maphasbeenset = false;
    //public Settlement[] settlements; public Settlement[] settlements_toremove;

    //public static Person[] people; 
    public static Movable[] players;

    public My_TilemapRenderer tilemapRenderer;

    private bool isPaused = false;

    void Update()
    {
        if (isPaused)
        {
            return;
        }
        if (!maphasbeenset)//what if save load
        {
            return;
        }

        HexTile_Info[][] temp = new HexTile_Info[GameMap.Length][];

        for (int i = 0; i < GameMap.Length; i++)
        {
            temp[i] = new HexTile_Info[GameMap[i].Length];
            for (int j = 0; j < GameMap[i].Length; j++)
            {
                if (GameMap[i][j] != null && GameMap[i][j].haschanged_render)
                {
                    temp[i][j] = GameMap[i][j];
                    GameMap[i][j].haschanged_render = false; // Reset haschanged after rendering
                }
            }
        }
        tilemapRenderer.RenderMap(temp); //Render the current map

        Game_Entity[] tempEntities = new Game_Entity[movables.Length];
        for (int i = 0; i < movables.Length; i++)
        {
            if (movables[i] != null && movables[i].haschanged_render)
            {
                tempEntities[i] = movables[i];
                movables[i].haschanged_render = false; // Reset haschanged after rendering
            }
        }
        tilemapRenderer.RenderEntities(tempEntities); //Render entities on the map

        // Cleanup
        tilemapRenderer.StopRendering(GameMap_toremove, movables_toremove);
        GameMap_toremove = null; // Reset after rendering
        movables_toremove = null;
        //settlements_toremove = null;
    }

    public void AddEntity_toremove(Game_Entity entity)
    {
        if (movables_toremove == null)
        {
            movables_toremove = new Game_Entity[] { entity };
        }
        else
        {
            Array.Resize(ref movables_toremove, movables_toremove.Length + 1);
            movables_toremove[movables_toremove.Length - 1] = entity;
        }
        if (movables != null)
        {
            movables = movables.Where(e => e != entity).ToArray();
        }
    }

    public void AddEntity(Game_Entity entity)
    {
        if (movables == null)
        {
            movables = new Game_Entity[] { entity };
        }
        else
        {
            if (movables.Contains(entity))
            {
                Debug.LogWarning("Entity " + entity.type + " " + entity.id + ", " + entity.x + ", " + entity.y + " already exists in movables array.");
                return;
            }
            Array.Resize(ref movables, movables.Length + 1);
            movables[movables.Length - 1] = entity;
        }

    }

    public void SetGameMap(HexTile_Info[][] gameMap)
    {
        if (GameMap != null)
        {
            GameMap_toremove = GameMap; // Store the old map for cleanup
        }
        GameMap = gameMap;
        maphasbeenset = true;
    }


    public void SetPlayers(Movable[] players)
    {
        Controller.players = players;
        foreach (var player in players)
        {
            if (player != null && movables != null && !movables.Contains(player))
            {
                AddEntity(player);
            }
        }
    }

    public void SaveCurrentGame(string path)
    {
        isPaused = true;
        tilemapRenderer.StopRendering(GameMap_toremove, movables_toremove);
        SaveData state = new SaveData(GameMap, movables, players);
        SaveGame.SaveCurrentGame(state, path);
        isPaused = false;
    }

    public static SaveData LoadGame(string path)
    {
        return SaveGame.LoadGame(path);
    }

    public void Outline()
    {
        //tilemapRenderer.RemoveAllOutlines();
        tilemapRenderer.OutlineTileAt(players[0].x, players[0].y, Color.red, 25f);
    }

}
