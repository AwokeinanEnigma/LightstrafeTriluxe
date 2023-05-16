#region

using System;
using System.IO;
using fNbt;
using Level_Editor_Scripts;
using UnityEngine;
using UnityEngine.SceneManagement;

#endregion

public class Timers : MonoBehaviour
{
    private const string FIRST_LEVEL = "Level1";

    private const string LAST_LEVEL = "Level9";

    private static Timers I;

    private Player player;

    private Level level;

    public int CurrentLevelTickCount { get; private set; }

    public int CurrentFullRunTickCount { get; private set; }

    public bool PB { get; private set; }

    public bool TimerRunning { get; set; }

    public int FullGamePB
    {
        get => PlayerPrefs.GetInt("FullGamePB", -1);
        set => PlayerPrefs.SetInt("FullGamePB", value);
    }

    private void Awake()
    {
        if (I == null)
        {
            Game.OnAwakeBind(this);
        }
    }
    
    private void Start()
    {
        if (I == null)
        {
            DontDestroyOnLoad(gameObject);
            I = this;
            ResetFullGameRun();
        }
        else if (I != this)
        {
            I.Start();
            Destroy(gameObject);
            return;
        }

        PB = false;
        level = Game.OnStartResolve<Level>();
        player = Game.OnStartResolve<Player>();
        CurrentLevelTickCount = 0;
        TimerRunning = false;
    }

    private void FixedUpdate()
    {
        if (level == null)
        {
            ResetFullGameRun();
        }

        if (level != null && !level.IsLevelFinished && !TimerRunning && player != null && Flatten(player.velocity).magnitude > 0.01f)
        {
            if (level.LevelName == "Level1")
            {
                StartFullGameRun();
            }

            CurrentLevelTickCount++;
            if (CurrentFullRunTickCount >= 0)
            {
                CurrentFullRunTickCount++;
            }

            TimerRunning = true;
        }

        if (TimerRunning)
        {
            CurrentLevelTickCount++;
            if (CurrentFullRunTickCount >= 0)
            {
                CurrentFullRunTickCount++;
            }

            if (level.IsLevelFinished)
            {
                EndTimer();
            }
        }
    }

    public void SetBestLevelTime(string level, int ticks)
    {
        PlayerPrefs.SetInt("BestTime" + level, ticks);
        Debug.Log( PlayerPrefs.GetInt("BestTime" + level));
    }

    public int GetBestLevelTime(string level)
    {
        Debug.Log("finding time for " + level);
        if (!PlayerPrefs.HasKey("BestTime" + level))
        {
            return -1;
        }

        return PlayerPrefs.GetInt("BestTime" + level);
    }

    public void ResetFullGameRun()
    {
        CurrentFullRunTickCount = -1;
    }

    public void StartFullGameRun()
    {
        CurrentFullRunTickCount = 0;
    }

    public void ResetTimes()
    {
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(i));
            if (PlayerPrefs.HasKey("BestTime" + fileNameWithoutExtension))
            {
                PlayerPrefs.DeleteKey("BestTime" + fileNameWithoutExtension);
            }
        }
    }

    public void ResetMapTimes()
    {
        string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Lightstrafe\\maps";
        Directory.CreateDirectory(path);
        string[] files = Directory.GetFiles(path);
        foreach (string text in files)
        {
            NbtFile file = new NbtFile(text);
            string name = file.RootTag.Get<NbtString>("MapName").Value;
            Debug.Log(name);
            if (PlayerPrefs.HasKey("BestTime" + name))
            {
                PlayerPrefs.DeleteKey("BestTime" + name);
            }
        }
    }

    private void EndTimer()
    {
        if (!TimerRunning)
        {
            return;
        }

        TimerRunning = false;
        if (level.LevelName == "Level9" && CurrentFullRunTickCount >= 0 && CurrentFullRunTickCount < FullGamePB)
        {
            FullGamePB = CurrentFullRunTickCount;
            if (GameSettings.FullGameTimer)
            {
                PB = true;
            }
        }

        if ((CurrentLevelTickCount < GetBestLevelTime(level.LevelName) || GetBestLevelTime(level.LevelName) < 0f) && !Game.playingReplay)
        {
            SetBestLevelTime(level.LevelName, CurrentLevelTickCount);
            Debug.Log(level.LevelName);
            if (!GameSettings.FullGameTimer || CurrentFullRunTickCount < 0)
            {
                PB = true;
            }
        }
    }

    private static Vector3 Flatten(Vector3 vec)
    {
        return new Vector3(vec.x, 0f, vec.z);
    }
}