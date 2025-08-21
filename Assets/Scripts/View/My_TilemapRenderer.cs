using UnityEngine;

public class My_TilemapRenderer : MonoBehaviour
{
    public Transform tileParent;
    public float tileWidth = 1f;
    public float tileHeight = 0.866f; // flat-top hexes

    public void RenderMap(HexTile_Tnfo[][] gameMap, Game_Entity[] movables)
    {
        // Clear previous tiles + units
        foreach (Transform child in tileParent)
        {
            Destroy(child.gameObject);
        }

        for (int y = 0; y < gameMap.Length; y++)
        {
            for (int x = 0; x < gameMap[y].Length; x++)
            {
                HexTile_Tnfo tile = gameMap[y][x];
                if (tile == null) continue; // only create if not null

                string spriteName = tile.type.ToString();
                Sprite sprite = Resources.Load<Sprite>("Sprites/" + spriteName);
                if (sprite == null)
                {
                    Debug.LogWarning($"Failed to load tile sprite: {spriteName} at ({x},{y})");
                    continue;
                }

                // Create Tile GameObject
                GameObject go = new GameObject($"Tile_{x}_{y}");
                go.transform.parent = tileParent;
                go.layer = LayerMask.NameToLayer("Base_Tile_Layer");
                var sr = go.AddComponent<SpriteRenderer>();
                sr.sprite = sprite;
                sr.sortingOrder = 0; // tiles always under entities

                // Scale sprite to match tile dimensions
                var bounds = sprite.bounds.size;
                float scaleX = tileWidth / bounds.x;
                float scaleY = tileHeight / bounds.y;
                go.transform.localScale = new Vector3(scaleX, scaleY, 1f);

                // Flat-top hex positioning
                float xOffset = tileWidth * 0.75f * x;
                float yOffset = tileHeight * y + (x % 2 == 0 ? 0 : tileHeight / 2f);
                go.transform.localPosition = new Vector3(xOffset, yOffset, 0);

                // Add BoxCollider2D
                var collider = go.AddComponent<BoxCollider2D>();
                collider.isTrigger = false;
                collider.size = sprite.bounds.size;
                collider.offset = sprite.bounds.center;

                // Attach tile info
                var hexComp = go.AddComponent<HexTileComponent>();
                hexComp.hexTileInfo = tile;

                // Render entities
                if (movables != null)
                {
                    foreach (var entity in movables)
                    {
                        if (entity == null) continue;
                        if (entity.x == x && entity.y == y)
                        {
                            // Use entity.type to pick correct sprite
                            CreateEntity(entity, entity.type, x, y, xOffset, yOffset, go.transform.localScale);
                        }
                    }
                }
            }
        }
    }

    private void CreateEntity(Game_Entity entity, string spriteName, int x, int y,
                              float xOffset, float yOffset, Vector3 scale)
    {
        Sprite entitySprite = Resources.Load<Sprite>("Sprites/" + spriteName);
        if (entitySprite == null)
        {
            Debug.LogWarning($"Failed to load {spriteName} sprite at ({x},{y})");
            return;
        }

        GameObject entityGO = new GameObject($"{spriteName}_{x}_{y}");
        entityGO.transform.parent = tileParent;

        // Footsteps get their own layer
        if (entity.type == "Footsteps")
            entityGO.layer = LayerMask.NameToLayer("Unit_Movement_Layer");
        else
            entityGO.layer = LayerMask.NameToLayer("Base_Tile_Layer");

        var sr = entityGO.AddComponent<SpriteRenderer>();
        sr.sprite = entitySprite;
        sr.sortingOrder = 1; // entities always above tiles
        entityGO.transform.localScale = scale;
        entityGO.transform.localPosition = new Vector3(xOffset, yOffset, 0);

        var collider = entityGO.AddComponent<BoxCollider2D>();
        collider.isTrigger = false;
        collider.size = entitySprite.bounds.size;
        collider.offset = entitySprite.bounds.center;

        var gameEntityComp = entityGO.AddComponent<Game_Entity_Component>();
        gameEntityComp.game_Entity = entity;
    }
}

public class HexTileComponent : MonoBehaviour
{
    public HexTile_Tnfo hexTileInfo;
}

public class Game_Entity_Component : MonoBehaviour
{
    public Game_Entity game_Entity;
}
