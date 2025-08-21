using UnityEngine;

[System.Serializable]
public class HexTile_Tnfo
{
    public bool haschanged; //so the hex doesn't have to be redrawn all the time
    public int x;
    public int y;
    public TileType type;

    public GoodsType[] localResource;
    public HexTile_Tnfo(int x, int y, TileType type)
    {
        this.x = x;
        this.y = y;
        this.type = type;
        this.haschanged = true;
        this.localResource = new GoodsType[0];  
    }

    public HexTile_Tnfo(int x, int y, TileType type, GoodsType good1)
    {
        this.x = x;
        this.y = y;
        this.type = type;
        this.haschanged = true;
        this.localResource = new GoodsType[] { good1 };
    }

    public HexTile_Tnfo(int x, int y, TileType type, GoodsType good1, GoodsType good2)
    {
        this.x = x;
        this.y = y;
        this.type = type;
        this.haschanged = true;
        this.localResource = new GoodsType[] { good1, good2 };
    }

    public HexTile_Tnfo(int x, int y, TileType type, GoodsType good1, GoodsType good2, GoodsType good3)
    {
        this.x = x;
        this.y = y;
        this.type = type;
        this.haschanged = true;
        this.localResource = new GoodsType[] { good1, good2, good3 };
    }
    public override string ToString()
    {
        string goods = localResource != null && localResource.Length > 0
            ? string.Join(", ", localResource)
            : "None";
        return $"HexTile ({x}, {y})\nType: {type}\nGoods: {goods}";
    }
}