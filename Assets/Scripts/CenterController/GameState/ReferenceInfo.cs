using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum Page
{
    MainMenu,
    TextingPage,
    PhoneCallPage,
    DialPage,
    SettingPage,
    OpeningRitualPage,
    FinalRitualPage
}
public class ReferenceInfo : MonoSingleton<ReferenceInfo>
{
    public List<Type> GameStatesRefList = new List<Type>(){typeof(GameStates.MenuPage),typeof(GameStates.TextingPage),typeof(GameStates.PhoneCallPage),
        typeof(GameStates.DialPage),typeof(GameStates.SettingPage),typeof(GameStates.OpeningRitualPage),typeof(GameStates.FinalRitualPage)};
    
    //the size of the array is the same as the number of the page;
    public List<GameObject>[] CameraRenderingItem = new List<GameObject>[System.Enum.GetNames (typeof(Page)).Length];
    public List<Button>[] ToSpecifiedPage = new List<Button>[System.Enum.GetNames (typeof(Page)).Length+1];
    public Dictionary<string, int> SceneDic = new Dictionary<string, int>();
    public Dictionary<string, GameObject> MenuPage = new Dictionary<string, GameObject>();
    public Dictionary<string, GameObject> BigPage = new Dictionary<string, GameObject>();
    private void Awake()
    {
        Services.referenceInfo = this;
        for(int i = 0; i <CameraRenderingItem.Length; i++)
        {
            CameraRenderingItem[i] = new List<GameObject>();
        }
        
        for(int i = 0; i <ToSpecifiedPage.Length; i++)
        {
            ToSpecifiedPage[i] = new List<Button>();
        }
        
        Debug.Log(CameraRenderingItem[2]);
        SceneDic.Add("Main",1);
        SceneDic.Add("OpeningRitual",0);
        
        //BigPage.Add("MainMenu", BigPageObj.Find(x => x.name == "MainMenu"));
        //BigPage.Add("Setting", BigPageObj.Find(x => x.name == "Setting"));
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
