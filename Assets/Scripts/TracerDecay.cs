#region

using UnityEngine;

#endregion

public class TracerDecay : MonoBehaviour
{
    private LineRenderer line;

    private float a = 1f;

    private void Start()
    {
        line = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        if (a < 0f)
        {
            Destroy(gameObject);
        }

        Color white = Color.white;
        white.a = Mathf.Clamp01(a);
        line.material.color = white;
        a -= Time.deltaTime;
    }
}