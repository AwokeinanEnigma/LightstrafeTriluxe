#region

using UnityEngine;

#endregion

public class GameSettings
{
    public static bool UseTimingDisplay
    {
        get => PlayerPrefs.GetInt("UseTimingDisplay", 0) != 0;
        set => PlayerPrefs.SetInt("UseTimingDisplay", value ? 1 : 0);
    }

    public static bool FullGameTimer
    {
        get => PlayerPrefs.GetInt("FullGameTimer", 0) != 0;
        set => PlayerPrefs.SetInt("FullGameTimer", value ? 1 : 0);
    }

    public static float SoundVolume
    {
        get => PlayerPrefs.GetFloat("SoundVolume", 0.4f);
        set => PlayerPrefs.SetFloat("SoundVolume", value);
    }

    public static float MusicVolume
    {
        get => PlayerPrefs.GetFloat("MusicVolume", 0.8f);
        set => PlayerPrefs.SetFloat("MusicVolume", value);
    }

    public static float Sensitivity
    {
        get
        {
            if (!PlayerPrefs.HasKey("Sensitivity"))
            {
                PlayerPrefs.SetFloat("Sensitivity", 1f);
            }

            return PlayerPrefs.GetFloat("Sensitivity");
        }
        set => PlayerPrefs.SetFloat("Sensitivity", value);
    }
}