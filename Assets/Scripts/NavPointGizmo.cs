using UnityEngine;

public class NavPointGizmo : MonoBehaviour
{
    public float radius = 0.2f;
    public Color color = Color.yellow;

    private void OnDrawGizmos()
    {
        Gizmos.color = color;
        Gizmos.DrawSphere(transform.position, radius);
    }
}
