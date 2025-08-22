using System.Linq;
using UnityEngine;

[System.Serializable]
public class HexTile_Info
{
    public bool haschanged_render=true; //so the hex doesn't have to be redrawn all the time
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

    private bool Forest ;


    private Terrain generateTerrain()
    {// Generate terrain based on the tile's properties //ideally with hexagon picture
        return Terrain.Grassland; // Placeholder, implement actual terrain generation logic
    }   

    public int x;
    public int y;
    public TileType type;

    public GoodsType[] localResource;

    //Constructors------
        private static System.Random rng = new System.Random();  // Random generator for soil, roughness, wassergrad, temperatur

        public HexTile_Info(int x, int y, TileType type, GoodsType[] goods)
            {
            this.x = x;
            this.y = y;
            this.type = type;
            this.localResource = goods;
            if (goods == null || goods.Length == 0)
            {
                var values = System.Enum.GetValues(typeof(GoodsType));
                this.localResource = Enumerable.Range(0, 3)
                    .Select(_ => (GoodsType)values.GetValue(rng.Next(values.Length)))
                    .Distinct()
                    .Take(3)
                    .ToArray();
            }

            // Randomize soil
            this.soil = (Soil)rng.Next(System.Enum.GetValues(typeof(Soil)).Length);
            // Randomize roughness, süßwassergrad, temperatur (1-100)
            this.roughness = rng.Next(1, 101);
            this.süßwassergrad = rng.Next(1, 101);
            this.temperatur = rng.Next(1, 101);

            // Calculate forest presence
            this.Forest = hasForest();
            if (this.Forest)
                localResource = localResource.Append(GoodsType.Wood).ToArray();
        }

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

    //------
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
            $"HasChanged: {haschanged_render}\n" +
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


//all neighbouring hexes that can produce the same goods are one field (up to certain size)
public enum GoodsType
{
    Flax,
    //Flax needs fertile, well-drained soil, best in moderate, temperate climates with sufficient rainfall. 
    //It struggled in: Very dry zones (too little water), Cold northern areas (short growing seasons), Poor, rocky soils (low fertility).
    //Can be turned into oil and textiles.
    Clay,
    //very abundant in river valleys and deltas.
    //Can be turned into pottery and bricks.
    Timber,
    //Too heavy for long-distance transport.
    //Can be turned into buildings, fuel (cooking, heating, pottery), boats.
    Obsidian,
    //Can be turned into tools for art.
    Flint,
    //Can be turned into tools and weapons.
    Shells,
    //Can be turned into jewelry.
    Ochre,
    // Common but some high-quality sources were traded.
    // Used as pigment for art, body paint, and rituals.
    Malachite_azurite,
    // Copper minerals, rarer and traded.
    // Used as pigments; later as copper ores.
    Manganese_oxides,
    // Less common mineral pigments, sometimes traded.
    // Used for black/purple pigments in art and ritual.
    Hematite_limonite,
    // Iron oxides; traded when high-quality.
    // Used as red/yellow pigments, symbolic items.
    Salt,
    //Can be turned into seasoning and preservation.
    Wood,
}
