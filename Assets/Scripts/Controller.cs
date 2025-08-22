using UnityEngine;
using System;
using System.Linq;

//all gameobjects must have scripts containing

// This script is responsible for controlling the display of the game at all times
public class Controller : MonoBehaviour
//TODO turn into global controller
{
    public Input_Manager_Controller inputManagerController;

    //GameData to save/load

    //coord system is 
    public HexTile_Info[][] GameMap;    //Base_Tile_Layer 

    public Game_Entity[] movables; //units and such different layers

    public Settlement[] settlements; //on different layers //Rendering of settlements buggy?
    public My_TilemapRenderer tilemapRenderer;

    // Update is called once per frame
    void Update()
    {
        if (GameMap != null)
            tilemapRenderer.RenderMap(GameMap, movables, settlements); //Render the current map
    }

    public void SetGameMap(HexTile_Info[][] newMap)
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

    public void SetSettlements(Settlement[] newSettlements)
    {
        settlements = newSettlements;
    }
    
    public void add_Settlement(Settlement settlement)
    {
        if (settlements == null)
        {
            settlements = new Settlement[] { settlement };
        }
        else
        {
            Array.Resize(ref settlements, settlements.Length + 1);
            settlements[settlements.Length - 1] = settlement;
        }
    }
}
