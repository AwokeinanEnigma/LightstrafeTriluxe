#region

using System.Collections.Generic;
using Level_Editor_Scripts;
using UnityEngine;

#endregion

public class PlayerAudioManager : MonoBehaviour
{
    public struct PlayingAudio
    {
        public GameObject obj;

        public float volume;

        public bool ignoreTimescale;

        public PlayingAudio(GameObject obj, float volume = 1f, bool ignoreTimescale = false)
        {
            this.obj = obj;
            this.volume = volume;
            this.ignoreTimescale = ignoreTimescale;
        }
    }

    private Dictionary<string, PlayingAudio> playingAudio = new Dictionary<string, PlayingAudio>();

    private int i;

    public GameObject EditorCamera;
    
    public void PlayOneShot(AudioClip clip, bool looping = false, float volume = 1f, bool ignoreTimescale = false)
    {
        string text = clip.name + i++;
        if (i > 500000)
        {
            i = 0;
        }

        if (!playingAudio.ContainsKey(text))
        {
            GameObject gameObject = new GameObject("Audio-" + text);
            gameObject.transform.parent = this.gameObject.transform;
            gameObject.transform.localPosition = Vector3.zero;
            AudioSource audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = clip;
            audioSource.volume = GameSettings.SoundVolume;
            if (!ignoreTimescale)
            {
                audioSource.pitch = Time.timeScale;
            }

            audioSource.loop = looping;
            audioSource.Play();
            PlayingAudio value = new PlayingAudio(gameObject, volume, ignoreTimescale);
            playingAudio[text] = value;
        }
    }

    public void PlayOneShotEditor(AudioClip clip, bool looping = false, float volume = 1f, bool ignoreTimescale = false)
    {
        string text = clip.name + i++;
        if (i > 500000)
        {
            i = 0;
        }

        if (!playingAudio.ContainsKey(text))
        {
            GameObject gameObject = new GameObject("Audio-" + text);
            gameObject.transform.parent = EditorCamera.transform;
            gameObject.transform.localPosition = Vector3.zero;
            AudioSource audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = clip;
            audioSource.volume = GameSettings.SoundVolume;
            if (!ignoreTimescale)
            {
                audioSource.pitch = Time.timeScale;
            }

            audioSource.loop = looping;
            audioSource.Play();
            PlayingAudio value = new PlayingAudio(gameObject, volume, ignoreTimescale);
            playingAudio[text] = value;
        }
    }

    public void PlayAudio(AudioClip clip, bool looping = false, float volume = 1f, bool ignoreTimescale = false)
    {
        if (IsPlaying(clip))
        {
            StopAudio(clip);
        }

        GameObject gameObject = new GameObject("Audio-" + clip.name);
        gameObject.transform.parent = this.gameObject.transform;
        gameObject.transform.localPosition = Vector3.zero;
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.volume = GameSettings.SoundVolume * volume;
        if (!ignoreTimescale)
        {
            audioSource.pitch = Time.timeScale;
        }

        audioSource.loop = looping;
        audioSource.Play();
        PlayingAudio value = new PlayingAudio(gameObject, volume, ignoreTimescale);
        playingAudio[clip.name] = value;
    }
    
    public void PlayAudioDeferred(AudioClip clip, bool looping = false, float volume = 1f, bool ignoreTimescale = false)
    {
        if (SessionManager.Instance)
        {
            PlayAudioEditor(clip, looping, volume, ignoreTimescale);
        }
        else
        {
            PlayAudio(clip, looping, volume, ignoreTimescale);
        }
    }
    
    public void PlayOneShotDeferred(AudioClip clip, bool looping = false, float volume = 1f, bool ignoreTimescale = false)
    {
        if (SessionManager.Instance)
        {
            PlayOneShotEditor(clip, looping, volume, ignoreTimescale);
        }
        else
        {
            PlayOneShot(clip, looping, volume, ignoreTimescale);
        }
    }

    public void PlayAudioEditor(AudioClip clip, bool looping = false, float volume = 1f, bool ignoreTimescale = false)
    {
        if (IsPlaying(clip))
        {
            StopAudio(clip);
        }

        GameObject gameObject = new GameObject("Audio-" + clip.name);
        gameObject.transform.parent = EditorCamera.transform;
        gameObject.transform.localPosition = Vector3.zero;
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.volume = GameSettings.SoundVolume * volume;
        if (!ignoreTimescale)
        {
            audioSource.pitch = Time.timeScale;
        }

        audioSource.loop = looping;
        audioSource.Play();
        PlayingAudio value = new PlayingAudio(gameObject, volume, ignoreTimescale);
        playingAudio[clip.name] = value;
    }
    
    public void StopAudio(AudioClip clip)
    {
        if (IsPlaying(clip))
        {
            PlayingAudio obj = playingAudio[clip.name];
            playingAudio.Remove(clip.name);
            Destroy(obj.obj);
        }
    }

    public bool IsPlaying(AudioClip clip)
    {
        return playingAudio.ContainsKey(clip.name);
    }

    public void SetVolume(AudioClip clip, float volume)
    {
        if (IsPlaying(clip))
        {
            PlayingAudio value = playingAudio[clip.name];
            value.volume = volume;
            playingAudio[clip.name] = value;
        }
    }

    private void Awake()
    {
        Game.OnAwakeBind(this);
    }

    private void Update()
    {
        foreach (KeyValuePair<string, PlayingAudio> item in playingAudio)
        {
            PlayingAudio value = item.Value;
            AudioSource component = value.obj.GetComponent<AudioSource>();
            if (!item.Value.ignoreTimescale)
            {
                component.pitch = Time.timeScale;
            }

            component.volume = GameSettings.SoundVolume * value.volume;
        }
    }

    private void FixedUpdate()
    {
        Dictionary<string, PlayingAudio>.Enumerator enumerator = playingAudio.GetEnumerator();
        List<string> list = new List<string>();
        while (enumerator.MoveNext())
        {
            if (!enumerator.Current.Value.obj.GetComponent<AudioSource>().isPlaying)
            {
                list.Add(enumerator.Current.Key);
                Destroy(enumerator.Current.Value.obj);
            }
        }

        foreach (string item in list)
        {
            playingAudio.Remove(item);
        }
    }
}