using UnityEngine;

public class EntityRenderer : MonoBehaviour
{
    public Transform entityParent;
    public string settlerSpriteName = "Settlers"; // Without .png extension
    public float tileWidth = 1f;
    public float tileHeight = 0.866f;
    public HexTile_Tnfo[][] gameMap;
    public Game_Entity[] movables;

    void Update()
    {
        RenderEntities();
    }

    public void SetGameMap(HexTile_Tnfo[][] newMap)
    {
        gameMap = newMap;
    }

    public void SetEntities(Game_Entity[] entities)
    {
        movables = entities;
    }

    void RenderEntities()
    {
        if (entityParent == null || gameMap == null || movables == null)
            return;

        // Clear previous entity sprites
        foreach (Transform child in entityParent)
        {
            Destroy(child.gameObject);
        }

        foreach (var entity in movables)
        {
            if (entity is Settler_Unit)
            {
                // Find the tile position
                int x = entity.x;
                int y = entity.y;
                if (x < 0 || x >= gameMap.Length || y < 0 || y >= gameMap[x].Length)
                    continue;

                // Create sprite
                Sprite sprite = Resources.Load<Sprite>("Sprites/" + settlerSpriteName);
                if (sprite == null)
                {
                    Debug.LogWarning($"Failed to load sprite: {settlerSpriteName} at ({x},{y})");
                    continue;
                }
                GameObject go = new GameObject($"Settler_{x}_{y}");
                go.transform.parent = entityParent;
                var sr = go.AddComponent<SpriteRenderer>();
                sr.sprite = sprite;

                // Scale the GameObject so the sprite fits tileWidth and tileHeight
                float scaleX = 1f, scaleY = 1f;
                var bounds = sprite.bounds.size;
                scaleX = tileWidth / bounds.x;
                scaleY = tileHeight / bounds.y;
                go.transform.localScale = new Vector3(scaleX, scaleY, 1f);

                // Flat-top hex positioning (match TilemapRenderer)
                float xOffset = tileWidth * 0.75f * x;
                float yOffset = tileHeight * y + (x % 2 == 0 ? 0 : tileHeight / 2f);
                go.transform.localPosition = new Vector3(xOffset, yOffset, 0);
            }
        }
    }
}
