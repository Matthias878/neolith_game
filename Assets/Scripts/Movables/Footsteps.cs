using UnityEngine;
public class Footsteps : Game_Entity
{
    private Game_Entity unit_to_move;
    public Footsteps(int x, int y, Game_Entity unit_to_move) : base(2, "Footstep", x, y)
    {
        this.movePoints = 0;
        this.health = 1;
        this.unit_to_move = unit_to_move;
    }


    public override void presentActions()
    {
        //present users or do AI
        //return the fact move was called
        //build settlement()
    }

    public override void move()
    {
        unit_to_move.move(new Vector2Int(x, y));
    }

    public override void Turnend()
    {
        Debug.Log("TODO call inputfail, should not be able to end turn with footsteps");
    }
    
    public override void move(Vector2Int endpos)
    {
        Debug.Log("Not applicable, object (Footsteps) should not move to a different tile.");
    }
}
