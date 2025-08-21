using UnityEngine;
using System;
using System.Linq;

//all gameobjects must have scripts containing

// This script is responsible for controlling the display of the game at all times
public class Display_Controller : MonoBehaviour
{
    public Input_Manager_Controller inputManagerController;
    public HexTile_Tnfo[][] GameMap;    //Base_Tile_Layer

    public Game_Entity[] movables; //units and such //on Entity_Layer
    public My_TilemapRenderer tilemapRenderer;

    // Update is called once per frame
    void Update()
    {
        if (GameMap != null)
            tilemapRenderer.RenderMap(GameMap, movables);
    }

    public void SetGameMap(HexTile_Tnfo[][] newMap)
    {
        GameMap = newMap;
    }

    public void SetEntities(Game_Entity[] entities)
    {
        movables = entities;
    }

    public void add_Entity(Game_Entity entity)
    {
        if (movables == null)
        {
            movables = new Game_Entity[] { entity };
        }
        else
        {
            Array.Resize(ref movables, movables.Length + 1);
            movables[movables.Length - 1] = entity;
        }
    }

    public void remove_Entity(Game_Entity entity)
    {
        if (movables != null)
        {
            movables = movables.Where(e => e != entity).ToArray();
        }
    }   
}
