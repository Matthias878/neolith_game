using UnityEngine;

public class old_TilemapRenderer : MonoBehaviour
{
    public Transform tileParent;
    public float tileWidth = 1f;
    public float tileHeight = 0.866f; // for flat-top hexes, height = width * sqrt(3)/2

    //Show tiles and units above it

    public void RenderMap(HexTile_Tnfo[][] gameMap, Game_Entity[] movables)
    {
        // Clear previous tiles
        foreach (Transform child in tileParent)
        {
            Destroy(child.gameObject);
        }

        for (int y = 0; y < gameMap.Length; y++)
        {
            for (int x = 0; x < gameMap[y].Length; x++)
            {
                HexTile_Tnfo tile = gameMap[y][x];
                string spriteName = tile.type.ToString();
                Sprite sprite = Resources.Load<Sprite>("Sprites/" + spriteName);
                if (sprite == null)
                {
                    Debug.LogWarning("Failed to load sprite: " + spriteName + " coordinates: (" + x + ", " + y + ")");
                    continue;
                }

                GameObject go = new GameObject($"Tile_{x}_{y}");
                go.transform.parent = tileParent;
                go.layer = LayerMask.NameToLayer("Base_Tile_Layer");
                var sr = go.AddComponent<SpriteRenderer>();
                sr.sprite = sprite;

                // Scale the GameObject so the sprite fits tileWidth and tileHeight
                float scaleX = 1f, scaleY = 1f;
                if (sprite != null)
                {
                    var bounds = sprite.bounds.size;
                    scaleX = tileWidth / bounds.x;
                    scaleY = tileHeight / bounds.y;
                    go.transform.localScale = new Vector3(scaleX, scaleY, 1f);
                }

                // Add a BoxCollider2D for raycasting (for 2D hexes)
                var poly = go.AddComponent<PolygonCollider2D>();
                poly.isTrigger = true;          // hover only
                // no manual size/offset; let the sprite physics shape + scaling do the work

                // Attach HexTileComponent and assign info
                var hexComp = go.AddComponent<HexTileComponent>();
                hexComp.hexTileInfo = tile;

                // Flat-top hex positioning
                float xOffset = tileWidth * 0.75f * x;
                float yOffset = tileHeight * y + (x % 2 == 0 ? 0 : tileHeight / 2f);
                go.transform.localPosition = new Vector3(xOffset, yOffset, 0);

                // Check for Settler_Unit on this tile
                if (movables != null)
                {
                    foreach (var entity in movables)
                    {
                        if (entity != null && entity.x == x && entity.y == y && entity.GetType().Name == "Settler_Unit")
                        {
                            Sprite settlerSprite = Resources.Load<Sprite>("Sprites/Settler");
                            if (settlerSprite != null)
                            {
                                GameObject settlerGO = new GameObject($"Settler_{x}_{y}");
                                settlerGO.transform.parent = tileParent;
                                settlerGO.layer = LayerMask.NameToLayer("Base_Tile_Layer"); //Game_Entity_Layer
                                var settlerSR = settlerGO.AddComponent<SpriteRenderer>();
                                settlerSR.sprite = settlerSprite;
                                settlerSR.sortingOrder = sr.sortingOrder + 1; // Draw above tile
                                settlerGO.transform.localScale = go.transform.localScale;
                                settlerGO.transform.localPosition = new Vector3(xOffset, yOffset, 0);

                                // Add a BoxCollider2D for selection/clicking and fit it to the sprite
                                var settlerCollider = settlerGO.AddComponent<BoxCollider2D>();
                                settlerCollider.isTrigger = false;
                                // Fit collider to sprite bounds (in local space)
                                if (settlerSprite != null)
                                {
                                    var bounds = settlerSprite.bounds;
                                    settlerCollider.size = bounds.size;
                                    settlerCollider.offset = bounds.center;
                                }

                                // Attach Game_Entity_Component script as a component
                                var gameEntityComp = settlerGO.AddComponent<Game_Entity_Component>();
                                gameEntityComp.game_Entity = entity;
                                // Confirm attachment (for debug)
                                Debug.Log($"Settler GameObject created and Game_Entity_Component attached at ({x},{y})");
                            }
                            else
                            {
                                Debug.LogWarning("Failed to load Settler sprite at (" + x + ", " + y + ")");
                            }
                        }
                    }
                }
            }
        }
    }
}
