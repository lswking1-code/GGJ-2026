using UnityEngine;

public class DataSaveManage : MonoBehaviour
{
    public DataSaveEventSO dataSaveEventSO;
    public GameObject player;
    private Vector3 savedPlayerPosition;
    private bool hasSavedPosition;

    private void OnEnable()
    {
        if (dataSaveEventSO != null)
        {
            dataSaveEventSO.OnEventRaised += OnDataSave;
        }
    }

    private void OnDisable()
    {
        if (dataSaveEventSO != null)
        {
            dataSaveEventSO.OnEventRaised -= OnDataSave;
        }
    }

    private void OnDataSave(Vector2 position)
    {
        float z = player != null ? player.transform.position.z : 0f;
        savedPlayerPosition = new Vector3(position.x, position.y, z);
        hasSavedPosition = true;
    }

    public void ResetPlayerToSavedPosition()
    {
        if (!hasSavedPosition)
        {
            Debug.LogWarning("No saved position yet.");
            return;
        }

        if (player == null)
        {
            Debug.LogWarning("Player reference is missing.");
            return;
        }

        player.transform.position = savedPlayerPosition;
    }
}
