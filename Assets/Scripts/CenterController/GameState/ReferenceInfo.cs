using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ReferenceInfo : MonoSingleton<ReferenceInfo>
{
    public List<Button> ToTextingPage = new List<Button>();
    public List<Button> ToPhoneCallPage= new List<Button>();
    public List<Button> ToSettingPage= new List<Button>();
    public List<Button> ToFinalRitualPage= new List<Button>();
    public List<Button> ToMainMenuPage= new List<Button>();

    public Dictionary<string, int> SceneDic = new Dictionary<string, int>();

    private void Awake()
    {
        SceneDic.Add("Main",1);
        SceneDic.Add("Setting",2);
        SceneDic.Add("OpeningRitual",0);
        Debug.Log("finish");
    }

    public int GetSceneWithName(string sceneName)
    {
        if(SceneDic.ContainsKey(sceneName)) {int num =SceneDic[sceneName]; return num; }
        else
        {
            Debug.Log("Can't find the scene with the given name");
            return SceneDic.Count;
        }           
    }
}
