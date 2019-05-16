using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="MyScriptableObjectData/ContractData")]
public class ContractData : ScriptableObject
{
    public bool Notification = false;
    public bool Mail = false;
    public bool Camera = false;
    public bool Album = false;
    public bool Location = false;
    public bool YourPhone = false;
    public bool YourLove = false;
    public bool Yourself = false;

    public void ResetContract()
    {
        Notification = false;
        Mail = false;
        Camera = false;
        Album = false;
        Location = false;
        YourPhone = false;
        YourLove = false;
        Yourself = false;
    }
}
