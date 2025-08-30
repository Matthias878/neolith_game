using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System.Linq;
using System.Collections.Generic;
public class Settlement : Game_Entity
{
    //only public for json, should never be called from outside class otherwise
    public Settlement(int x, int y, Person leader) : base(4, x, y, leader)
    {
        this.foodstoredcurrently = 100;
        this.maxfoodStorable = 1000; //TODO make dependend on size and buildings
    }

    private int foodProducedeachTurn;

    private List<Pop> inhabitants = new List<Pop>();

    private int foodConsumedeachTurn;

    private int foodStoredeachTurn;

    private int maxfoodStorable;

    private int foodstoredcurrently;

    public static void foundSettlement(int x, int y, Person leader)
    {
        var s = new Settlement(x, y, leader);
        var p = new Pop(x, y, leader);
        s.inhabitants.Add(p);
        Controller_GameData.AddEntity(s);
        Controller_GameData.AddEntity(p);
    }

    private int influenceRange = 2;

    public override void presentActions_and_Data()
    {
        //Show food production
        TextMeshProUGUI Ressource_UI = Controller_GameData.inputManagerController.addUIText();
        Ressource_UI.text = "";
        Ressource_UI.text += "This settlement is at x:" + x + " y:" + y + ".\n";
        Ressource_UI.text += "This turn the settlement is producing " + totalFoodProduction() + " food.\n";

        //show ressource production + hood type
        List<(GoodsType, int)> allressources = new List<(GoodsType, int)>();
        foreach (var resourceOwn in totalGoodProductions(this))
        {
            Ressource_UI.text += "This settlement is able to produce " + resourceOwn.Item1 + " at a cost of " + resourceOwn.Item2 + ".\n";
            allressources.Add(resourceOwn);
        }
        //show ressource import
        foreach (var settlement in Controller.movables)
        {
            if (settlement is Settlement s && s != this)
            {
                List<(GoodsType, int)> exportableResources = s.productionExportTo(this);
                foreach (var resourceImp in exportableResources)
                {
                    Ressource_UI.text += "This settlement is able to import " + resourceImp.Item1 + " from the settlement at x:" + s.x + " y:" + s.y + " at a cost of " + resourceImp.Item2 + ".\n";
                    allressources.Add(resourceImp);
                }
            }
        }

        //show settlement size
        int size = 0;
        foreach (var pop in inhabitants)
        {
            size += pop.getSizeAtPosition(new Vector2(x, y));
        }
        Ressource_UI.text += "The settlement has a population of " + size + " people.\n";
        Ressource_UI.text += getHoodstype(allressources);

    }

    private int totalFoodProduction()
    {
        HexTile_Info[][] map = Controller.GameMap;
        int ret = 0;

        //above
        ret += FoodProduction(map[x][y + 1]);
        //below
        ret += FoodProduction(map[x][y - 1]);
        //left
        ret += FoodProduction(map[x - 1][y]);
        //right
        ret += FoodProduction(map[x + 1][y]);

        //left,right depending on even or odd
        if (x % 2 == 0)
        {
            ret += FoodProduction(map[x - 1][y - 1]);
            ret += FoodProduction(map[x + 1][y - 1]);
        }
    
        else
        {    
            ret += FoodProduction(map[x - 1][y + 1]);
            ret += FoodProduction(map[x + 1][y + 1]);
        }
        if (influenceRange > 1)
        {
            //TODO add malus for distance
        }
        return ret;
    }

    public static int FoodProduction(HexTile_Info field)
    {
        int ret = 0;
        if (field == null) return 0;
        //ret = field.Suesswassergrad / field.Roughness; //value water more and make roughness count more in the extreme?
        ret = (field.suesswassergrad + field.roughness - 1) / field.roughness;//is simple division to round up

        switch (field.soil)
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
        switch (field.terrain)
        {
            case Terrain.Field:
                ret *= 10;
                break;
            case Terrain.Grassland:
                ret *= 4;
                break;
            default:
                ret *= 1;
                break;
        }
        return ret;
    }

