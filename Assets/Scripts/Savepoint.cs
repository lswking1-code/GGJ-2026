using UnityEngine;
using UnityEngine.UI;

public class Savepoint : MonoBehaviour
{
    [SerializeField] private DataSaveEventSO dataSaveEventSO;
    private bool isSaved = false;
    private SpriteRenderer spriteRenderer;
    public Sprite savedSprite;
    
    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {

        if (other.CompareTag("Player") && !isSaved)
        {
            dataSaveEventSO.RaiseEvent(other.transform.position);
            isSaved = true;
            spriteRenderer.sprite = savedSprite;
        }
    }
}
