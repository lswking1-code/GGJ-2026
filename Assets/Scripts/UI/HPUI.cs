using UnityEngine;
using UnityEngine.UI;

public class HPUI : MonoBehaviour
{
    public Image hp1;
    public Image hp2;
    public Image hp3;

    public void OnHealthChange(float health)
    {
        int hp = Mathf.Clamp(Mathf.FloorToInt(health), 0, 3);
        if (hp1 != null) hp1.enabled = hp >= 1;
        if (hp2 != null) hp2.enabled = hp >= 2;
        if (hp3 != null) hp3.enabled = hp >= 3;
    }

}
