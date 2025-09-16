using UnityEngine;
public class Footsteps : Game_Entity
{
    private Movable unit_to_move;
    public Footsteps(int x, int y, Movable unit_to_move) : base(2, x, y, null)
    {
        this.unit_to_move = unit_to_move;
    }


    public override void presentActions_and_Data()
    {
        unit_to_move.move_to(x,y);
        //present users or do AI
        //return the fact move was called
        //build settlement()
    }

    public override void Turnend()
    {
        Debug.Log("TODO call inputfail, should not be able to end turn with footsteps");
    }

}
