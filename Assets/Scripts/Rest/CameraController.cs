using UnityEngine;
using UnityEngine.InputSystem;

// Attach this script to your Camera GameObject
public class CameraController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float scrollSpeed = 5f;
    public float minOrthoSize = 2f;
    public float maxOrthoSize = 20f;

    private Camera cam;
    private Vector2 moveInput;
    private float scrollInput;

    void Start()
    {
        cam = GetComponent<Camera>();
        if (cam == null)
        {
            Debug.LogError("CameraController must be attached to a Camera!");
        }
    }

    void Update()//TODO change scroll speed based on zoom level
    {
        // WASD and Arrow Key Movement using new Input System
        moveInput = Vector2.zero;
        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)
                moveInput.y += 1;
            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)
                moveInput.y -= 1;
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
                moveInput.x -= 1;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
                moveInput.x += 1;
        }
    Vector3 move = new Vector3(moveInput.x, moveInput.y, 0f).normalized;
    float scaledSpeed = moveSpeed * cam.orthographicSize / 5f; // 5f is your default zoom
    transform.position += move * scaledSpeed * Time.deltaTime;

        // Scroll to zoom in/out using new Input System
        scrollInput = 0f;
        if (Mouse.current != null)
        {
            scrollInput = Mouse.current.scroll.ReadValue().y;
        }
        if (scrollInput != 0f && cam.orthographic)
        {
            cam.orthographicSize -= scrollInput * scrollSpeed * 0.1f; // 0.1f to match old scroll sensitivity
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minOrthoSize, maxOrthoSize);
        }
    }
}
