using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/SettingScriptableObject", order = 1)]
public class Settings : ScriptableObject
{
   public bool isVibrationOn;
   [Range(0.0f, 1.0f)]
   public float effectVolume;
   [Range(0.0f, 1.0f)]
   public float bgmVolume;
}
