using System;
using System.Collections.Generic;

public class Clan //clans are all pops, indivual, entities Leaders sworn to the clan leader
{
    public Person Leader;
    //public List<Person> notablePeople;
    //public List<Pop> commoners;
    //public Game_Entity[] Units_Settlements;

    //public string name;
    //public Culture culture; //?

    //private bool settled;

    public Clan(Person leader)
    {
        //this.name = name;
        //this.culture = culture;
        this.Leader = leader;
        //this.settled = new Random().Next(2) == 0; //random for each new clan?
    }

    public override string ToString()
    {
        return $"The mighty clan: {Leader.name}'s \n These mighty rulers are sworn to the clan leader:\n" + getAllFollowers();
    }

    private string getAllFollowers()
    {

        string ret = "";

        foreach (var entity in Controller.movables)
        {
            var tle = entity.leader.getClanLeader();
            if (tle == Leader)
            {
                switch (entity.type)
                {
                    case "Pop":
                        break;
                    case "Settlement":
                        ret += "The mighty: " + entity.leader.name + " has sworn his Settlement at " + entity.x + ", " + entity.y + " to " + Leader.name + "\n";
                        break;
                    case "Scout":
                        ret += "The mighty: " + entity.leader.name + " has sworn his Scout at " + entity.x + ", " + entity.y + " to " + Leader.name + "\n";
                        break;
                    case "Settler":
                        ret += "The mighty: " + entity.leader.name + " has sworn his Settler at " + entity.x + ", " + entity.y + " to " + Leader.name + "\n";
                        break;
                    default:
                        break;
                }
            }
        }
        return ret;
    }
}