    private int totalFoodConsumption()
    {
        HexTile_Info[][] map = Controller.GameMap;
        int ret = 0;

        //above
        ret += FoodConsumption(map[x][y + 1]);
        //below
        ret += FoodConsumption(map[x][y - 1]);
        //left
        ret += FoodConsumption(map[x - 1][y]);
        //right
        ret += FoodConsumption(map[x + 1][y]);

        //left,right depending on even or odd
        if (x % 2 == 0)
        {
            ret += FoodConsumption(map[x - 1][y - 1]);
            ret += FoodConsumption(map[x + 1][y - 1]);
        }
    
        else
        {
            ret += FoodConsumption(map[x - 1][y + 1]);
            ret += FoodConsumption(map[x + 1][y + 1]);
        }
        if (influenceRange > 1)
        {
            //TODO add malus for distance
        }
        return ret;
    }

    private int FoodConsumption(HexTile_Info field)
    {
        int v = 0;
        foreach (var entity in Controller.movables)
        {
            if (entity is Pop pop)
            {
                v += pop.getSizeAtPosition(new Vector2(field.x, field.y));
            }
        }
        return v;
    }

    private int calculateFoodStoredeachTurn()
    {
        int a = totalFoodConsumption();
        int b = totalFoodProduction();
        int c = maxfoodStorable;
        int d = foodstoredcurrently;
        if (b - a > 0)
        {
            int e = b - a;
            if (e + d > c)
            {
                foodstoredcurrently = c;
                Debug.Log("The settlement at x:" + x + " y:" + y + " is has excess production of " + (e) + ". but has only " + (c - d) + " food storage left.");
                return c - d;
            }
            else
            {
                foodstoredcurrently += e;
                Debug.Log("The settlement at x:" + x + " y:" + y + " is storing " + (e) + " food.");
                return e;

            }
        }
        else
        {
            Debug.Log("The settlement at x:" + x + " y:" + y + " is consuming more food than it produces and thus starving.");
            return 0;
        }
    }
    public static List<(GoodsType, int)> totalGoodProductions(Settlement settlement)
    {
        List<(GoodsType, int)> re = new List<(GoodsType, int)>();
        HexTile_Info[][] map = Controller.GameMap;

        re.AddRange(GoodProduction(map[settlement.x + 1][settlement.y]));
        re.AddRange(GoodProduction(map[settlement.x - 1][settlement.y]));
        re.AddRange(GoodProduction(map[settlement.x][settlement.y + 1]));
        re.AddRange(GoodProduction(map[settlement.x][settlement.y - 1]));

        if (settlement.y % 2 == 0) {
            re.AddRange(GoodProduction(map[settlement.x - 1][settlement.y + 1]));
            re.AddRange(GoodProduction(map[settlement.x - 1][settlement.y - 1]));
        } else {
            re.AddRange(GoodProduction(map[settlement.x + 1][settlement.y + 1]));
            re.AddRange(GoodProduction(map[settlement.x + 1][settlement.y - 1]));
        }
        return re;
    }

    public List<(GoodsType, int)> productionExportTo(Settlement settlement_to_export_to)
    {
        List<(GoodsType, int)> allressources = new List<(GoodsType, int)>();
        List<(GoodsType, int)> exportableResources = totalGoodProductions(this);
        foreach (var resource in exportableResources)
        {
            Debug.Log("We here at x:" + this.x + " y:" + this.y + " are producing " + resource.Item1 + ".");
            List<(Vector2Int, int)> transportCosts = NeolithianRev.Utility.MovementAlgos.GetCheapestPath(new Vector2Int(this.x, this.y), new Vector2Int(settlement_to_export_to.x, settlement_to_export_to.y), resource.Item1);

            int adjustedAmount = resource.Item2 + transportCosts.Sum(step => step.Item2);
            Debug.Log("We here at x:" + this.x + " y:" + this.y + " are exporting " + resource.Item1 + " to " + settlement_to_export_to.x + " " + settlement_to_export_to.y + " for a total cost of " + adjustedAmount + ".");
            allressources.Add((resource.Item1, adjustedAmount));
        }
        return allressources;
    }

