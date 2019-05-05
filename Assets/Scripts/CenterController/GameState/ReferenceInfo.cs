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

    public List<GameObject> MenuPageObj = new List<GameObject>();
    public List<GameObject> BigPageObj = new List<GameObject>();
    public Dictionary<string, int> SceneDic = new Dictionary<string, int>();
    public Dictionary<string, GameObject> MenuPage = new Dictionary<string, GameObject>();
    public Dictionary<string, GameObject> BigPage = new Dictionary<string, GameObject>();
    private void Awake()
    {
        SceneDic.Add("Main",1);
        SceneDic.Add("OpeningRitual",0);
        
        MenuPage.Add("MenuPage", MenuPageObj.Find(x => x.name == "MenuPage"));
        MenuPage.Add("TextPage", MenuPageObj.Find(x => x.name == "TextingPage"));
        MenuPage.Add("DialPage",MenuPageObj.Find(x => x.name =="DialPage"));
        MenuPage.Add("DemonCallingPage",MenuPageObj.Find(x => x.name =="DemonCallingPage"));
        MenuPage.Add("FinalRitualPage",MenuPageObj.Find(x => x.name =="FinalRitualPage"));
        
        BigPage.Add("MainMenu", BigPageObj.Find(x => x.name == "MainMenu"));
        BigPage.Add("Setting", BigPageObj.Find(x => x.name == "Setting"));
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
