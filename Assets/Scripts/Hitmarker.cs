#region

using UnityEngine;
using UnityEngine.UI;

#endregion

public class Hitmarker : MonoBehaviour
{
    private Image image;

    private Image Image
    {
        get
        {
            if (image == null)
            {
                image = GetComponent<Image>();
            }

            return image;
        }
    }

    public void Display()
    {
        Color color = Image.color;
        color.a = 1f;
        Image.color = color;
    }

    private void Awake()
    {
        Color color = Image.color;
        color.a = 0f;
        Image.color = color;
    }

    private void Update()
    {
        Color color = Image.color;
        if (color.a > 0f)
        {
            color.a -= Time.deltaTime;
        }

        Image.color = color;
    }
}