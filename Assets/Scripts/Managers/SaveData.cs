[System.Serializable]
public class SaveData
{

    public HexTile_Info[][] GameMap;
    public Game_Entity[] movables;
    public Movable[] players;

    public SaveData(HexTile_Info[][] GameMap, Game_Entity[] movables, Movable[] players)
    {
        this.GameMap = GameMap;
        this.movables = movables;
        this.players = players;
    }
}