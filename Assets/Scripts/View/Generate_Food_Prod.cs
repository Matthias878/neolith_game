using UnityEngine;
using TMPro;
using System.Collections.Generic; // For List<T>
using System; // F

public class GenerateFoodProd : MonoBehaviour
{ 
    // No longer using TextMeshProUGUI, will create TextMeshPro objects in world space
    private GameObject[] show_food = new GameObject[0];

    public void Show_food_prod_on_map(Controller controller)
    {
        HexTile_Info[][] map = controller.GameMap;

        for (int y = 0; y < map.Length; y++)
        {
            for (int x = 0; x < map[y].Length; x++)
            {
                float tileWidth = 1f; //from my_tilemaprenderer
                float tileHeight = 0.866f;
                // Create a new GameObject for the text
                GameObject textObj = new GameObject($"FoodText_{x}_{y}");
                textObj.transform.parent = this.transform;
                TextMeshPro tmp = textObj.AddComponent<TextMeshPro>();
                int va = Settlement.calculateFoodProduction_helper(map[y][x]);
                tmp.text = $"{va}";
                tmp.fontSize = 2;
                int minval = 8;
                int maxval = 1000;
                int absoMaxval = 4000;
                if (va <= maxval)
                {
                    // Interpolate from red (minval) to green (maxval)
                    float t = Mathf.InverseLerp(minval, maxval, va);
                    tmp.color = Color.Lerp(Color.red, Color.green, t); //tweak cause most things are red
                }
                else
                {
                    // Interpolate from green (maxval) to blue (absoMaxval)
                    float t = Mathf.InverseLerp(maxval, absoMaxval, va);
                    tmp.color = Color.Lerp(Color.green, Color.blue, t);
                }
                tmp.alignment = TextAlignmentOptions.Center;
                tmp.sortingOrder = 10; // Ensure it's visible above sprites
                float xOffset = tileWidth * 0.75f * x;
                float yOffset = tileHeight * y + (x % 2 == 0 ? 0 : tileHeight / 2f);
                textObj.transform.position = new Vector3(xOffset, yOffset, 0f);
                // Rotate 180 degrees around Y so text faces the camera
                textObj.transform.rotation = Quaternion.Euler(0, 180, 0);
                Array.Resize(ref show_food, show_food.Length + 1);
                show_food[show_food.Length - 1] = textObj;
            }
        }
    }

    public void deactivate_food_prod()
    {
        foreach (var food in show_food)
        {
            GameObject.Destroy(food);
        }
        show_food = new GameObject[0];
    }
}