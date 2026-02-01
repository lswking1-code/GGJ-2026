using UnityEngine;
using UnityEngine.Events;

public class Trigger : MonoBehaviour
{
    public UnityEvent TriggerEnterEvent;
    public UnityEvent TriggerExitEvent;
    private SpriteRenderer SpriteRenderer;
    public Sprite EnterSprite;
    public Sprite ExitSprite;
    private int insideCount;

    private void Start()
    {
        SpriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        insideCount++;
        TriggerEnterEvent?.Invoke();
        SpriteRenderer.sprite = EnterSprite;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        insideCount = Mathf.Max(0, insideCount - 1);
        if (insideCount == 0)
        {
            TriggerExitEvent?.Invoke();
            SpriteRenderer.sprite = ExitSprite;
        }
    }
}
