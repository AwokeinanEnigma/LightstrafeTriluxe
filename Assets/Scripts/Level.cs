#region

using Level_Editor_Scripts;
using UnityEngine;
using UnityEngine.SceneManagement;

#endregion

public class Level : MonoBehaviour
{
    public Canvas LevelCompletedUIPrefab;

    private CanvasManager canvasManager;

    private const float KILL_LEVEL = -10f;

    private Player player;

    public bool IsLevelFinished { get; private set; }

    public string LevelName;

    private void Awake()
    {
        Game.OnAwakeBind(this);
    }

    private void Start()
    {
        player = Game.OnStartResolve<Player>();
        canvasManager = Game.OnStartResolve<CanvasManager>();
        Time.timeScale = 1f;
    }

    private void Update()
    {
        if (CustomMapLoader.CustomMap)
        {
            LevelName = CustomMapLoader.MapName;
        }
        else         
            LevelName = SceneManager.GetActiveScene().name;
        
        if (!Input.GetKeyDown((KeyCode)PlayerInput.Pause))
        {
            return;
        }

        if (canvasManager.MenuLayerCount == 1)
        {
            if (!Game.playingReplay || !Game.replayFinishedPlaying)
            {
                Time.timeScale = 1f;
            }

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        if (canvasManager.MenuLayerCount == 0)
        {
            if (IsLevelFinished)
            {
                Time.timeScale = 0f;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                canvasManager.OpenMenuAndSetAsBaseCanvas(LevelCompletedUIPrefab);
            }
            else if (canvasManager.MenuLayerCount == 0)
            {
                Time.timeScale = 0f;
                canvasManager.OpenMenu(canvasManager.Pause);
            }
        }
    }

    private void FixedUpdate()
    {
        if (player != null && player.transform.position.y <= -10f)
        {
            player.DoQuickDeath();
        }

        if (IsLevelFinished)
        {
            if (Time.timeScale > 0.1f)
            {
                Time.timeScale -= Mathf.Min(Time.fixedUnscaledDeltaTime * Time.timeScale, Time.timeScale);
                return;
            }

            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            canvasManager.OpenMenuAndSetAsBaseCanvas(LevelCompletedUIPrefab);
        }
    }

    public void LevelFinished()
    {
        IsLevelFinished = true;
        Game.SaveReplay = true;
    }

    public void RestartLevel()
    {
        IsLevelFinished = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void NextLevel()
    {
        if (SceneManager.GetActiveScene().buildIndex + 1 >= SceneManager.sceneCountInBuildSettings)
        {
            if (SceneManager.GetActiveScene().buildIndex == 0)
            {
                RestartLevel();
                return;
            }

            IsLevelFinished = false;
            Time.timeScale = 1f;
            SceneManager.LoadScene(0);
        }
        else
        {
            IsLevelFinished = false;
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}