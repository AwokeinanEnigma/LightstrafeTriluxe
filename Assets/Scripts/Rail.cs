#region

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#endregion

[RequireComponent(typeof(LineRenderer))]
[ExecuteInEditMode]
public class Rail : MonoBehaviour
{
    public enum RailDirection
    {
        OMNIDIRECTIONAL = 0,
        FORWARD = 1,
        BACKWARD = 2
    }

    [Header("Gizmos")]
    public bool showGizmos = true;

    public float detail = 0.02f;

    public float gizmoSize = 0.1f;

    public float hitboxSize = 1f;

    public float speed = 80f;

    public bool hitbox = true;

    public Vector3[] smoothedPoints;

    public Color gizmoColor = new Color(1f, 0f, 0f, 0.5f);

    private RailPoint[] _linePoints = new RailPoint[0];

    private Vector3[] _linePositions = new Vector3[0];

    public LineRenderer line;

    public RailDirection railDirection;

    public void Start()
    {
        Update();
    }

    public void Update()
    {
        GetPoints();
        if (_linePoints.Any(point => point.transform.hasChanged))
        {
            smoothedPoints = getCurvePoints(_linePositions, detail);
            if (hitbox)
            {
                GenerateHitboxes();
            }

            line.positionCount = smoothedPoints.Length;
            line.SetPositions(smoothedPoints);
        }

        RailPoint[] linePoints = _linePoints;
        for (int i = 0; i < linePoints.Length; i++)
        {
            linePoints[i].transform.hasChanged = false;
        }
    }

    public void ChangeHitboxLayer(int layer)
    {
        for (int num = transform.childCount - 1; num > 0; num--)
        {
            if (transform.GetChild(num).name == "hitbox")
            {
                transform.GetChild(num).gameObject.layer = layer;
            }
        }
    }

    public void GenerateHitboxes()
    {
        Vector3 vector = default;
        bool flag = true;
        for (int num = transform.childCount - 1; num > 0; num--)
        {
            if (transform.GetChild(num).name == "hitbox")
            {
                DestroyImmediate(transform.GetChild(num).gameObject);
            }
        }

        Vector3[] array = smoothedPoints;
        foreach (Vector3 vector2 in array)
        {
            if (!flag)
            {
                if ((vector - vector2).magnitude < 1f)
                {
                    continue;
                }

                GameObject obj = new GameObject();
                obj.name = "hitbox";
                obj.tag = "Rail";
                obj.transform.parent = gameObject.transform;
                CapsuleCollider capsuleCollider = (CapsuleCollider)obj.AddComponent(typeof(CapsuleCollider));
                obj.transform.position = Vector3.Lerp(vector2, vector, 0.5f);
                capsuleCollider.height = (vector - vector2).magnitude + hitboxSize;
                capsuleCollider.radius = hitboxSize;
                capsuleCollider.isTrigger = true;
                capsuleCollider.direction = 2;
                obj.transform.rotation = Quaternion.LookRotation((vector - vector2).normalized);
            }

            flag = false;
            vector = vector2;
        }
    }

    private void GetPoints()
    {
        _linePoints = GetComponentsInChildren<RailPoint>();
        _linePositions = new Vector3[_linePoints.Length];
        for (int i = 0; i < _linePoints.Length; i++)
        {
            _linePositions[i] = _linePoints[i].transform.position;
        }
    }

    public static Vector3[] getCurvePoints(Vector3[] controls, float detail)
    {
        if (!(detail > 1f))
        {
            _ = 0f;
        }

        List<Vector3> list = new List<Vector3>();
        List<Vector3> list2 = new List<Vector3>(controls);
        for (int i = 1; i < list2.Count - 1; i += 4)
        {
            Vector3 p = list2[i - 1];
            Vector3 p2 = list2[i];
            Vector3 p3 = list2[i + 1];
            if (i + 2 > list2.Count - 1)
            {
                for (float num = 0f; num < 1f; num += detail)
                {
                    list.Add(quadBezier(p, p2, p3, num));
                }

                continue;
            }

            Vector3 p4 = list2[i + 2];
            for (float num2 = 0f; num2 < 1f; num2 += detail)
            {
                list.Add(cubicBezier(p, p2, p3, p4, num2));
            }
        }

        return list.ToArray();
    }

    public static Vector3 cubicBezier(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, float t)
    {
        return new Vector3(cubicBezierPoint(p1.x, p2.x, p3.x, p4.x, t), cubicBezierPoint(p1.y, p2.y, p3.y, p4.y, t), cubicBezierPoint(p1.z, p2.z, p3.z, p4.z, t));
    }

    public static Vector3 quadBezier(Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        return new Vector3(quadBezierPoint(p1.x, p2.x, p3.x, t), quadBezierPoint(p1.y, p2.y, p3.y, t), quadBezierPoint(p1.z, p2.z, p3.z, t));
    }

    private static float cubicBezierPoint(float a0, float a1, float a2, float a3, float t)
    {
        return Mathf.Pow(1f - t, 3f) * a0 + 3f * Mathf.Pow(1f - t, 2f) * t * a1 + 3f * (1f - t) * Mathf.Pow(t, 2f) * a2 + Mathf.Pow(t, 3f) * a3;
    }

    private static float quadBezierPoint(float a0, float a1, float a2, float t)
    {
        return Mathf.Pow(1f - t, 2f) * a0 + 2f * (1f - t) * t * a1 + Mathf.Pow(t, 2f) * a2;
    }

    public static Vector3 center(Vector3 p1, Vector3 p2)
    {
        return new Vector3((p1.x + p2.x) / 2f, (p1.y + p2.y) / 2f, (p1.z + p2.z) / 2f);
    }

    private void OnDrawGizmosSelected()
    {
        Update();
    }

    private void OnDrawGizmos()
    {
        if (_linePoints.Length == 0)
        {
            GetPoints();
        }

        RailPoint[] linePoints = _linePoints;
        foreach (RailPoint obj in linePoints)
        {
            obj.showGizmo = showGizmos;
            obj.gizmoSize = gizmoSize;
            obj.gizmoColor = gizmoColor;
        }
    }
}