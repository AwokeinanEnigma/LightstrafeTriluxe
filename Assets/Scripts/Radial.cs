#region

using System;
using UnityEngine;
using UnityEngine.UI;

#endregion

public class Radial : MonoBehaviour
{
    public float size;

    public float position;

    private const float RADIUS = 28f;

    private const float MIN_SIZE = 0.25f;

    private float yScale;

    public Image Image { get; set; }

    private void Awake()
    {
        Image = GetComponent<Image>();
    }

    private void Start()
    {
        transform.localPosition = new Vector3(0f - Mathf.Sin(position * ((float)Math.PI / 180f)), Mathf.Cos(position * ((float)Math.PI / 180f)), 0f) * 28f;
        transform.localRotation = Quaternion.Euler(0f, 0f, position);
    }

    private void Update()
    {
        size = Mathf.Lerp(size, 0.25f, Time.deltaTime);
        yScale = Mathf.Lerp(yScale, size, Time.deltaTime * 20f);
        transform.localScale = new Vector3(1f, yScale, 1f);
        if (yScale >= size || size <= 0.3f)
        {
            if (Image.color.a > 0f)
            {
                Image.color -= new Color(0f, 0f, 0f, Time.deltaTime);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}