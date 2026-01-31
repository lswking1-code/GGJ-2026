using System;
using UnityEngine;

[CreateAssetMenu(fileName = "DataSaveEventSO", menuName = "Scriptable Objects/DataSaveEventSO")]
public class DataSaveEventSO : ScriptableObject
{
    public event Action<Vector2> OnEventRaised;

    public void RaiseEvent(Vector2 position)
    {
        OnEventRaised?.Invoke(position);
    }
}
