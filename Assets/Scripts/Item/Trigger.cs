using UnityEngine;
using UnityEngine.Events;

public class Trigger : MonoBehaviour
{
    public UnityEvent TriggerEnterEvent;
    public UnityEvent TriggerExitEvent;
    private SpriteRenderer SpriteRenderer;
    public Sprite EnterSprite;
    public Sprite ExitSprite;

    private void Start()
    {
        SpriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TriggerEnterEvent?.Invoke();
        SpriteRenderer.sprite = EnterSprite;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        TriggerExitEvent?.Invoke();
        SpriteRenderer.sprite = ExitSprite;
    }
}
