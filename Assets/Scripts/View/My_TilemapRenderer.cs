using UnityEngine;

public class My_TilemapRenderer : MonoBehaviour
{
    public Transform tileParent;
    public float tileWidth = 1f;
    public float tileHeight = 0.866f; // flat-top hexes  //Is used elswhere to calculate worldpos from x and y !

    public void RenderMap(HexTile_Info[][] gameMap, Game_Entity[] movables, Settlement[] settlements = null)
    {
        // Clear previous tiles + units + settlements
        foreach (Transform child in tileParent)
        {
            Destroy(child.gameObject);
        }

        for (int y = 0; y < gameMap.Length; y++)
        {
            for (int x = 0; x < gameMap[y].Length; x++)
            {
                HexTile_Info tile = gameMap[y][x];
                if (tile == null) continue;

                string spriteName = tile.type.ToString();
                Sprite sprite = Resources.Load<Sprite>("Sprites/" + spriteName);
                if (sprite == null)
                {
                    Debug.LogWarning($"Failed to load tile sprite: {spriteName} at ({x},{y})");
                    continue;
                }

                // Create Tile GameObject
                GameObject go = new GameObject($"Tile_X_Coord_is{x}_AND_Y_Coord_is{y}");
                go.transform.parent = tileParent;
                go.layer = LayerMask.NameToLayer("Base_Tile_Layer");
                var sr = go.AddComponent<SpriteRenderer>();
                sr.sprite = sprite;
                sr.sortingOrder = 0;

                // Scale sprite
                var bounds = sprite.bounds.size;
                float scaleX = tileWidth / bounds.x;
                float scaleY = tileHeight / bounds.y;
                go.transform.localScale = new Vector3(scaleX, scaleY, 1f);

                // Flat-top hex positioning
                float xOffset = tileWidth * 0.75f * x;
                float yOffset = tileHeight * y + (x % 2 == 0 ? 0 : tileHeight / 2f);
                go.transform.localPosition = new Vector3(xOffset, yOffset, 0);

                // Add collider
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
                        if (entity != null && entity.x == x && entity.y == y)
                        {
                            CreateEntity(entity, entity.type, x, y, xOffset, yOffset, go.transform.localScale);
                        }
                    }
                }

                // Render settlements
                if (settlements != null)
                {
                    foreach (var settlement in settlements)
                    {
                        if (settlement != null && settlement.x == x && settlement.y == y)
                        {
                            CreateSettlement(settlement, x, y, xOffset, yOffset, go.transform.localScale);
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
        

        if (entity.type == "Footsteps")
            entityGO.layer = LayerMask.NameToLayer("Unit_Movement_Layer");
        else
            entityGO.layer = LayerMask.NameToLayer("Base_Tile_Layer");

        var sr = entityGO.AddComponent<SpriteRenderer>();
        sr.sprite = entitySprite;
        sr.sortingOrder = 2; //only relevant for rendering not clicking i e colliders
        entityGO.transform.localScale = scale;
        entityGO.transform.localPosition = new Vector3(xOffset, yOffset, 0f);

        var collider = entityGO.AddComponent<BoxCollider2D>();
        collider.isTrigger = false;
        collider.size = entitySprite.bounds.size;
        collider.offset = entitySprite.bounds.center;

        var gameEntityComp = entityGO.AddComponent<Game_Entity_Component>();
        gameEntityComp.game_Entity = entity;
    }

    private void CreateSettlement(Settlement settlement, int x, int y,
                                  float xOffset, float yOffset, Vector3 scale)
    {
        Sprite settlementSprite = Resources.Load<Sprite>("Sprites/" + settlement.type);
        if (settlementSprite == null)
        {
            Debug.LogWarning($"Failed to load settlement sprite: {settlement.type} at ({x},{y})");
            return;
        }

        GameObject settlementGO = new GameObject($"Settlement_{x}_{y}");
        settlementGO.transform.parent = tileParent;
        settlementGO.layer = LayerMask.NameToLayer("Base_Tile_Layer");

        var sr = settlementGO.AddComponent<SpriteRenderer>();
        sr.sprite = settlementSprite;
        sr.sortingOrder = 1; // above units
        settlementGO.transform.localScale = scale;
        settlementGO.transform.localPosition = new Vector3(xOffset, yOffset, 0);

        var collider = settlementGO.AddComponent<BoxCollider2D>();
        collider.isTrigger = false;
        collider.size = settlementSprite.bounds.size;
        collider.offset = settlementSprite.bounds.center;

        var settlementComp = settlementGO.AddComponent<Settlement_Component>();
        settlementComp.settlement = settlement;
    }
}

public class HexTileComponent : MonoBehaviour
{
    public HexTile_Info hexTileInfo;
}

public class Game_Entity_Component : MonoBehaviour
{
    public Game_Entity game_Entity;
}

public class Settlement_Component : MonoBehaviour
{
    public Settlement settlement;
}
