#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using FullSerializer;
using Level_Editor_Scripts;
using UnityEngine;
using UnityEngine.SceneManagement;

#endregion

public class Game : MonoBehaviour
{
    public class ReplayTick
    {
        public Vector3 pV;

        public Vector3 pP;

        public float py;

        public float pp;
    }

    public class Replay
    {
        public Dictionary<int, List<int>> keyPresses = new Dictionary<int, List<int>>();

        public Dictionary<int, List<int>> keyReleases = new Dictionary<int, List<int>>();

        public int scene;

        public Dictionary<int, ReplayTick> ticks = new Dictionary<int, ReplayTick>();

        public int everyNTicks = 2;

        public int finalTimeTickCount;
        
        public string mapName;
    }

    private static Game I;

    private static List<object> bindings = new List<object>();

    private PlayerInput input;

    private Player player;

    private Timers timers;

    private static Replay currentReplay;

    public static bool playingReplay;

    public static bool replayFinishedPlaying;

    public static bool SaveReplay;

    private static bool replayIgnoreUnload;

    private float nextYaw;

    private float currentYaw;

    private float nextPitch;

    private float currentPitch;

    private float interpolationDelta;

    public static void OnAwakeBind<T>(T obj)
    {
        for (int num = bindings.Count - 1; num >= 0; num--)
        {
            if (bindings[num].GetType() == typeof(T))
            {
                bindings.RemoveAt(num);
                break;
            }
        }

        bindings.Add(obj);
    }

    public static T OnStartResolve<T>()
    {
        T result = default;
        for (int num = bindings.Count - 1; num >= 0; num--)
        {
            object obj = bindings[num];
            if (obj == null)
            {
                bindings.RemoveAt(num);
            }
            else if (obj.GetType() == typeof(T))
            {
                result = (T)obj;
            }
        }

        return result;
    }

    private void Start()
    {
        if (I == null)
        {
            DontDestroyOnLoad(gameObject);
            I = this;
            Application.targetFrameRate = 144;
        }
        else if (I != this)
        {
            I.Start();
            Destroy(gameObject);
            return;
        }

        input = OnStartResolve<PlayerInput>();
        player = OnStartResolve<Player>();
        timers = OnStartResolve<Timers>();
    }

