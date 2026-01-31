using UnityEngine;

public class Enemy : MonoBehaviour
{
    public GameObject[] NavPoints;
    public float moveSpeed = 5f;
    public float arriveDistance = 0.1f;

    private int currentIndex = 0;
    private int direction = 1;

    private void Update()
    {
        if (NavPoints == null || NavPoints.Length == 0)
        {
            return;
        }

        if (currentIndex < 0 || currentIndex >= NavPoints.Length)
        {
            currentIndex = Mathf.Clamp(currentIndex, 0, NavPoints.Length - 1);
            direction = 1;
        }

        GameObject target = NavPoints[currentIndex];
        if (target == null)
        {
            AdvanceIndex();
            return;
        }

        Vector3 targetPos = target.transform.position;
        transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPos) <= arriveDistance)
        {
            AdvanceIndex();
        }
    }

    private void AdvanceIndex()
    {
        if (NavPoints == null || NavPoints.Length == 0)
        {
            return;
        }

        if (currentIndex == NavPoints.Length - 1)
        {
            direction = -1;
        }
        else if (currentIndex == 0)
        {
            direction = 1;
        }

        currentIndex += direction;
        currentIndex = Mathf.Clamp(currentIndex, 0, NavPoints.Length - 1);
    }
}
