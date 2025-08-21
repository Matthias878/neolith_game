using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class TileHoverUI : MonoBehaviour
{
    public TextMeshProUGUI infoText;
    public string tileLayerName = "Base_Tile_Layer";
    private Camera cam; //Camera is on movable on x,y plane with z = 10 (ratation is x= 0 y= 180 z= 0) projection is orthographic with size changable by scrolling

    int _tileLayer;
    int _tileMask;
    GameObject _lastHit;

    void Awake()
    {
        cam = Camera.main;

        _tileLayer = LayerMask.NameToLayer(tileLayerName);
        if (_tileLayer < 0)
        {
            _tileLayer = LayerMask.NameToLayer("Default");
        }
        _tileMask = 1 << _tileLayer;
    }

    void Start()
    {
    }

    void Update()
    {
        if (cam == null || infoText == null) {
            return;
        }
        if (!TryGetPointerScreenPosition(out Vector2 screenPos))
        {
            ClearIfChanged();
            return;
        }
            // Ray from camera through screen point
            Ray ray = cam.ScreenPointToRay(screenPos);
            float targetZ = -0.5f;
            float t = (targetZ - ray.origin.z) / ray.direction.z; // intersection with z = -0.5
            Vector3 targetPoint = ray.origin + ray.direction * t;
            Vector2 rayOrigin2D = new Vector2(ray.origin.x, ray.origin.y);
            Vector2 rayDir2D = new Vector2(ray.direction.x, ray.direction.y).normalized;
            float maxDistance = Vector2.Distance(rayOrigin2D, new Vector2(targetPoint.x, targetPoint.y));

            // Raycast for any collider along the ray until z = -0.5
            RaycastHit2D[] hits = Physics2D.RaycastAll(rayOrigin2D, rayDir2D, maxDistance, _tileMask);
            Debug.Log($"[TileHoverUI] Raycast hits: {hits.Length}");
            // Draw debug line from camera to intersection point / draw a short horizontal line at the intersection point
            //Debug.DrawLine(ray.origin, targetPoint, Color.green, 0.1f);
            //Debug.DrawLine(new Vector2(targetPoint.x, targetPoint.y) + Vector2.left * 0.25f, new Vector2(targetPoint.x, targetPoint.y) + Vector2.right * 0.25f, Color.red, 0.1f);
            if (hits != null && hits.Length > 0)
            {
                // Convert RaycastHit2D[] to Collider2D[] for ChooseTopMost
                Collider2D[] colliders = new Collider2D[hits.Length];
                for (int i = 0; i < hits.Length; i++) colliders[i] = hits[i].collider;
                GameObject best = ChooseTopMost(colliders);
                if (best != null)
                {
                    Vector3 pos = best.transform.position;
                    Debug.Log($"[TileHoverUI] Best GameObject position: x={pos.x}, y={pos.y}");
                }
                if (best != _lastHit)
                {
                    _lastHit = best;
                    if (TryParseCoordsFromName(best.name, out int x, out int y))
                    {
                        infoText.text = $"({x}, {y})";
                    }
                    else
                    {
                        infoText.text = best.name;
                    }
                }
            }
            else
            {
                ClearIfChanged();
            }
    }

    // Draws a marker at the mouse pointer's world position in the Scene view
    void OnDrawGizmos()
    {
        if (Camera.main == null) return;
        if (!TryGetPointerScreenPosition(out Vector2 screenPos)) return;
        var cam = Camera.main;
    // Project mouse position to world at z = 0
    float targetZ = 0f;
    float distance = cam.transform.position.z - targetZ;
    Vector3 screenPoint = new Vector3(screenPos.x, screenPos.y, distance);
    Vector3 worldPos = cam.ScreenToWorldPoint(screenPoint);
    Gizmos.color = Color.yellow;
    Gizmos.DrawSphere(worldPos, 0.2f);
    }

    static GameObject ChooseTopMost(Collider2D[] hits)
    {
        GameObject best = null;
        int bestSortingLayer = int.MinValue;
        int bestOrder = int.MinValue;
        float bestZ = float.NegativeInfinity;

        foreach (var h in hits)
        {
            var go = h.gameObject;
            var sr = go.GetComponent<SpriteRenderer>();
            if (sr)
            {
                int layer = SortingLayer.GetLayerValueFromID(sr.sortingLayerID);
                if (layer > bestSortingLayer || (layer == bestSortingLayer && sr.sortingOrder > bestOrder))
                {
                    bestSortingLayer = layer;
                    bestOrder = sr.sortingOrder;
                    best = go;
                    bestZ = go.transform.position.z;
                }
            }
            else
            {
                // fallback by Z if no SpriteRenderer
                if (go.transform.position.z > bestZ)
                {
                    bestZ = go.transform.position.z;
                    best = go;
                }
            }
        }
        return best ?? hits[0].gameObject;
    }

    static bool TryParseCoordsFromName(string name, out int x, out int y)
    {
        x = 0; y = 0;
        if (!name.StartsWith("Tile_")) return false;
        var parts = name.Split('_');
        if (parts.Length < 3) return false;
        return int.TryParse(parts[1], out x) && int.TryParse(parts[2], out y);
    }

    static bool TryGetPointerScreenPosition(out Vector2 pos)
    {
        pos = default;
        if (Mouse.current != null) { pos = Mouse.current.position.ReadValue(); return true; }
        if (Pen.current != null)   { pos = Pen.current.position.ReadValue();   return true; }
        if (Touchscreen.current != null)
        {
            var t = Touchscreen.current.primaryTouch;
            if (t.press.isPressed) { pos = t.position.ReadValue(); return true; }
        }
        return false;
    }

    void ClearIfChanged()
    {
        if (_lastHit != null)
        {
            _lastHit = null;
            infoText.text = "(-, -)";
        }
    }
}
