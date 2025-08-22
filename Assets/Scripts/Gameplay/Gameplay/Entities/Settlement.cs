using UnityEngine.UI;
using TMPro;

public class Settlement : Game_Entity {

    public Settlement(int x, int y) : base(4, x, y)
    {
    }

    private int foodProducedeachTurn;

    private int foodConsumedeachTurn;

    private int foodStoredeachTurn;

    private int maxfoodStorable;

    private int foodstoredcurrently;

    private int range = 2;

    public override void presentActions_and_Data()
    {

        TextMeshProUGUI myUItext = Controller_GameData.inputManagerController.addUIText();
        foodProducedeachTurn = calculateFoodProduction();
        myUItext.text = "This turn the settlement is producing " + foodProducedeachTurn + " food.";

    }

    //Technology and such
    private int calculateFoodProduction()
    {
        HexTile_Info[][] map = Controller_GameData.GameMap;
        int ret = 0;
        ret += calculateFoodProduction_helper(map[x + 1][y]);
        ret += calculateFoodProduction_helper(map[x + 1][y + 1]);
        ret += calculateFoodProduction_helper(map[x + 1][y - 1]);
        ret += calculateFoodProduction_helper(map[x][y + 1]);
        ret += calculateFoodProduction_helper(map[x][y - 1]);
        ret += calculateFoodProduction_helper(map[x - 1][y]);
        if (range > 1)
        {
            //TODO add malus for distance
        }
        //Controller_GameData.
        //TODO: Implement food production calculation
        //include range
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

    public string Name { get; set; }
    public int Population { get; set; }
    public HexTile_Info Location { get; set; }

    public void showProduction()
    {
        //TODO
        return;
    }
    // Implement abstract members from Game_Entity

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


//Gebäude pallisaden, nahrungsspeicher, wohnhäuser/langhäuser