using System;
using System.Collections;
using System.Collections.Generic;
using DialADemon.Library;
using DialADemon.Page;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;

internal class AudioManagerMonoBehaviour : MonoBehaviour
{
    public delegate void AudioDelegate();
    public AudioDelegate updateDele;

    private void Update()
    {
        updateDele.Invoke();
    }
}

public enum DefaultAudioSource
{
    Effect,
    Background,
    PhoneCall
}
public class AudioManager
{
    private static AudioManagerMonoBehaviour _AudioManagerMonoBehaviour;
    public static Dictionary<DefaultAudioSource, AudioSource> audioSources;
    static AudioManager()
    {
        Init();
    }
    public static void PlaySoundOnSource(AudioSource source, AudioClip clip)
    {
        source.clip = clip;
        source.Play();
    }

    
    public static void PlaySound( DefaultAudioSource das, AudioClip clip, bool loop =  false)
    {
        var a = audioSources[das];
        a.clip = clip;
        a.Play();
        a.loop = loop;
    }

    public static void EffectVolumeChange(DefaultAudioSource das, float finalVolume, float speed=0.5f)
    {
        AudioSource audioSource = audioSources[das];
        CoroutineManager.DoCoroutine(VolumeChangeCoroutine(audioSource, finalVolume, speed));
    }

    static IEnumerator VolumeChangeCoroutine(AudioSource audioSource, float finalVolume, float speed)
    {
        while (Mathf.Abs(finalVolume - audioSource.volume) > 0.02f)
        {
            var newVolume = Mathf.Lerp(audioSource.volume, finalVolume, speed * Time.deltaTime);
            audioSource.volume = newVolume;
            yield return null;
        }
    }

    public static float StopAudio(AudioSource audioSource)
    {
        audioSource.Stop();
        return audioSource.time;
    }
    
    private static void Init()
    {
        var go = new GameObject();
        go.name = "AudioManager";
        _AudioManagerMonoBehaviour = go.AddComponent<AudioManagerMonoBehaviour>();
        audioSources.Add(DefaultAudioSource.Effect, go.AddComponent<AudioSource>());
        audioSources.Add(DefaultAudioSource.Background, go.AddComponent<AudioSource>());
        audioSources.Add(DefaultAudioSource.PhoneCall, go.AddComponent<AudioSource>());
        GameObject.DontDestroyOnLoad(go);
    }
}
