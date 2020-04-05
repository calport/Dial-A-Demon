using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TimeUtil;
using UnityEngine;



public class AudioManager
{
    internal class AudioManagerMonoBehaviour : MonoBehaviour
    {
        private AudioManager parent;
        private SlowUpdate su = new SlowUpdate(10);

        AudioManagerMonoBehaviour(AudioManager am)
        {
            parent = am;
        }
        private void Update()
        {
            if (su.detect)
                _CleanDeadSources();
        }

        private void _CleanDeadSources()
        {
            foreach (var source in parent.effectAudioSources)
                if (!source.isPlaying)
                    Destroy(source.gameObject);
        }
        
    }

    private AudioManagerMonoBehaviour _audioManagerMonoBehaviour;
    private GameObject ea;
    private GameObject ba;
    public List<AudioSource> effectAudioSources = new List<AudioSource>(0);
    public List<AudioSource> backgroundAudioSources = new List<AudioSource>(0);
    
    public AudioManager()
    {
        _Init();
    }

    private void _Init()
    {
        //create the main game object
        var am = new GameObject();
        am.name = "AudioManager";
        _audioManagerMonoBehaviour = am.AddComponent<AudioManagerMonoBehaviour>();
        
        //create seperate obj for storing the audio sources
        ea = new GameObject();
        ea.name = "EffectAudioSources";
        ea.transform.parent = am.transform;
        
        ba = new GameObject();
        ba.name = "BackgroundAudioSources";
        ba.transform.parent = am.transform;

        for (int i = 0; i < 3; i++)
            _CreateNewAudioSource("BackgroundTrack" + i,false);
        
        GameObject.DontDestroyOnLoad(am);
        
    }

    public AudioSource PlayEffectAudio(string audioName, string path = "")
    {
        var clip = GetAudioClip(audioName,path);
        var audioSource = _CreateNewAudioSource(audioName,true);
        
        audioSource.clip = clip;
        audioSource.Play();
        return audioSource;
    }

    public AudioSource PlayEffectAudio(AudioClip clip)
    {
        var audioSource = _CreateNewAudioSource(clip.name, true);
        
        audioSource.clip = clip;
        audioSource.Play();
        return audioSource;
    }

    public AudioManager StopAllEffectAudio()
    {
        foreach (var audio in effectAudioSources)
            audio.Stop();
        return this;
    }

    public AudioManager StopEffectAudio(AudioSource source)
    {
        if (effectAudioSources.Contains(source))
        {
            effectAudioSources.Remove(source);
            source.Stop();
            GameObject.Destroy(source.gameObject);
        }
        return this;
    }

    public AudioManager StopBackgroundAudio(int trackNumber)
    {
        backgroundAudioSources[trackNumber].Stop();
        return this;
    }

    public AudioManager PlayBackgroundAudio(int trackNumber,string audioName,string path = "")
    {
        var clip = GetAudioClip(audioName);
        var audioSource = backgroundAudioSources[trackNumber];
        audioSource.clip = clip;
        audioSource.Play();
        return this;
    }
    
    public AudioClip GetAudioClip(string audioName,string path = "")
    {
        var clip = Resources.Load<AudioClip>("Audios/"+ path + audioName);
        return clip;
    }
    
    private AudioSource _CreateNewAudioSource(string name, bool isEffect)
    {
        var newAudioObj = new GameObject();
        newAudioObj.name = name + "AudioPlayer";
        var audioSource = newAudioObj.AddComponent<AudioSource>();
        switch (isEffect)
        {
            case true:
                newAudioObj.transform.parent = ea.transform;
                effectAudioSources.Add(audioSource);
                break;
            case false:
                newAudioObj.transform.parent = ba.transform;
                backgroundAudioSources.Add(audioSource);
                audioSource.loop = true;
                break;
        }
        return audioSource;
    }
}

public class AudioPiece
{
    public AudioPiece()
    {
        
    }
}
