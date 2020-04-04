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
    private Dictionary<string,AudioClip> _clipDic = new Dictionary<string, AudioClip>(0);
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

    public AudioManager PlayEffectAudio(string audioName)
    {
        var clip = _GetAudioClip(audioName);
        var audioSource = _CreateNewAudioSource(audioName,true);
        
        audioSource.clip = clip;
        audioSource.Play();
        return this;
    }

    public AudioManager StopAllEffectAudio()
    {
        foreach (var audio in effectAudioSources)
            audio.Stop();
        return this;
    }

    public AudioManager PlayBackgroundAudio(string audioName,int trackNumber)
    {
        var clip = _GetAudioClip(audioName);
        var audioSource = backgroundAudioSources[trackNumber];
        audioSource.clip = clip;
        audioSource.Play();
        return this;
    }
    
    private AudioClip _GetAudioClip(string audioName)
    {
        if (_clipDic.ContainsKey(audioName))
            return  _clipDic[audioName];
        
        var clip = Resources.Load<AudioClip>("Audios/"+audioName);
        _clipDic.Add(audioName,clip);
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
