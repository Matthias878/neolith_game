using UnityEngine.UI;
using TMPro;
using System.Linq;
using System.Collections.Generic;
public class Settlement : Game_Entity
{

    public Settlement(int x, int y) : base(4, x, y)
    {
    }

    private int foodProducedeachTurn;

    private int foodConsumedeachTurn;

    private int foodStoredeachTurn;

    private int maxfoodStorable;

    private int foodstoredcurrently;

    private int influenceRange = 2;

    public override void presentActions_and_Data()
    {
        TextMeshProUGUI myUItext = Controller_GameData.inputManagerController.addUIText();
        foodProducedeachTurn = calculateFoodProduction();
        myUItext.text = "This turn the settlement is producing " + foodProducedeachTurn + " food.";
        
        TextMeshProUGUI Ressource_UI = Controller_GameData.inputManagerController.addUIText();
        List<(Ressourcen, int)> resources = giveResources();
        Ressource_UI.text = "";
        foreach (var resource in resources)
        {
            Ressource_UI.text += "This settlement is able to produce " + resource.Item1 + " at a cost of " + resource.Item2 + ".\n";
        }
            Ressource_UI.text += getHoodstype(resources);

    }

    //Technology and such
    private int calculateFoodProduction()
    {
        HexTile_Info[][] map = Controller_GameData.GameMap;
        int ret = 0;
        
        ret += calculateFoodProduction_helper(map[x + 1][y]);
        ret += calculateFoodProduction_helper(map[x - 1][y]);
        ret += calculateFoodProduction_helper(map[x][y + 1]);
        ret += calculateFoodProduction_helper(map[x][y - 1]);

        if (y % 2 == 0) {
            ret += calculateFoodProduction_helper(map[x - 1][y + 1]);
            ret += calculateFoodProduction_helper(map[x - 1][y - 1]);
        } else {
            ret += calculateFoodProduction_helper(map[x + 1][y + 1]);
            ret += calculateFoodProduction_helper(map[x + 1][y - 1]);   
        }
        if (influenceRange > 1)
        {
            //TODO add malus for distance
        }
        return ret;
    }

    public static int calculateFoodProduction_helper(HexTile_Info field)
    {
        int ret = 0;
        if (field == null) return 0;


        // 0 if no water

        //ignore temp already handled in soil and terrain devide by zero?

        //ret = field.Suesswassergrad / field.Roughness; //value water more and make roughness count more in the extreme?
        ret = (field.Suesswassergrad + field.Roughness - 1) / field.Roughness;//is simple division to round up

        switch (field.SoilType)
        {
            case Soil.Lössböden: ret *= 10; break;
            case Soil.Schwemmböden: ret *= 9; break;
            case Soil.Schwarzerde: ret *= 9; break;
            case Soil.Braunerde: ret *= 6; break;
            case Soil.Kalksteinböden: ret *= 4; break;
            case Soil.Sandböden: ret *= 3; break;
            case Soil.Podsolböden: ret *= 2; break;
        }

        //Terrain Field *1, Grass *0.4, unworked(jungle) *0.1?
        switch (field.TileTerrain)
        {
            case Terrain.Field:
                ret *= 10;
                break;
            case Terrain.Grass:
                ret *= 4;
                break;
            default:
                ret *= 1;
                break;
        }
        return ret;
    }

    private int calculateFoodConsumption()
    {
        //TODO: Implement food consumption calculation
        return 0;
    }

    private int calculateFoodStoredeachTurn()
    {
        //TODO: Implement food storage calculation
        return 0;
    }

    private List<(Ressourcen, int)> giveResources()
    {
        List<(Ressourcen, int)> re = new List<(Ressourcen, int)>();
        HexTile_Info[][] map = Controller_GameData.GameMap;

        re.AddRange(giveResources_helper(map[x + 1][y]));
        re.AddRange(giveResources_helper(map[x - 1][y]));
        re.AddRange(giveResources_helper(map[x][y + 1]));
        re.AddRange(giveResources_helper(map[x][y - 1]));

        if (y % 2 == 0) {
            re.AddRange(giveResources_helper(map[x - 1][y + 1]));
            re.AddRange(giveResources_helper(map[x - 1][y - 1]));
        } else {
            re.AddRange(giveResources_helper(map[x + 1][y + 1]));
            re.AddRange(giveResources_helper(map[x + 1][y - 1]));
        }
        //TODO welche Ressourcen selber + abbauschwierigkeitsgrad(wie viel Arbeit braucht es?)
        //TODO welche Ressourcen erhandelbar zu welchem Preis
        //TODO welche Ressourcen exportierbar zum besten Preis

        //Ressources potentially available: Lehm braucht lehmvorkommen und wenig arbeitsaufwand,Lehmziegel braucht lehmvorkommen, Hitze, Stroh? und mittel arbeitsaufwand, Ton braucht lehmvorkommen, wasser und mittel arbeitsaufwand, Tonziegel/Keramik braucht Ton,brennmaterial, und viel arbeitsaufwand
        //Stroh braucht Getreideanbau und wenig arbeitsaufwand, Schilf braucht Vorkommen und wenig arbeitsaufwand, Holz braucht Wald und extremen arbeitsaufwand?, kleinee steine braucht nichts und wenig arbeitsaufwand, mittlere_große steine brauchen Vorkommen und viel arbeitsaufwand
        //Kalk braucht Kalkböden/Muscheln und mittleren Arbeitsaufwand, Kalkmörtel braucht Kalkböden/Muscheln, Brennmaterial und viel Arbeitsaufwand, Flachs braucht Flachsanbau und hohen Arbeitsaaufwans, Colors braucht Vorkommen und verschiedenen Arbeitsaufwand
        //Verzierung braucht MuschelnPerlen,Knochen/Zähne/iwelche ressourcen und viel Arbeitsaufwand, Bitumen braucht Vorkommen und viel Arbeitsaufwand, Harz braucht Wald und mittleren Aufwand, Asche braucht Vegetation? und wenig Arbeitsaufwand, Tierprodukte TODO

        //Leder/Felle → Tiere, Gerben (viel Aufwand). //Wolle/Haar → Schafe/Ziegen, Spinnen, Weben (hoch). //Knochen/Horn/Geweih → Jagd/Nutztier, mittel Aufwand (Bearbeitung). //Sehnen/Darm → Tiere, Aufbereitung (mittel), wichtig für Bindungen.

        //TODO add tradable ressources, replace if cheaper to produce

        //TODO: Implement resource giving logic
        return re;
    }

