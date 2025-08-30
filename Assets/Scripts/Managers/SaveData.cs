[System.Serializable]
public class SaveData
{

    public HexTile_Info[][] GameMap;
    public Game_Entity[] movables;
    public Person[] people;
    public Person[] players;

    public SaveData(HexTile_Info[][] GameMap, Game_Entity[] movables, Person[] people, Person[] players)
    {
        this.GameMap = GameMap;
        this.movables = movables;
        this.people = people;
        this.players = players;
    }
}