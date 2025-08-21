using UnityEngine;
using UnityEngine.InputSystem;

public class HexTileHoverDetector : MonoBehaviour
{
    [SerializeField] private Camera cam;
    //public TMPro.TextMeshProUGUI infoText;

    private void Awake()
    {
        if (cam == null)
        {
            cam = Camera.main;
        }
    }

    void Update()
    {
        // Get mouse position from the new Input System
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPos = cam.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, cam.nearClipPlane));

        // Raycast to find the tile under the mouse
        RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero);
        if (hit.collider != null)
        {
            HexTileComponent hexComp = hit.collider.GetComponent<HexTileComponent>();
            if (hexComp != null)
            {
                HexTile_Tnfo info = hexComp.hexTileInfo;
                //infoText.text = info.ToString();
                Debug.Log($"Hovering over tile: {info}");
            }
        }
    }
}