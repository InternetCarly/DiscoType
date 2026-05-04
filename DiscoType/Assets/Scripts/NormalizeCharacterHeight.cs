using UnityEngine;

public class NormalizeCharacterHeight : MonoBehaviour
{
    [Header("Settings")]
    public float targetHeight = 3f;

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        NormalizeHeight();
    }

    public void NormalizeHeight()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer.sprite == null) return;

        float currentHeight = spriteRenderer.sprite.bounds.size.y;
        float scaleFactor   = targetHeight / currentHeight;

        transform.localScale = new Vector3(scaleFactor, scaleFactor, 1f);
    }
}