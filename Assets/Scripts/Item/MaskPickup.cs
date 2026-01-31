using UnityEngine;

public class MaskPickup : MonoBehaviour
{
    [SerializeField] private TopDownPlayerController1.MaskType maskType = TopDownPlayerController1.MaskType.None;
    [SerializeField] private DataSaveEventSO dataSaveEventSO;

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
        TopDownPlayerController1 player = other.GetComponent<TopDownPlayerController1>();
        if (player == null)
        {
            return;
        }

        if (dataSaveEventSO != null)
        {
            Vector3 playerPosition = player.transform.position;
            dataSaveEventSO.RaiseEvent(new Vector2(playerPosition.x, playerPosition.y));
        }

        player.AcquireMask(maskType);
        Destroy(gameObject);
    }
}