    private List<(Ressourcen, int)> giveResources_helper(HexTile_Info tile)
    {
        //z.b lehm existiert lehmvorkommen + (100-roughness) * 1(arbeitsaufwand)
        //z.b holz existiert wood + (100-roughness) * 4(arbeitsaufwand)
        //z.b stroh existiert entityfarm.getreide pauschale LATER
        List<(Ressourcen, int)> ret = new List<(Ressourcen, int)>();
        foreach (GoodsType good in tile.localResource)
        {
            if (good == GoodsType.Wood)
            {
                // Calculate wood resources based on tile properties
                int amount = (tile.Roughness) * 4; // Example calculation
                ret.Add((Ressourcen.Holz, amount));
            }
            else if (good == GoodsType.Clay)
            {
                // Calculate Lehm resources based on tile properties
                int amount = (tile.Roughness) * 1; // Example calculation
                ret.Add((Ressourcen.Lehm, amount));
            }
        }
        return ret;
    }

    private string getHoodstype(List<(Ressourcen, int)> resources)
    {
        bool hasWood = resources.Any(r => r.Item1 == Ressourcen.Holz);
        bool hasClay = resources.Any(r => r.Item1 == Ressourcen.Lehm);
        bool hasStraw = resources.Any(r => r.Item1 == Ressourcen.Stroh);
        if (hasWood) {
            return "because the settlement access to wood, the houses are built from wood";
        }else if (hasClay && hasStraw) {
            return "because the settlement has access to clay and straw, the houses are built from clay";
        } else  {
            return "this settlement does not have enough resources to build houses on its own, it must import them.";
        }
    } 


    //Der Bau von Häusern ist wie billig
    //Fundament:
    //Hauswände:
    //Dach:Mitteleuropa: Satteldach, mit Stroh/Schilf gedeckt.
    //Mittelmeer: Flachdächer oder Kuppeldächer aus Steinplatten/Lehm.
    //Atlantik: dicke Torfschichten oder Steinplatten gegen Sturm.
    //Inneneinrichtung (Putz, Herd, Mühlstein):
    //kulturelle präferenzen (Rundhäuser vorallem Lehm-stein, individual, gut gegen Wind, billiger im Neubau, Langhäuser Holzpfosten, Vielseitiger nutzbar-Viehstall-einteilbar, erweiterbar, gemeinschaftlich), kleinere Rechteckhäuser?
    //Welche Art von Häusern werden gebaut (für billige -> was ist vorhanden und was ist kultur) (für teure -> was ist irgendiwie zu bekommen?/ was haben andere von hohem Standard)
    //TODO
    //2. Baumaterialien
    //
    //Holzreiches Mitteleuropa: Pfosten, Balken, Lehmwände, Strohdächer//.
    ////
    //Steinreiche Insel- und Küstenregionen: Trockenmauerwerk, Megalithblöcke//.
    ////
    //Tonreiche Gebiete (Balkan, Anatolien): gebrannte Ziegel, Lehmziegel.


    // Implement abstract members from Game_Entity:
    public override void move_to(UnityEngine.Vector2Int endpos)
    {
        // TODO: Implement move with Vector2Int for Settlement
    }

    public override void move_starter()
    {
        // TODO: Implement move for Settlement
    }

    public override void Turnend()
    {
        // TODO: Implement Turnend for Settlement
    }


}


//directly control walls, major infrastructure, places, religious sites?

//population <1000 

//Gebäude pallisaden, nahrungsspeicher, wohnhäuser/langhäuser

//hamlets (like Jericho or Çatalhöyük

//Ackerbau und Viehzucht

//Monumentalbauten (z. B. Megalithgräber) oft rund oder oval, da Gewölbe/Kranztechnik mit schweren Steinen einfacher rund zu schließen ist.
//Spezialgebäude: Grubenhäuser?, Vorratslager, Werkststtbauten?, Tempel, Gräber, Grabenwerke, Gemeinschaftsmonumente

public enum Ressourcen
{
    Lehm, //focus on
    Holz, //focus on
    Stroh,
    Lehmziegel,
    Ton,
    Tonziegel,
    Schilf,
    KleineSteine,
    MittlereGroßeSteine,
    Kalk,
    Kalkmörtel,
    Flachs,
    Farben,
    Verzierung,
    Bitumen,
    Harz,
    Asche,
    Tierprodukte,
    Sonne_Hitze,
    Wasser

}