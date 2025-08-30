using System.Collections.Generic;

public static class Entity_Stats
{
    //movement costs units
    public static readonly Dictionary<Terrain, int> Settler_Movement_Costs = new Dictionary<Terrain, int>
    {
        { Terrain.Grassland, 1 },
        { Terrain.River, 2 },
        { Terrain.Sea, 3 },
        { Terrain.Out_Of_Map, 999 },

        // Add more as needed
    };
    //movement costs goods/resources
    public static readonly Dictionary<GoodsType, float> Goods_Movement_Costs_Factor = new Dictionary<GoodsType, float>
    {
        { GoodsType.Flax, 1.0f },
        { GoodsType.Holz, 4.0f },
        { GoodsType.Lehm, 2.0f },
        // Add more as needed
    };

    
}