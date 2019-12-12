using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class AudioClipsAddress
{
    public static Dictionary<string, string> audioClipsAddress = new Dictionary<string, string>();

    public static AudioClip GetClip(string clipName)
    {
        string textAssetLocation;
        audioClipsAddress.TryGetValue(clipName, out textAssetLocation);
        var ac = Resources.Load<AudioClip>("Sounds/" + textAssetLocation);
        return ac;
    }
}
