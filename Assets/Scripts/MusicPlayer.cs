#region

using System.Collections.Generic;
using UnityEngine;

#endregion

public class MusicPlayer : MonoBehaviour
{
    public List<AudioClip> musicList;

    public List<float> bpmList;

    private float bpm = 140f;

    public int currentlyPlayingIndex;

    private static MusicPlayer I;

    private Player player;

    public AudioSource Audio { get; set; }

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
            Audio = GetComponent<AudioSource>();
            PlayMusic(1);
        }
        else if (I != this)
        {
            I.Start();
            currentlyPlayingIndex = -1;
            for (int i = 0; i < musicList.Count; i++)
            {
                AudioClip audioClip = musicList[i];
                if (I.musicList[I.currentlyPlayingIndex].name == audioClip.name)
                {
                    I.currentlyPlayingIndex = i;
                    currentlyPlayingIndex = 0;
                    break;
                }
            }

            I.musicList = musicList;
            I.bpmList = bpmList;
            if (I.currentlyPlayingIndex == -1)
            {
                I.PlayMusic(0);
            }

            Destroy(gameObject);
            return;
        }

        player = Game.OnStartResolve<Player>();
    }

    public void PlayMusic(int index)
    {
        if (Audio.isPlaying)
        {
            Audio.Stop();
        }

        currentlyPlayingIndex = index;
        Audio.clip = musicList[index];
        bpm = bpmList[index];
        Audio.volume = GameSettings.MusicVolume * GameSettings.SoundVolume;
        Audio.pitch = 1f;
        Audio.loop = false;
        Audio.Play();
    }

    private void Update()
    {
        if (!Audio.isPlaying)
        {
            currentlyPlayingIndex++;
            if (currentlyPlayingIndex >= bpmList.Count)
            {
                currentlyPlayingIndex = 0;
            }

            Audio.clip = musicList[currentlyPlayingIndex];
            bpm = bpmList[currentlyPlayingIndex];
            Audio.Play();
        }

        Audio.volume = GameSettings.MusicVolume * GameSettings.SoundVolume;
        if (player != null)
        {
            transform.position = player.camera.transform.position;
        }
    }
}