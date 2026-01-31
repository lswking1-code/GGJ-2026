using UnityEngine;

public class Door : MonoBehaviour
{
    private Collider2D Collider;
    private SpriteRenderer SpriteRenderer;
    public Sprite OpenSprite;
    public Sprite CloseSprite;

    private void Start()
    {
        Collider = GetComponent<Collider2D>();
        SpriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Open()
    {
        Collider.enabled = false;
        SpriteRenderer.sprite = OpenSprite;
    }
    public void Close()
    {
        Collider.enabled = true;
        SpriteRenderer.sprite = CloseSprite;
    }
}
