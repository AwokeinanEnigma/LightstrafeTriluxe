#region

using UnityEngine;

#endregion

[ExecuteAlways]
public class ButtonAlignment : MonoBehaviour
{
    private void Update()
    {
        if (!Application.isPlaying)
        {
            RectTransform component = GetComponent<RectTransform>();
            Vector3 localPosition = transform.localPosition;
            localPosition.x -= localPosition.x % (component.rect.width + 10f);
            localPosition.y -= localPosition.y % (component.rect.height + 10f);
        }
    }
}