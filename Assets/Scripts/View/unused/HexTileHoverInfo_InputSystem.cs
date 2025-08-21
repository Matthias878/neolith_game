using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class HexTileHoverInfo : MonoBehaviour
{
    public Camera mainCamera;
    public LayerMask hexTileLayer;
    public TMPro.TextMeshProUGUI infoText;

    void Update()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        if (mainCamera == null || infoText == null)
            return;

        Vector2 mousePosition = Mouse.current != null ? Mouse.current.position.ReadValue() : Vector2.zero;
        // Use the camera's distance for Z to get the correct world point
        Vector3 mouseScreen = new Vector3(mousePosition.x, mousePosition.y, Mathf.Abs(mainCamera.transform.position.z));
        Vector3 worldPoint = mainCamera.ScreenToWorldPoint(mouseScreen);
        Vector2 worldPoint2D = new Vector2(worldPoint.x, worldPoint.y);
        Collider2D hit = Physics2D.OverlapPoint(worldPoint2D, hexTileLayer);
        if (hit != null)
        {
            var tile = hit.GetComponent<HexTileComponent>();
            if (tile != null && tile.hexTileInfo != null)
            {
                infoText.text = tile.hexTileInfo.ToString();
            }
            else
            {
                infoText.text = "";
            }
        }
        else
        {
            infoText.text = "";
        }
    }
}
