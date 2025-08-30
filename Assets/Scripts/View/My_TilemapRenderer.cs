using System.Collections.Generic;
using UnityEngine;

public class My_TilemapRenderer : MonoBehaviour
{
    public Transform tileParent;
    public float tileWidth = 1f; //1f;
    public float tileHeight = 0.866f; // flat-top hexes

    // Registries
    private readonly Dictionary<HexTile_Info, GameObject> _tileGO = new();
    private readonly Dictionary<int, GameObject> _entityGO = new();
    private readonly Dictionary<int, GameObject> _settlementGO = new();

    public void StopRendering(HexTile_Info[][] gameMap, Game_Entity[] movables)
    {
        // tiles
        if (gameMap != null)
        {
            for (int y = 0; y < gameMap.Length; y++)
            {
                var row = gameMap[y];
                if (row == null) continue;
                for (int x = 0; x < row.Length; x++)
                {
                    var t = row[x];
                    if (t == null) continue;
                    if (_tileGO.TryGetValue(t, out var go) && go) DestroySafe(go);
                    _tileGO.Remove(t);
                }
            }
        }

        // entities
        if (movables != null)
        {
            foreach (var e in movables)
            {
                if (e == null) continue;
                if (_entityGO.TryGetValue(e.id, out var go) && go) DestroySafe(go);
                _entityGO.Remove(e.id);
            }
        }
    }

    public void RenderMap(HexTile_Info[][] gameMap)
    {
        for (int x = 0; x < gameMap.Length; x++)
        {
            for (int y = 0; y < gameMap[x].Length; y++)
            {
                HexTile_Info tile = gameMap[x][y];
                if (tile == null) continue;
                string spriteName = tile.terrain.ToString();
                Sprite sprite = Resources.Load<Sprite>("Sprites/" + spriteName);
                if (sprite == null)
                {
                    Debug.LogWarning($"Failed to load tile sprite: {spriteName} at ({x},{y})");
                    continue;
                }

                GameObject go = new GameObject($"Tile_X_Coord_is{x}_AND_Y_Coord_is{y}");
                go.transform.parent = tileParent;
                go.layer = LayerMask.NameToLayer("Base_Tile_Layer");
                var sr = go.AddComponent<SpriteRenderer>();
                sr.sprite = sprite;
                sr.sortingOrder = 0;

                var bounds = sprite.bounds.size;
                float scaleX = tileWidth / bounds.x;
                float scaleY = tileHeight / bounds.y;
                go.transform.localScale = new Vector3(scaleX, scaleY, 1f);

                float xOffset = tileWidth * 0.75f * x;
                float yOffset = tileHeight * y + (x % 2 == 0 ? 0 : tileHeight / 2f);
                go.transform.localPosition = new Vector3(xOffset, yOffset, 0);

                var collider = go.AddComponent<BoxCollider2D>();
                collider.isTrigger = false;
                collider.size = sprite.bounds.size;
                collider.offset = sprite.bounds.center;

                var hexComp = go.AddComponent<HexTileComponent>();
                hexComp.hexTileInfo = tile;

                _tileGO[tile] = go;
            }
        }
    }

