using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="MyScriptableObjectData/Data")]
public abstract class Data : ScriptableObject
{
    public Dictionary<string, bool> DataBool;
    public abstract void ResetContract();

}
