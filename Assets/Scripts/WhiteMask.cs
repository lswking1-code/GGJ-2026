using UnityEngine;

public class WhiteMask : MonoBehaviour
{
    [SerializeField] private int amount = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryGive(other.gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        TryGive(collision.gameObject);
    }

    private void TryGive(GameObject other)
    {
        TopDownPlayerController player = other.GetComponent<TopDownPlayerController>();
        if (player == null)
        {
            return;
        }

        player.AddWhiteMask(amount);
        Destroy(gameObject);
    }
}
