using UnityEngine;
public abstract class Game_Entity
{
    // Base class for all game entities (units, buildings, etc.)
    private static int nextId = 1;
    public readonly int id; // Unique identifier for the entity

    public int x; // X position on the map

    public int y; // Y position on the map
    protected int movePoints;

    protected int health;

    public readonly string type; // Type of the entity (e.g., "Settler", "Warrior")
    //type name needs to match sprite name
    public readonly string name;

    public readonly string description;

    public abstract void presentActions(); //settle fight move build etc.

    public abstract void move(Vector2Int endpos); // Move to a new position

    
    public abstract void move();

    public abstract void Turnend();

    public Game_Entity(int tt, string name, int x, int y)
    {
        //tt is type 1 is settler 2 is footsteps 3 is scout
        this.name = name;
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
        this.id = nextId++;

    }

}