    private void OnEnable()
    {
        if (!(I != null))
        {
            SceneManager.sceneUnloaded += OnSceneUnloaded;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
    }

    public static void StartLevel(string level)
    {
        SceneManager.LoadScene(level);
    }

    public static void PlayReplay(Replay replay)
    {
        currentReplay = replay;
        playingReplay = true;
        replayIgnoreUnload = true;
        replayFinishedPlaying = false;
        if (replay.scene == 9)
        {
            CustomMapLoader.MapName = replay.mapName;
        }
        SceneManager.LoadScene(replay.scene);
    }

    private void OnSceneUnloaded(Scene scene)
    {
        if (SaveReplay)
        {
            SaveReplay = false;
            if (currentReplay.ticks.Count > 0 && !playingReplay)
            {
                string text = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Lightstrafe\\replays";
                Directory.CreateDirectory(text);
                string text2 = DateTime.Now.ToString("MM-dd-y hh:mmtt");
                text2 = text2.Replace("/", "-");
                text2 = text2.Replace(":", "-");
                currentReplay.finalTimeTickCount = timers.CurrentLevelTickCount;
                string text3 = "0.00";
                string text4 = "00.00";
                string text5 = "0";
                
                int finalTimeTickCount = currentReplay.finalTimeTickCount;
                float num = finalTimeTickCount % 6000 * Time.fixedDeltaTime;
                float num2 = Mathf.Floor(finalTimeTickCount / 6000f);
                string text6 = !(num2 > 0f) ? num.ToString(text3) : num2.ToString(text5) + "." + num.ToString(text4);
                string path = text + "\\" + text6 + " - " + scene.name + " " + text2 + ".json";
                if (SceneManager.GetActiveScene().name == "Custom Map")
                {
                    currentReplay.mapName = CustomMapLoader.MapName;
                }
                new fsSerializer().TrySerialize(currentReplay, out var data);
                File.WriteAllText(path, fsJsonPrinter.CompressedJson(data));
            }
        }

        if (replayIgnoreUnload)
        {
            replayIgnoreUnload = false;
        }
        else
        {
            playingReplay = false;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!playingReplay)
        {
            currentReplay = new Replay
            {
                scene = SceneManager.GetActiveScene().buildIndex,
                everyNTicks = 4
            };
        }
    }

    private void Update()
    {
        interpolationDelta += Time.deltaTime;
        if (input != null && player != null && playingReplay)
        {
            float yaw = Mathf.Lerp(currentYaw, nextYaw, interpolationDelta / (Time.fixedDeltaTime * currentReplay.everyNTicks));
            float pitch = Mathf.Lerp(currentPitch, nextPitch, interpolationDelta / (Time.fixedDeltaTime * currentReplay.everyNTicks));
            player.Yaw = yaw;
            player.Pitch = pitch;
        }
    }

    private void FixedUpdate()
    {
        if (!(input != null) || !(player != null))
        {
            return;
        }

        if (playingReplay)
        {
            int num = currentReplay.ticks.Keys.Prepend(0).Max();
            if (input.tickCount <= num)
            {
                if (currentReplay.keyPresses.ContainsKey(input.tickCount))
                {
                    foreach (int item in currentReplay.keyPresses[input.tickCount])
                    {
                        input.SimulateKeyPress(item);
                    }
                }

                if (currentReplay.keyReleases.ContainsKey(input.tickCount))
                {
                    foreach (int item2 in currentReplay.keyReleases[input.tickCount])
                    {
                        input.SimulateKeyRelease(item2);
                    }
                }

                if (currentReplay.ticks.ContainsKey(input.tickCount))
                {
                    ReplayTick replayTick = currentReplay.ticks[input.tickCount];
                    player.velocity = replayTick.pV;
                    player.transform.position = replayTick.pP;
                    currentYaw = replayTick.py;
                    currentPitch = replayTick.pp;
                    if (currentReplay.ticks.ContainsKey(input.tickCount + currentReplay.everyNTicks))
                    {
                        nextYaw = currentReplay.ticks[input.tickCount + currentReplay.everyNTicks].py;
                        nextPitch = currentReplay.ticks[input.tickCount + currentReplay.everyNTicks].pp;
                    }
                    else
                    {
                        nextYaw = currentYaw;
                        nextPitch = currentPitch;
                    }

                    interpolationDelta = 0f;
                }
            }
            else
            {
                Time.timeScale = 0f;
                replayFinishedPlaying = true;
            }

            return;
        }

        PropertyInfo[] properties = typeof(PlayerInput).GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Public);
        List<int> list = new List<int>();
        List<int> list2 = new List<int>();
        PropertyInfo[] array = properties;
        for (int i = 0; i < array.Length; i++)
        {
            int num2 = (int)array[i].GetValue(null, null);
            if (input.SincePressed(num2) == 0)
            {
                list.Add(num2);
            }

            if (input.SinceReleased(num2) == 0)
            {
                list2.Add(num2);
            }
        }

        if (list.Count > 0)
        {
            currentReplay.keyPresses[input.tickCount] = list;
        }

        if (list2.Count > 0)
        {
            currentReplay.keyReleases[input.tickCount] = list2;
        }

        if (input.tickCount % currentReplay.everyNTicks == 0)
        {
            ReplayTick value = new ReplayTick
            {
                pV = player.velocity,
                pP = player.transform.position,
                py = player.Yaw,
                pp = player.Pitch
            };
            currentReplay.ticks[input.tickCount] = value;
        }
    }
}