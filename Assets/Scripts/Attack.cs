using UnityEngine;

public class Attack : MonoBehaviour
{
    public int damage;

    private void OnTriggerStay2D(Collider2D collision)
    {
        collision.GetComponent<Character>()?.TakeDamage(this);
    }
}
