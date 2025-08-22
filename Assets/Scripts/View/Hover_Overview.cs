using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.SceneManagement;


//SOMEHOW ONLY WORKS IF NEVER CHANGEs SCENE AFTER ATTACHING Fixed
//does not work if units /settlements on tile
public class Hover_Overview : MonoBehaviour
{
    public TextMeshProUGUI infoText;
    private Camera cam; //Camera is on movable on x,y plane with z = 10 (ratation is x= 0 y= 180 z= 0) projection is orthographic with size changable by scrolling

    void Start()
    {
        if (SceneManager.GetActiveScene().name != "Ingame")
            return;

        cam = Camera.main;
    }


    void Update()
    {
        if (SceneManager.GetActiveScene().name != "Ingame")
            return;

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
                HexTile_Info info = hexComp.hexTileInfo;
                infoText.text = info.ToString();
                //Debug.Log($"Hovering over tile: {info}");
            }
        }
    }
}