    public void RenderEntities(Game_Entity[] movables)
    {
        if (movables == null) return;

        foreach (var entity in movables)
        {
            if (entity == null) continue;
            if (entity.neverRender == true) continue;

            if (_entityGO.TryGetValue(entity.id, out var go) && go) DestroySafe(go);
            _entityGO.Remove(entity.id);

            string spriteName = entity.type;
            Sprite entitySprite = Resources.Load<Sprite>("Sprites/" + spriteName);
            if (entitySprite == null)
            {
                Debug.LogWarning($"Failed to load entity sprite: {spriteName} at ({entity.x},{entity.y})");
                continue;
            }

            float xOffset = tileWidth * 0.75f * entity.x;
            float yOffset = tileHeight * entity.y + (entity.x % 2 == 0 ? 0 : tileHeight / 2f);

            Vector3 scale = new Vector3(
                tileWidth / entitySprite.bounds.size.x,
                tileHeight / entitySprite.bounds.size.y,
                1f
            );

            GameObject entityGO = new GameObject($"{spriteName}_{entity.x}_{entity.y}");
            entityGO.transform.parent = tileParent;
            entityGO.layer = (entity.type == "Footsteps")
                ? LayerMask.NameToLayer("Unit_Movement_Layer")
                : LayerMask.NameToLayer("Base_Tile_Layer");

            var sr = entityGO.AddComponent<SpriteRenderer>();
            sr.sprite = entitySprite;
            sr.sortingOrder = 2;
            entityGO.transform.localScale = scale;
            entityGO.transform.localPosition = new Vector3(xOffset, yOffset, 0f);

            var collider = entityGO.AddComponent<BoxCollider2D>();
            collider.isTrigger = false;
            collider.size = entitySprite.bounds.size;
            collider.offset = entitySprite.bounds.center;

            var comp = entityGO.AddComponent<Game_Entity_Component>();
            comp.game_Entity = entity;

            _entityGO[entity.id] = entityGO;
        }
    }

    public void RenderSettlements(Settlement[] settlements)
    {
        if (settlements == null) return;

        foreach (var settlement in settlements)
        {
            if (settlement == null) continue;
            
            if (_settlementGO.TryGetValue(settlement.id, out var go) && go) DestroySafe(go);
            _settlementGO.Remove(settlement.id);

            Sprite settlementSprite = Resources.Load<Sprite>("Sprites/" + settlement.type);
            if (settlementSprite == null)
            {
                Debug.LogWarning($"Failed to load settlement sprite: {settlement.type} at ({settlement.x},{settlement.y})");
                continue;
            }

            float xOffset = tileWidth * 0.75f * settlement.x;
            float yOffset = tileHeight * settlement.y + (settlement.x % 2 == 0 ? 0 : tileHeight / 2f);

            Vector3 scale = new Vector3(
                tileWidth / settlementSprite.bounds.size.x,
                tileHeight / settlementSprite.bounds.size.y,
                1f
            );

            GameObject settlementGO = new GameObject($"Settlement_{settlement.x}_{settlement.y}");
            settlementGO.transform.parent = tileParent;
            settlementGO.layer = LayerMask.NameToLayer("Base_Tile_Layer");

            var sr = settlementGO.AddComponent<SpriteRenderer>();
            sr.sprite = settlementSprite;
            sr.sortingOrder = 1;
            settlementGO.transform.localScale = scale;
            settlementGO.transform.localPosition = new Vector3(xOffset, yOffset, 0);

            var collider = settlementGO.AddComponent<BoxCollider2D>();
            collider.isTrigger = false;
            collider.size = settlementSprite.bounds.size;
            collider.offset = settlementSprite.bounds.center;

            var comp = settlementGO.AddComponent<Settlement_Component>();
            comp.settlement = settlement;

            _settlementGO[settlement.id] = settlementGO;
        }
    }

    // Optional helpers
    public void ClearAll()
    {
        foreach (var kv in _tileGO) if (kv.Value) DestroySafe(kv.Value);
        foreach (var kv in _entityGO) if (kv.Value) DestroySafe(kv.Value);
        foreach (var kv in _settlementGO) if (kv.Value) DestroySafe(kv.Value);
        _tileGO.Clear(); _entityGO.Clear(); _settlementGO.Clear();
    }

    private static void DestroySafe(GameObject go)
    {
#if UNITY_EDITOR
        if (!Application.isPlaying) Object.DestroyImmediate(go);
        else Object.Destroy(go);
#else
        Object.Destroy(go);
#endif
    }
}

public class HexTileComponent : MonoBehaviour { public HexTile_Info hexTileInfo; }
public class Game_Entity_Component : MonoBehaviour { public Game_Entity game_Entity; }
public class Settlement_Component : MonoBehaviour { public Settlement settlement; }
