using System;
using UnityEngine;

[CreateAssetMenu(fileName = "MaskChangeEventSO", menuName = "Scriptable Objects/MaskChangeEventSO")]
public class MaskChangeEventSO : ScriptableObject
{
    public event Action<int> OnEventRaised;

    public void RaiseEvent(int value)
    {
        OnEventRaised?.Invoke(value);
    }
}
