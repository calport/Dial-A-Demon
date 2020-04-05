using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.XR;

public static class PhoneFileAddress
{
    public static Dictionary<Type, string> phoneFileAddress = new Dictionary<Type, string>{{typeof(PlotManager.Day1_Phone1), "DemonCall3"},};
    public static Dictionary<Type, string> voiceMailAddress = new Dictionary<Type, string>();
    
    public static AudioClip GetPhoneClipName(Type plotType = null)
    {
        string textAssetLocation;
        phoneFileAddress.TryGetValue(plotType, out textAssetLocation);
        var ac = Services.audioManager.GetAudioClip(textAssetLocation,"Audios/PhoneCall");
        return ac;
    }
    
    public static AudioClip GetVoiceMailClip(Type plotType)
    {
        string textAssetLocation;
        voiceMailAddress.TryGetValue(plotType, out textAssetLocation);
        var ac = Resources.Load<AudioClip>("Sounds/" + textAssetLocation);
        return ac;
    }
}
