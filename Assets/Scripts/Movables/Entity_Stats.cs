using System.Collections.Generic;

public static class Entity_Stats
{
    public static readonly Dictionary<TileType, int> Settler_Movement_Costs = new Dictionary<TileType, int>
    {
        { TileType.Plains, 1 },
        { TileType.Forest, 2 },
        { TileType.Mountains, 3 },
        { TileType.Out_Of_Map, 999 },

        // Add more as needed
    };
}