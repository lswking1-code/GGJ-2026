using UnityEngine;

public class GoldMask : MonoBehaviour
{
    public VoidEventSO gameClearEventSO;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            gameClearEventSO.RaiseEvent();
        }
    }
}
