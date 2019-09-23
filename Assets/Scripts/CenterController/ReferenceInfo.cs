﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DialADemon.Page;

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

//INTENT: Creates reference list of GameObjects, buttons, and main scenes rendered into each screen. 
//USAGE: This script is attached to GameObject called GameReference in the MainSceen 

public class ReferenceInfo : MonoSingleton<ReferenceInfo>
{
    public List<Pages> pageList = new List<Pages>();
    
    public List<Type> GameStatesRefList = new List<Type>(){typeof(GameStates.MenuPage),typeof(GameStates.TextingPage),typeof(GameStates.PhoneCallPage),
        typeof(GameStates.DialPage),typeof(GameStates.SettingPage),typeof(GameStates.OpeningRitualPage),typeof(GameStates.FinalRitualPage)};
    
    //the size of the array is the same as the number of the page; 
    //there is a separate script for each of these lists and dictionaries and that script is attached to accurate GameObjects
    public List<Button>[] ToSpecifiedPage = new List<Button>[System.Enum.GetNames (typeof(Page)).Length+1];
    public Dictionary<string, int> SceneDic = new Dictionary<string, int>();
    public Dictionary<string, GameObject> MenuPage = new Dictionary<string, GameObject>();
    public Dictionary<string, GameObject> BigPage = new Dictionary<string, GameObject>();
    private void Awake()
    {
        Services.referenceInfo = this;
        
        for(int i = 0; i <ToSpecifiedPage.Length; i++)
        {
            ToSpecifiedPage[i] = new List<Button>();
        }
        
        SceneDic.Add("Main",0);
        
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
