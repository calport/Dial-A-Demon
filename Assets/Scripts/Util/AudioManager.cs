using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager
{
    public static void PlaySound(AudioSource source, AudioClip clip)
    {
        source.clip = clip;
        source.Play();
    }

    public static void SoundFade(AudioSource source)
    {
        
    }
}
