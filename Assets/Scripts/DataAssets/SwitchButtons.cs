using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[CreateAssetMenu(menuName="MyScriptableObjectData/SwitchButtons")]
public class SwitchButtons : ScriptableObject
{
    public List<Button> ToTextingPage;
    public List<Button> ToPhoneCallPage;
    public List<Button> ToSettingPage;
    public List<Button> ToFinalRitualPage;
    public List<Button> ToMainMenuPage;

}
