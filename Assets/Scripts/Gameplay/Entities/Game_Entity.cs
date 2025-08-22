using UnityEngine;
public abstract class Game_Entity // Base class for all game entities (units, buildings, etc.)
{
    public bool haschanged_render = true;//use later for render efficiency
    public static Controller Controller_GameData = GameObject.Find("Script_Holder_Ingame").GetComponent<Controller>();
    private static int nextId = 1;public readonly int id; // Threadsafe
    public int x; public int y; //public readonly string name;
    public readonly string type; // Type of the entity (e.g., "Settler", "Warrior")//!type name needs to match sprite name!
    public readonly string description;
    public abstract void presentActions_and_Data(); //settle fight move build etc. add buttons 
    public abstract void move_to(Vector2Int endpos); // Move to a new position
    public abstract void move_starter();
    public abstract void Turnend();
    public Game_Entity(int tt, int x, int y)
    {
        //tt is type 1 is settler 2 is footsteps 3 is scout
        //4 is settlement
        //this.name = name;
        this.x = x;
        this.y = y;
        if (tt == 1)
        {
            this.description = "description_Settler";
            this.type = "Settler";
        }
        else if (tt == 2)
        {
            this.description = "description_Footsteps";
            this.type = "Footsteps";
        }
        else if (tt == 3)
        {
            this.description = "description_Scout";
            this.type = "Scout";
        }
        else if (tt == 4)
        {
            this.description = "description_Settlement";
            this.type = "Settlement";
        }
        this.id = nextId++;
    }
}
