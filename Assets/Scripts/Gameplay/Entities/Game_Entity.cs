using UnityEngine;
public abstract class Game_Entity // Base class for all game entities (units, buildings, etc.)
{
    //public bool isMovable = false;
    public bool haschanged_render = true;//use later for render efficiency
    public bool neverRender = false; // Flag to control rendering
    public static Controller Controller_GameData = GameObject.Find("Script_Holder_Ingame").GetComponent<Controller>();
    private static int nextId = 1; public readonly int id; // Threadsafe
    public int x; public int y; //public readonly string name;
    public readonly string type; // Type of the entity (e.g., "Settler", "Warrior")//!type name needs to match sprite name!
    //public readonly string description;
    public Person leader;
    public abstract void presentActions_and_Data(); //settle fight move build etc. add buttons 
    public abstract void Turnend();
    public Game_Entity(int tt, int x, int y, Person leader)
    {
        //tt is type 1 is settler 2 is footsteps 3 is scout
        //4 is settlement 5 is pop
        //this.name = name;
        this.x = x;
        this.y = y;
        this.leader = leader;
        if (tt == 1)
        {
            this.type = "Settler";
        }
        else if (tt == 2)
        {
            this.type = "Footsteps";
        }
        else if (tt == 3)
        {
            this.type = "Scout";
        }
        else if (tt == 4)
        {
            this.type = "Settlement";
        }
        else if (tt == 5)
        {
            this.neverRender = true;
            this.type = "Pop";
        }
        else if (tt == 6)
        {
            this.type = "Person";
        }
        else if (tt == 7)
        {
            this.type = "Messenger";
        }
        else
        {
            this.type = "Unknown";
            Debug.LogError("Unknown entity type: " + tt);
        }
        this.id = nextId++;
    }
    
    //TODO abstract toInformation() when hovering over entity
}
