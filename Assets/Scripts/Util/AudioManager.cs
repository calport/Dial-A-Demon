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
        private AudioManager _parent;
        private readonly SlowUpdate _su = new SlowUpdate(10);
        

        void Update()
        {
            if (_su.detect)
                _CleanDeadSources();
        }

        public void Init(AudioManager am)
        {
            _parent = am;
        }

        private void _CleanDeadSources()
        {
            foreach (var piece in _parent.discardedPieces)
            {
                if(_parent.trackingPieces.Contains(piece))
                   _parent.trackingPieces.Remove(piece);
            }
            
            _parent.discardedPieces.Clear();
            
            foreach (var audioPiece in _parent.trackingPieces)
                if (!audioPiece.audioSource.isPlaying && audioPiece.audioSource.time/audioPiece.audioSource.clip.length > 0.99f)
                    audioPiece.Stop();
        }
        
    }

    private AudioManagerMonoBehaviour _audioManagerMonoBehaviour;
    public GameObject ea { get; private set; }
    public GameObject ba { get; private set; }
    public List<AudioPiece> trackingPieces = new List<AudioPiece>();
    public List<AudioPiece> discardedPieces = new List<AudioPiece>();
    public List<AudioSource> backgroundAudioTrack = new List<AudioSource>();
    
    public AudioManager()
    {
        
        trackingPieces = new List<AudioPiece>(); 
        discardedPieces = new List<AudioPiece>();
        backgroundAudioTrack = new List<AudioSource>();
        _Init(); 
    }

    private void _Init()
    {
        //create the main game object
        var am = new GameObject();
        am.name = "AudioManager";
        _audioManagerMonoBehaviour = am.AddComponent<AudioManagerMonoBehaviour>();
        _audioManagerMonoBehaviour.Init(this);
        
        //create seperate obj for storing the audio sources
        ea = new GameObject();
        ea.name = "EffectAudioSources";
        ea.transform.parent = am.transform;
        
        ba = new GameObject();
        ba.name = "BackgroundAudioSources";
        ba.transform.parent = am.transform;

        for (int i = 0; i < 3; i++)
            _CreateNewAudioSource("BackgroundTrack" + i,ba,true);
        
        GameObject.DontDestroyOnLoad(am);
        
    }

/*    public AudioSource PlayEffectAudio(string audioName, string path = "")
    {
        var clip = CreateAudioPiece(audioName,path);
        var audioSource = _CreateNewAudioSource(audioName,true);
        
        audioSource.clip = clip;
        audioSource.Play();
        return audioSource;
    }*/

/*    public AudioSource PlayEffectAudio(AudioClip clip)
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
        backgroundAudioTrack[trackNumber].Stop();
        return this;
    }

    public AudioManager PlayBackgroundAudio(int trackNumber,string audioName,string path = "")
    {
        var clip = CreateAudioPiece(audioName);
        var audioSource = backgroundAudioTrack[trackNumber];
        audioSource.clip = clip;
        audioSource.Play();
        return this;
    }*/
    
    public AudioPiece CreateAudioPiece(string audioName,string path = "", int trackNumber = -1)
    {
        var clip = Resources.Load<AudioClip>("Audios/"+ path + audioName);
        return CreateAudioPiece(clip, trackNumber);
    }
    
    public AudioPiece CreateAudioPiece(AudioClip clip, int trackNumber = -1)
    {
        return new AudioPiece(clip, trackNumber);
    }

    public AudioClip GetAudioClip(string audioName, string path = "")
    {
        return Resources.Load<AudioClip>("Audios/"+ path + audioName);
    }
    
    private AudioSource _CreateNewAudioSource(string name, GameObject parent, bool isLoop)
    {
        var newAudioObj = new GameObject();
        newAudioObj.name = name + "AudioPlayer";
        var audioSource = newAudioObj.AddComponent<AudioSource>();
        newAudioObj.transform.parent = parent.transform;
        audioSource.loop = isLoop;
        
        return audioSource;
    }
}

public class AudioPiece
{
    public AudioPiece(AudioClip clip, int trackNumber = -1)
    {
        audioClip = clip;
        this.trackNumber = trackNumber;
    }
    
    public AudioClip audioClip { get; private set; }
    public AudioSource audioSource { get; private set; }
    public int trackNumber{ get; private set; }
    public Action onAudioFinished;
    

    private GameObject _sourceParent
    {
        get { return trackNumber == -1 ? am.ea : am.ba; }
    }
    private AudioManager am = Services.audioManager;
    
    public AudioPiece Play()
    {
        if (ReferenceEquals(audioSource, null))
        {
            if (trackNumber != -1) 
                audioSource = am.backgroundAudioTrack[trackNumber];
            else
            {
                var newAudioObj = new GameObject();
                newAudioObj.name = audioClip.name + "AudioPlayer";
                audioSource = newAudioObj.AddComponent<AudioSource>();
                newAudioObj.transform.parent = _sourceParent.transform;
            }
        }
        
        if(!am.trackingPieces.Contains(this))
            am.trackingPieces.Add(this);
        audioSource.clip = audioClip;
        audioSource.Play();
        return this;
    }
    
    public AudioPiece Stop()
    {
        if (ReferenceEquals(audioSource, null)) return this;
        audioSource.Stop();
        if(am.trackingPieces.Contains(this))
            am.discardedPieces.Add(this);
        if (trackNumber == -1) GameObject.Destroy(audioSource.gameObject);
        else audioSource.clip = null;
        return this;
    }

    public AudioPiece Pause()
    {
        if (ReferenceEquals(audioSource, null)) return this;
        audioSource.Pause();
        return this;
    }
}
