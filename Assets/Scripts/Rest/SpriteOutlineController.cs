using UnityEngine;

public class SpriteOutlineController : MonoBehaviour
{
    public Color outlineColor = Color.red;
    public float outlineSize = 1f;

    private SpriteRenderer sr;
    private Material outlineMat;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        outlineMat = new Material(Shader.Find("Custom/SpriteOutline"));
        outlineMat.SetColor("_OutlineColor", outlineColor);
        outlineMat.SetFloat("_OutlineSize", outlineSize);
    }

    public void EnableOutline(bool enable)
    {
        if (enable)
            sr.material = outlineMat;
        else
            sr.material = null; // revert to default
    }
}