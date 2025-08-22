using System.Collections.Generic;

public static class Entity_Stats
{
    public static readonly Dictionary<TileType, int> Settler_Movement_Costs = new Dictionary<TileType, int>
    {
        { TileType.Land, 1 },
        { TileType.River, 2 },
        { TileType.Sea, 3 },
        { TileType.Out_Of_Map, 999 },

        // Add more as needed
    };
}