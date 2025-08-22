using UnityEngine;
using System;
using System.Linq;

//TODO create for GameMap, create a new Gameobject at game start and only rerender it haschanged is true 
public class Controller : MonoBehaviour
{
    public Input_Manager_Controller inputManagerController;

    //GameData to save/load
    public HexTile_Info[][] GameMap; public HexTile_Info[][] GameMap_toremove;

    public Game_Entity[] movables; public Game_Entity[] movables_toremove;
    public Settlement[] settlements; public Settlement[] settlements_toremove;

    public Clan[] clans; //TODO
    //public Culture[] cultures;?
    public Person[] people;    public Person[] players;
    public My_TilemapRenderer tilemapRenderer;

    void Update()
    {
        //How do you stop rendering something
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

        Settlement[] tempSettlements = new Settlement[settlements.Length];
        for (int i = 0; i < settlements.Length; i++)
        {
            if (settlements[i] != null && settlements[i].haschanged_render)
            {
                tempSettlements[i] = settlements[i];
                settlements[i].haschanged_render = false; // Reset haschanged after rendering
            }
        }
        tilemapRenderer.RenderSettlements(tempSettlements); //Render settlements on the map

        // Cleanup
        tilemapRenderer.StopRendering(GameMap_toremove, movables_toremove, settlements_toremove);
        GameMap_toremove = null; // Reset after rendering
        movables_toremove = null;
        settlements_toremove = null;
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

    public void AddSettlement_toremove(Settlement settlement)
    {
        if (settlements_toremove == null)
        {
            settlements_toremove = new Settlement[] { settlement };
        }
        else
        {
            Array.Resize(ref settlements_toremove, settlements_toremove.Length + 1);
            settlements_toremove[settlements_toremove.Length - 1] = settlement;
        }
        if (settlements != null)
        {
            settlements = settlements.Where(e => e != settlement).ToArray();
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
            Array.Resize(ref movables, movables.Length + 1);
            movables[movables.Length - 1] = entity;
        }

    }

    public void AddSettlement(Settlement settlement)
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

    public void SetGameMap(HexTile_Info[][] gameMap)
    {
        if (GameMap != null)
        {
            GameMap_toremove = GameMap; // Store the old map for cleanup
        }
        GameMap = gameMap;
        tilemapRenderer.RenderMap(gameMap); // Initial render of the new map
    }

    public void AddPerson(Person person)
    {
        if (people == null)
        {
            people = new Person[] { person };
        }
        else
        {
            Array.Resize(ref people, people.Length + 1);
            people[people.Length - 1] = person;
        }
    }

    public void SetPlayers(Person[] players)
    {
        this.players = players;
        foreach (var player in players)
        {
            if (player != null && people != null && !people.Contains(player))
            {
                AddPerson(player);
            }
        }
    }
}
