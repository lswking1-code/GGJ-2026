using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class LSwitch : MonoBehaviour
{
    public MaskChangeEventSO maskChangeEventSO;

    [Header("Global Volume")]
    [SerializeField] private Volume globalVolume;

    [Header("Color Filter")]
    [SerializeField] private Color angryColor = new Color(1f, 0.4f, 0.4f, 1f);
    [SerializeField] private Color happyColor = new Color(1f, 1f, 1f, 1f);
    [SerializeField] private Color sadColor = new Color(0.4f, 0.6f, 1f, 1f);

    private void OnEnable()
    {
        if (maskChangeEventSO == null)
        {
            return;
        }

        maskChangeEventSO.OnEventRaised += OnMaskChange;
    }
    private void OnDisable()
    {
        if (maskChangeEventSO == null)
        {
            return;
        }

        maskChangeEventSO.OnEventRaised -= OnMaskChange;
    }
    private void OnMaskChange(int value)
    {
        if (globalVolume == null || globalVolume.profile == null)
        {
            return;
        }

        if (!globalVolume.profile.TryGet(out ColorAdjustments colorAdjustments))
        {
            return;
        }

        Color targetColor;
        switch (value)
        {
            case 1:
                targetColor = angryColor;
                break;
            case 2:
                targetColor = happyColor;
                break;
            case 3:
                targetColor = sadColor;
                break;
            default:
                return;
        }

        colorAdjustments.colorFilter.overrideState = true;
        colorAdjustments.colorFilter.value = targetColor;
    }
}
