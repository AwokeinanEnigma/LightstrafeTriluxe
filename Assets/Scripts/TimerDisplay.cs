#region

using System.IO;
using Level_Editor_Scripts;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

#endregion

public class TimerDisplay : MonoBehaviour
{
    public enum TimerType
    {
        CURRENT_LEVEL = 0,
        LEVEL_BEST = 1,
        SUM_OF_BEST = 2,
        FULL_GAME_PB = 3,
        FULL_GAME = 4
    }

    public Text timerText;

    public string prefix;

    public string levelName;

    public TimerType timerType;

    private Timers timers;

    private Level level;

    private void Start()
    {
        level = Game.OnStartResolve<Level>();
        timers = Game.OnStartResolve<Timers>();
    }

    private void Update()
    {
        string text = "0.00";
        string text2 = "00.00";
        string text3 = "0";
        bool flag = levelName.Length <= 0;
        Color color = Color.white;
        if (level != null && level.IsLevelFinished)
        {
            color = Color.green;
            if (timers.PB)
            {
                color = Color.yellow;
            }
        }

        int num = 0;
        switch (timerType)
        {
            case TimerType.LEVEL_BEST:
                Debug.Log(timers.GetBestLevelTime(level.LevelName));
                num = timers.GetBestLevelTime(level.LevelName);
                break;
            case TimerType.CURRENT_LEVEL:
                num = timers.CurrentLevelTickCount;
                timerText.color = color;
                break;
            case TimerType.FULL_GAME:
                num = GameSettings.FullGameTimer && timers.CurrentFullRunTickCount >= 0 && timers.CurrentFullRunTickCount != timers.CurrentLevelTickCount ? timers.CurrentFullRunTickCount : -1;
                break;
            case TimerType.SUM_OF_BEST:
            {
                num = 0;
                for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
                {
                    string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(i));
                    if (fileNameWithoutExtension.ToLower().Contains("level") && !PlayerPrefs.HasKey("BestTime" + fileNameWithoutExtension))
                    {
                        num = -1;
                        break;
                    }
                }

                break;
            }
            case TimerType.FULL_GAME_PB:
                num = timers.FullGamePB;
                break;
        }

        if (num == -1)
        {
            timerText.text = "";
            return;
        }

        float num2 = num % 6000 * Time.fixedDeltaTime;
        float num3 = Mathf.Floor(num / 6000f);
        if (num3 > 0f)
        {
            timerText.text = prefix + num3.ToString(text3) + ":" + num2.ToString(text2);
        }
        else
        {
            timerText.text = prefix + num2.ToString(text);
        }
    }
}