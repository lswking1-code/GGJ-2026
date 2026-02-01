using UnityEngine;

public class DeadZone : MonoBehaviour
{
    private Collider2D Collider;
    public MaskChangeEventSO maskChangeEventSO;
    private void OnEnable()
    {
        maskChangeEventSO.OnEventRaised += OnMaskChange;
    }
    private void OnDisable()
    {
        maskChangeEventSO.OnEventRaised -= OnMaskChange;
    }
    private void Start()
    {
        Collider = GetComponent<Collider2D>();
    }
    private void OnMaskChange(int value)
    {
        if (value == 3)
        {
            Collider.enabled = false;
        }
        else
        {
            Collider.enabled = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            TopDownPlayerController1 player = other.GetComponent<TopDownPlayerController1>();
            if (!player.Sad)
            {
                other.GetComponent<Character>().OnDie?.Invoke();
            }
        }
    }
}
