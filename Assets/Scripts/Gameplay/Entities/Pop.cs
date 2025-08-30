using UnityEngine;
using System.Collections.Generic;
using System.Linq;
public class Pop : Game_Entity
{
    //private Clan clan; leader?
    private Culture culture;
    private int averageWealth;

    public List<(Vector2 position, int size)> allPositions; //positions and size there

    private string opinion = "Likes peaceful trading,...";

    private Trait[] traits; //only for people? //maybe more current interests- include randomness

    public override void Turnend()
    {
        // Logic for ending the turn for this population group Production and growth
    }

    public Pop(int x, int y, Person leader) : base(5, x, y, leader.getClanLeader())
    {
        this.culture = leader.culture; //swornchief culture?
        this.allPositions = new List<(Vector2 position, int size)>();
        var position = new Vector2(x, y);
        this.allPositions.Add((position, 10)); //starting position and size
    }

    public override void move_starter() { Debug.Log("Error pops should not have move logic."); }

    public override void move_to(int x, int y) { Debug.Log("Error pops should not have move logic."); }

    public override void presentActions_and_Data() { Debug.Log("Error pops should not have Actions or Data to present."); }

    public int getSizeAtPosition(Vector2 position)
    {
        var pos = allPositions.FirstOrDefault(p => p.position == position);
        return pos.size;
    }
}
