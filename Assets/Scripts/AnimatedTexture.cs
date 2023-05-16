#region

using System;
using UnityEngine;

#endregion

public class AnimatedTexture : MonoBehaviour
{
    public Texture2D[] fzzySodaFrames;

    public Texture2D[] bigBlockRightArrowFrames;

    public Texture2D[] bigBlockLeftArrowFrames;

    private MusicPlayer music;

    private Material[] materials;

    private int lastIndex;

    private void Start()
    {
        music = Game.OnStartResolve<MusicPlayer>();
        materials = GetComponent<Renderer>().materials;
    }

    private void Update()
    {
        if (music == null)
        {
            return;
        }

        Material[] array = materials;
        foreach (Material material in array)
        {
            if (material.name.StartsWith("bigBlockBlackYellowMat"))
            {
                float x = calcBounce(4f, 0.6f);
                Vector2 value = material.mainTextureOffset = new Vector2(x, 0f);
                material.SetTextureOffset("_EmissiveColorMap", value);
            }

            if (material.name.StartsWith("bigBlockColorfulMat"))
            {
                float x2 = calcElastic(10f, 0.5f) + calcSine(25f, 5f);
                float y = calcElastic(7f, 0.6f) + calcSine(15f, 3f);
                Vector2 value2 = material.mainTextureOffset = new Vector2(x2, y);
                material.SetTextureOffset("_EmissiveColorMap", value2);
            }

            if (material.name.StartsWith("fzzySodaMat"))
            {
                float num = music.Audio.timeSamples / (float)music.musicList[music.currentlyPlayingIndex].frequency;
                float num2 = music.bpmList[music.currentlyPlayingIndex] / 60f;
                int num3 = Mathf.RoundToInt(num * num2);
                num3 %= fzzySodaFrames.Length;
                if (num3 != lastIndex && Time.timeScale > 0f)
                {
                    material.mainTexture = fzzySodaFrames[num3];
                }

                lastIndex = num3;
            }

            if (material.name.StartsWith("bigBlockArrowMat"))
            {
                float num4 = music.Audio.timeSamples / (float)music.musicList[music.currentlyPlayingIndex].frequency;
                float num5 = music.bpmList[music.currentlyPlayingIndex] / 60f;
                int num6 = Mathf.RoundToInt(num4 * num5);
                num6 %= bigBlockRightArrowFrames.Length;
                if (num6 != lastIndex && Time.timeScale > 0f)
                {
                    material.mainTexture = bigBlockRightArrowFrames[num6];
                }

                lastIndex = num6;
            }

            if (material.name.StartsWith("bigBlockArrowLeftMat"))
            {
                float num7 = music.Audio.timeSamples / (float)music.musicList[music.currentlyPlayingIndex].frequency;
                float num8 = music.bpmList[music.currentlyPlayingIndex] / 60f;
                int num9 = Mathf.RoundToInt(num7 * num8);
                num9 %= bigBlockLeftArrowFrames.Length;
                if (num9 != lastIndex && Time.timeScale > 0f)
                {
                    material.mainTexture = bigBlockLeftArrowFrames[num9];
                }

                lastIndex = num9;
            }

            if (material.name.StartsWith("bigBlockRingMat"))
            {
                float num10 = calcRing(5f, 3f);
                Vector2 vector3 = new Vector2(num10, num10);
                vector3 += Vector2.one * 0.5f;
                material.mainTextureOffset = -(vector3 / 2f) + Vector2.one * 0.5f;
                material.mainTextureScale = vector3;
                material.SetTextureOffset("_EmissiveColorMap", -(vector3 / 2f) + Vector2.one * 0.5f);
                material.SetTextureScale("_EmissiveColorMap", vector3);
            }
        }
    }

    private float calcRing(float speed, float distance)
    {
        float num = (Time.time + distance) % speed;
        float num2 = 0f;
        if (num > speed / 2f)
        {
            float x = (num - speed / 2f) / (speed / 2f);
            float num3 = easeOutBounce(x);
            num2 = 1f - num3;
        }
        else
        {
            float x2 = num / (speed / 2f);
            num2 = easeOutElastic(x2);
        }

        return num2 * distance;
    }

    private float calcSine(float speed, float distance)
    {
        float num = (Time.time + distance) % speed;
        float num2 = 0f;
        if (num > speed / 2f)
        {
            float x = (num - speed / 2f) / (speed / 2f);
            float num3 = easeOutSine(x);
            num2 = 1f - num3;
        }
        else
        {
            float x2 = num / (speed / 2f);
            num2 = easeOutSine(x2);
        }

        return num2 * distance;
    }

    private float calcBounce(float speed, float distance)
    {
        float num = (Time.time + distance) % speed;
        float num2 = 0f;
        if (num > speed / 2f)
        {
            float x = (num - speed / 2f) / (speed / 2f);
            float num3 = easeOutBounce(x);
            num2 = 1f - num3;
        }
        else
        {
            float x2 = num / (speed / 2f);
            num2 = easeOutBounce(x2);
        }

        return num2 * distance;
    }

    private float calcElastic(float speed, float distance)
    {
        float num = (Time.time + distance) % speed;
        float num2 = 0f;
        if (num > speed / 2f)
        {
            float x = (num - speed / 2f) / (speed / 2f);
            float num3 = easeOutElastic(x);
            num2 = 1f - num3;
        }
        else
        {
            float x2 = num / (speed / 2f);
            num2 = easeOutElastic(x2);
        }

        return num2 * distance;
    }

    private float easeOutSine(float x)
    {
        return Mathf.Sin(x * (float)Math.PI / 2f);
    }

    private float easeInOutElastic(float x)
    {
        float num = (float)Math.PI * 4f / 9f;
        if (x != 0f)
        {
            if (x != 1f)
            {
                if (!(x < 0.5))
                {
                    return Mathf.Pow(2f, -20f * x + 10f) * Mathf.Sin((20f * x - 11.125f) * num) / 2f + 1f;
                }

                return (0f - Mathf.Pow(2f, 20f * x - 10f) * Mathf.Sin((20f * x - 11.125f) * num)) / 2f;
            }

            return 1f;
        }

        return 0f;
    }

    private float easeOutElastic(float x)
    {
        float num = (float)Math.PI * 2f / 3f;
        if (x != 0f)
        {
            if (x != 1f)
            {
                return Mathf.Pow(2f, -10f * x) * Mathf.Sin((x * 10f - 0.75f) * num) + 1f;
            }

            return 1f;
        }

        return 0f;
    }

    private float easeInOutBounce(float x)
    {
        if (!(x < 0.5f))
        {
            return (1f + easeOutBounce(2f * x - 1f)) / 2f;
        }

        return (1f - easeOutBounce(1f - 2f * x)) / 2f;
    }

    private float easeOutBounce(float x)
    {
        float num = 7.5625f;
        float num2 = 2.75f;
        if (x < 1f / num2)
        {
            return num * x * x;
        }

        if (x < 2f / num2)
        {
            return num * (x -= 1.5f / num2) * x + 0.75f;
        }

        if (x < 2.5f / num2)
        {
            return num * (x -= 2.25f / num2) * x + 0.9375f;
        }

        return num * (x -= 2.625f / num2) * x + 63f / 64f;
    }
}