    private static List<(GoodsType, int)> GoodProduction(HexTile_Info tile)
    {
        List<(GoodsType, int)> ret = new List<(GoodsType, int)>();
        foreach (GoodsType good in tile.localResource)
        {
            if (good == GoodsType.Holz)
            {
                // Calculate wood resources based on tile properties
                int amount = (tile.roughness) * 4; // Example calculation
                ret.Add((GoodsType.Holz, amount));
            }
            else if (good == GoodsType.Clay)
            {
                // Calculate Lehm resources based on tile properties
                int amount = (tile.roughness) * 1; // Example calculation
                ret.Add((GoodsType.Lehm, amount));
            }
        }
        return ret;
    }

    private string getHoodstype(List<(GoodsType, int)> resources)
    {
        bool hasWood = resources.Any(r => r.Item1 == GoodsType.Holz);
        bool hasClay = resources.Any(r => r.Item1 == GoodsType.Lehm);
        bool hasStraw = resources.Any(r => r.Item1 == GoodsType.Stroh);
        if (hasWood) {
            return "because the settlement access to wood, the houses are built from wood";
        }else if (hasClay && hasStraw) {
            return "because the settlement has access to clay and straw, the houses are built from clay";
        } else  {
            return "this settlement does not have enough resources to build houses on its own, it must import them.";
        }
    } 

    public override void move_to(int x, int y)
    {
        // TODO: Implement move with Vector2Int for Settlement
    }

    public override void move_starter()
    {
        // TODO: Implement move for Settlement
    }

    public override void Turnend()
    {
        calculateFoodStoredeachTurn();
        //TODO popgrowth, in general and with wood, clay price
    }


}


//directly control walls, major infrastructure, places, religious sites?

//population <1000 

//Gebäude pallisaden, nahrungsspeicher, wohnhäuser/langhäuser

//hamlets (like Jericho or Çatalhöyük

//Ackerbau und Viehzucht

//Monumentalbauten (z. B. Megalithgräber) oft rund oder oval, da Gewölbe/Kranztechnik mit schweren Steinen einfacher rund zu schließen ist.
//Spezialgebäude: Grubenhäuser?, Vorratslager, Werkststtbauten?, Tempel, Gräber, Grabenwerke, Gemeinschaftsmonumente

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

    
        //z.b lehm existiert lehmvorkommen + (100-roughness) * 1(arbeitsaufwand)
        //z.b holz existiert wood + (100-roughness) * 4(arbeitsaufwand)
        //z.b stroh existiert entityfarm.getreide pauschale LATER

        
        //Ressources potentially available: Lehm braucht lehmvorkommen und wenig arbeitsaufwand,Lehmziegel braucht lehmvorkommen, Hitze, Stroh? und mittel arbeitsaufwand, Ton braucht lehmvorkommen, wasser und mittel arbeitsaufwand, Tonziegel/Keramik braucht Ton,brennmaterial, und viel arbeitsaufwand
        //Stroh braucht Getreideanbau und wenig arbeitsaufwand, Schilf braucht Vorkommen und wenig arbeitsaufwand, Holz braucht Wald und extremen arbeitsaufwand?, kleinee steine braucht nichts und wenig arbeitsaufwand, mittlere_große steine brauchen Vorkommen und viel arbeitsaufwand
        //Kalk braucht Kalkböden/Muscheln und mittleren Arbeitsaufwand, Kalkmörtel braucht Kalkböden/Muscheln, Brennmaterial und viel Arbeitsaufwand, Flachs braucht Flachsanbau und hohen Arbeitsaaufwans, Colors braucht Vorkommen und verschiedenen Arbeitsaufwand
        //Verzierung braucht MuschelnPerlen,Knochen/Zähne/iwelche ressourcen und viel Arbeitsaufwand, Bitumen braucht Vorkommen und viel Arbeitsaufwand, Harz braucht Wald und mittleren Aufwand, Asche braucht Vegetation? und wenig Arbeitsaufwand, Tierprodukte TODO

        //Leder/Felle → Tiere, Gerben (viel Aufwand). //Wolle/Haar → Schafe/Ziegen, Spinnen, Weben (hoch). //Knochen/Horn/Geweih → Jagd/Nutztier, mittel Aufwand (Bearbeitung). //Sehnen/Darm → Tiere, Aufbereitung (mittel), wichtig für Bindungen.
