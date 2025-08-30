using System.Linq;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;

[JsonObject(MemberSerialization.Fields)]
public class HexTile_Info
{
    public bool haschanged_render = true; //so the hex doesn't have to be redrawn all the time
    public Soil soil; //

    public int roughness; //Berg, Hügel, Flachland 1-100, average height?

    public int suesswassergrad; //Misching aus kleineren Flüssen und grundwasserspiegel, speicherfähighkeit des bodens 1-100

    public int temperatur; //Klimazone 1-100

    public Terrain terrain;

    public bool IsForest;

    public int x;public int y;
    public bool isWater;
    public GoodsType[] localResource;

    //Constructors------
    private static System.Random rng = new System.Random();  // Random generator for soil, roughness, wassergrad, temperatur

    public HexTile_Info(int y, int x, GoodsType[] goods, bool isWater = false)
    {
        this.x = x;
        this.y = y;
        this.isWater = isWater;
        this.localResource = goods;
        if (goods == null || goods.Length == 0)
        {
            var values = System.Enum.GetValues(typeof(GoodsType));
            this.localResource = Enumerable.Range(0, 3)
                .Select(_ => (GoodsType)values.GetValue(rng.Next(9)))//manual natural ressource size
                .Distinct()
                .Take(3)
                .ToArray();
        }

        // Randomize soil
        this.soil = (Soil)rng.Next(System.Enum.GetValues(typeof(Soil)).Length);
        // Randomize roughness, suesswassergrad, temperatur (1-100)
        this.roughness = rng.Next(1, 101);
        this.suesswassergrad = rng.Next(1, 101);
        this.temperatur = rng.Next(1, 101);

        // Calculate forest presence
        IsForest = hasForest();
        if (IsForest)
            localResource = localResource.Append(GoodsType.Holz).ToArray();
        generateTerrain();
    }

    private bool hasForest()
    {
        if (suesswassergrad > 33)
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
                        case Soil.Braunerde: if (suesswassergrad > 50) return true; break;
                        case Soil.Kalksteinböden: if (suesswassergrad > 50) return true; break;
                        case Soil.Sandböden: if (suesswassergrad > 70 && temperatur > 30 && temperatur < 70) return true; break;
                        case Soil.Podsolböden: if (suesswassergrad > 40 && temperatur > 20 && temperatur < 70 && roughness < 70) return true; break;
                    }
                }
            }
        }
        return false;
    }

    private void generateTerrain()//defines hexagon picture// Generate terrain based on the tile's properties //Führt zu snowy mountains and desert, swamp? jungle plains etc... und moore! (entwässerung mittelalter)
    {
        if (isWater == true)
            terrain = Terrain.River; // Water tiles are rivers for simplicity
        else
            terrain = Terrain.Grassland; // Placeholder, implement actual terrain generation logic
    }
    public override string ToString()
    {
        string goods = localResource != null && localResource.Length > 0
            ? string.Join(", ", localResource)
            : "None";

        string t = "";

        if (isWater == false)
        {
            t = $"Type: Land\n";
        }
        else
        {
            t = $"Type: Water\n";
        }
        return
            $"X: {x}\n" +
            $"Y: {y}\n" +
            $"{t}" +
            $"Soil: {soil}\n" +
            $"Roughness: {roughness}\n" +
            $"Süßwassergrad: {suesswassergrad}\n" +
            $"Temperatur: {temperatur}\n" +
            $"Forest: {IsForest}\n" +
            $"HasChanged: {haschanged_render}\n" +
            $"Goods: {goods}";
    }

    public string ToSerializable()
    {
        return "TODO";
    }
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
    River,
    Sea,
    Field,
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
    Sinkhole,
    Out_Of_Map
}

//all neighbouring hexes that can produce the same goods are one field (up to certain size)
public enum GoodsType //natural resources
{
    Flax,
    //Flax needs fertile, well-drained soil, best in moderate, temperate climates with sufficient rainfall. 
    //It struggled in: Very dry zones (too little water), Cold northern areas (short growing seasons), Poor, rocky soils (low fertility).
    //Can be turned into oil and textiles.
    Clay,
    //very abundant in river valleys and deltas.
    //Can be turned into pottery and bricks.
    //Timber,
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
    //hextile only random UNTIL HERE
    //ressource not natural? goods not directly usable by settlements
    //Wood,
    //Verarbeitetere Produkte:
    Lehm,//Ressource //focus on
    Holz,//Ressource //focus on
    Stroh,//Ressource
    Lehmziegel,//Ressource
    Ton,//Ressource
    Tonziegel,//Ressource
    Schilf,//Ressource
    KleineSteine,//Ressource
    MittlereGroßeSteine,//Ressource
    Kalk,//Ressource
    Kalkmörtel,//Ressource
    Flachs,//Ressource
    Farben,//Ressource
    Verzierung,//Ressource
    Bitumen,//Ressource
    Harz,//Ressource
    Asche,//Ressource
    Tierprodukte,//Ressource
    Sonne_Hitze,//Ressource
    Wasser//Ressource

}
