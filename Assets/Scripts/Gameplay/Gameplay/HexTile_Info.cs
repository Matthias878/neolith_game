using UnityEngine;

[System.Serializable]
public class HexTile_Info
{
    private Soil soil; //

    private int roughness; //Berg, Hügel, Flachland 1-100, average height?

    private int süßwassergrad; //Misching aus kleineren Flüssen und grundwasserspiegel, speicherfähighkeit des bodens 1-100

    private int temperatur; //Klimazone 1-100

    private Terrain Terrain = Terrain.Grass;

    public Soil SoilType => soil;
    public int Roughness => roughness;
    public int Suesswassergrad => süßwassergrad;
    public int Temperatur => temperatur;
    public Terrain TileTerrain => Terrain;
    public bool IsForest => Forest;

    //Führt zu snowy mountains and desert, swamp? jungle plains etc... und moore! (entwässerung mittelalter)

    private bool Forest;
    private bool hasForest()
    {
        if (süßwassergrad > 33)
        {
            if (temperatur > 10 && temperatur < 90)
            {
                if (roughness < 85)
                {
                    switch (soil)
                    {
                        case Soil.Lössböden: return true;
                        case Soil.Schwemmböden: return true;
                        case Soil.Schwarzerde: return true;
                        case Soil.Braunerde: if (süßwassergrad > 50) return true; break;
                        case Soil.Kalksteinböden: if (süßwassergrad > 50) return true; break;
                        case Soil.Sandböden: if (süßwassergrad > 70 && temperatur > 30 && temperatur < 70) return true; break;
                        case Soil.Podsolböden: if (süßwassergrad > 40 && temperatur > 20 && temperatur < 70 && roughness < 70) return true; break;
                    }
                }
            }
        }
        return false;
    }

    private Terrain generateTerrain()
    {
        // Generate terrain based on the tile's properties 

        //ideally with hexagon picture
        return Terrain.Grassland; // Placeholder, implement actual terrain generation logic
    }   

    public bool haschanged; //so the hex doesn't have to be redrawn all the time
    public int x;
    public int y;
    public TileType type;

    public GoodsType[] localResource;
    

    // Random generator for soil, roughness, wassergrad, temperatur
    private static System.Random rng = new System.Random();

    private void Initialize(int x, int y, TileType type, GoodsType[] goods)
    {
        this.x = x;
        this.y = y;
        this.type = type;
        this.haschanged = true;
        this.localResource = goods ?? new GoodsType[0];

        // Randomize soil
        this.soil = (Soil)rng.Next(System.Enum.GetValues(typeof(Soil)).Length);
        // Randomize roughness, süßwassergrad, temperatur (1-100)
        this.roughness = rng.Next(1, 101);
        this.süßwassergrad = rng.Next(1, 101);
        this.temperatur = rng.Next(1, 101);

        // Calculate forest presence
        this.Forest = hasForest();
    }

    public HexTile_Info(int x, int y, TileType type)
    {
        Initialize(x, y, type, null);
    }

    public HexTile_Info(int x, int y, TileType type, GoodsType good1)
    {
        Initialize(x, y, type, new GoodsType[] { good1 });
    }

    public HexTile_Info(int x, int y, TileType type, GoodsType good1, GoodsType good2)
    {
        Initialize(x, y, type, new GoodsType[] { good1, good2 });
    }

    public HexTile_Info(int x, int y, TileType type, GoodsType good1, GoodsType good2, GoodsType good3)
    {
        Initialize(x, y, type, new GoodsType[] { good1, good2, good3 });
    }

    public override string ToString()
    {
        string goods = localResource != null && localResource.Length > 0
            ? string.Join(", ", localResource)
            : "None";
        return
            $"X: {x}\n" +
            $"Y: {y}\n" +
            $"Type: {type}\n" +
            $"Soil: {soil}\n" +
            $"Roughness: {roughness}\n" +
            $"Süßwassergrad: {süßwassergrad}\n" +
            $"Temperatur: {temperatur}\n" +
            $"Forest: {Forest}\n" +
            $"HasChanged: {haschanged}\n" +
            $"Goods: {goods}";
    }
}
public enum TileType
{
    River,
    Sea,
    Land,
    Canoyon,
    Out_Of_Map,
    //Neben Vulkanen?

    //minimaler dünger

    //in heißen gebieten mit künstlicher Bewässerung können felder versalzen (Drainage)
    //nach brand kurzzeitig fruchtbar aber dann schnell wieder unfruchtbar Nadelwald

}
public enum Soil
{
    Lössböden, // sehr nährstoffreich 10 - ohne brache/fruchtwechsel wird der boden schlechter, von eiszeit
    Schwemmböden, // sehr nährstoffreich 9 überschwemmungsrisikio aber selbstregenerierend fruchtbarr zweifach ernte später im jahr?
    Schwarzerde, // sehr nährstoffreich 9 ist lange fruchtbar zu trocken -> humus wird abgetragen risiko trockenheit dort wo kein wald wächst aber trotzdem fruchtbar humus
    Braunerde, // mittel 6
    Kalksteinböden, // mittel 5 kalkhaltig, gut für weinbau? 4?
    Sandböden, // schlecht 3 leicht zu bearbeiten verschlechterung der ernte mit der zeit
    Podsolböden,//schlecht 2 oft nadelwald

    //Salzboden, // schlecht 0 nur weideland, entsteht durch mechanik
}
public enum Terrain //Vegetation? Flora Fauna? 
{
    Field,
    Grass,    
    Grassland,
    Forest,
    Mountain,
    Desert,
    Swamp,
    Tundra,
    Taiga,
    Savanna,
    Rainforest,
    Jungle,
    Steppe,
    Marsh,
    Wetland,
    Hills,
    Plateau,
    Valley,
    Volcano,
    Glacier,
    Beach,
    Cliff,
    Canyon,
    Oasis,
    Mangrove,
    Floodplain,
    Delta,
    Badlands,
    Heath,
    Shrubland,
    Prairie,
    Meadow,
    Barren,
    Urban,
    Farmland,
    Vineyard,
    SaltFlat,
    CoralReef,
    Lagoon,
    Fjord,
    Cave,
    Grove,
    PineForest,
    BirchForest,
    MixedForest,
    BambooForest,
    MangroveForest,
    Rocky,
    Crater,
    Sinkhole
}