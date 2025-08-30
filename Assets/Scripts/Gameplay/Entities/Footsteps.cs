using UnityEngine;
public class Footsteps : Game_Entity
{
    private Game_Entity unit_to_move;
    public Footsteps(int x, int y, Game_Entity unit_to_move) : base(2, x, y, null)
    {
        this.unit_to_move = unit_to_move;
    }


    public override void presentActions_and_Data()
    {
        //present users or do AI
        //return the fact move was called
        //build settlement()
    }

    public override void move_starter()
    {
        unit_to_move.move_to(x,y);
    }

    public override void Turnend()
    {
        Debug.Log("TODO call inputfail, should not be able to end turn with footsteps");
    }

    public override void move_to(int x, int y)
    {
        Debug.Log("Not applicable, object (Footsteps) should not move to a different tile.");
    }
}
