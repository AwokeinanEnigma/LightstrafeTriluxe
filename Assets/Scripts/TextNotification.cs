#region

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#endregion

public class TextNotification : MonoBehaviour
{
    public Text text;

    public Image bg;

    public float xMargin = 10f;

    public float yMargin = 10f;

    public float fadeInMultiplier = 3f;

    public float fadeOutMultiplier = 1f;

    private float timeUntilFade;

    private List<int> waitingOnInputs;

    private PlayerInput input;

    public void SetText(string t, float duration = 5f, int fontSize = 40)
    {
        timeUntilFade = duration;
        text.text = t;
        text.fontSize = fontSize;
        
        // If this isn't done, it'll spam a null reference exception.
        waitingOnInputs = new List<int>();
    }

    public void SetText(string t, IEnumerable<int> waitForInputs, float duration = 5f, int fontSize = 40)
    {
        timeUntilFade = duration;
        text.text = t;
        text.fontSize = fontSize;
        waitingOnInputs = new List<int>(waitForInputs);
    }

    private void Start()
    {
        input = Game.OnStartResolve<PlayerInput>();
        Color color = text.color;
        color.a = 0f;
        text.color = color;
        Color color2 = bg.color;
        color2.a = 0f;
        bg.color = color2;
    }

    private void FixedUpdate()
    {
        if (waitingOnInputs.Count <= 0)
        {
            return;
        }

        timeUntilFade = 1f;
        for (int num = waitingOnInputs.Count - 1; num >= 0; num--)
        {
            int key = waitingOnInputs[num];
            if (input.SincePressed(key) == 0)
            {
                waitingOnInputs.RemoveAt(num);
            }
        }
    }

    private void Update()
    {
        Vector2 sizeDelta = bg.rectTransform.sizeDelta;
        sizeDelta.x = text.preferredWidth + xMargin * 2f;
        sizeDelta.y = text.preferredHeight + yMargin * 2f;
        bg.rectTransform.sizeDelta = sizeDelta;
        text.rectTransform.sizeDelta = sizeDelta;
        if (timeUntilFade < 0f)
        {
            Color color = text.color;
            color.a -= Mathf.Min(Time.deltaTime * fadeOutMultiplier, color.a);
            text.color = color;
            Color color2 = bg.color;
            color2.a -= Mathf.Min(Time.deltaTime * fadeOutMultiplier, color2.a);
            bg.color = color2;
            if (color.a <= 0f && color2.a <= 0f)
            {
                Destroy(this);
            }
        }
        else
        {
            Color color3 = text.color;
            color3.a += Mathf.Min(Time.deltaTime * fadeInMultiplier, 1f - color3.a);
            text.color = color3;
            Color color4 = bg.color;
            color4.a += Mathf.Min(Time.deltaTime * fadeInMultiplier, 1f - color4.a);
            bg.color = color4;
            timeUntilFade -= Time.deltaTime;
        }
    }
}