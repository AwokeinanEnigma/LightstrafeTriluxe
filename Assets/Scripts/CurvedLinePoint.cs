#region

using UnityEngine;

#endregion

public class CurvedLinePoint : MonoBehaviour
{
    [HideInInspector]
    public bool showGizmo = true;

    [HideInInspector]
    public float gizmoSize = 0.1f;

    [HideInInspector]
    public Color gizmoColor = new Color(1f, 0f, 0f, 0.5f);

    private void OnDrawGizmos()
    {
        if (showGizmo)
        {
            Gizmos.color = gizmoColor;
            Gizmos.DrawSphere(transform.position, gizmoSize);
        }
    }

    private void OnDrawGizmosSelected()
    {
        CurvedLineRenderer component = transform.parent.GetComponent<CurvedLineRenderer>();
        if (component != null)
        {
            component.Update();
        }
    }
}