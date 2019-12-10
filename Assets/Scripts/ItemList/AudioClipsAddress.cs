using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;

public static class AudioClipsAddress
{
    public static Dictionary<string, string> audioClipsAddress = new Dictionary<string, string>();

    public static AudioClip GetClip(string clipName)
    {
        string textAssetLocation;
        audioClipsAddress.TryGetValue(clipName, out textAssetLocation);
        var ac = AssetDatabase.LoadAssetAtPath<AudioClip>(Application.dataPath + "/Sounds/" + textAssetLocation);
        return ac;
    }
}
