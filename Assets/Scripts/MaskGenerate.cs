using System.Collections;
using UnityEngine;

public class MaskGenerate : MonoBehaviour
{
    public GameObject maskPrefab;
    public float respawnDelay = 15f;
    private GameObject currentMask;

    private void Start()
    {
        TrySpawnMask();
        StartCoroutine(RespawnLoop());
    }

    private IEnumerator RespawnLoop()
    {
        while (true)
        {
            if (currentMask == null)
            {
                if (respawnDelay > 0f)
                {
                    yield return new WaitForSeconds(respawnDelay);
                }

                if (currentMask == null)
                {
                    TrySpawnMask();
                }
            }

            yield return null;
        }
    }

    private void TrySpawnMask()
    {
        if (maskPrefab == null)
        {
            return;
        }

        currentMask = Instantiate(maskPrefab, transform.position, transform.rotation);
    }
}
