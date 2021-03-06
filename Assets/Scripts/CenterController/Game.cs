﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DialADemon.Library;
using DialADemon.Page;
using HedgehogTeam.EasyTouch;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

public enum RunState
{
    Load,
    Play,
    Pause,
}

public class Game : MonoBehaviour
{
    public RunState runState = RunState.Load;
    #region Life circle

    void Awake()
    {
        runState = RunState.Load;
        
        Services.saveManager.Init();

        //import all the data file
        //Services.referenceInfo = FindObjectOfType<ReferenceInfo>();
        Services.easyTouch = FindObjectOfType<EasyTouch>();
        Services.dataContract = ScriptableObject.CreateInstance<Data_Contract>();
        Services.dataContract.ResetContract();
        Services.settings = Resources.Load<Settings>("ScriptableObj/GameSetting");
        //import the system so it starts working
        Services.game = this;
        Services.textManager.Init();
        Services.plotManager.Init();
        Services.pageState.Init();
        Services.phoneManager.Init();

        Services.saveManager.LoadGame();
        runState = RunState.Play;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        
        Services.pageState.Start();
        //Services.eventManager.AddHandler<SceneChanged>(OnSceneChange);

        Input.simulateMouseWithTouches = true;
    }
    
    // Update is called once per frame
    void Update()
    {
        if (runState == RunState.Play)
        {
            Services.plotManager.Update();
            Services.pageState.Update();
            Services.textSequenceTaskRunner.Update();
            Services.textManager.Update();
            Services.phoneManager.Update();
            Services.plotManager.Update();
        }
    }
    #endregion

    #region Application Behavior

    private void OnApplicationPause(bool paused)
    {
        if (paused)
        {
            Save();
        }
    }

    private void OnApplicationQuit()
    {
        Clear();
        Save();
    }

    #endregion

    #region Static functions
    
    //init must include the start of all the components and the reading of all ths save files

    void Clear()
    {
        //this place is for save and clear everything
       // Services.ritualManager.Clear();
        Services.phoneManager.Clear();
        Services.plotManager.Clear();
        Services.textManager.Clear();
        Services.pageState.Clear();
        Services.saveManager.Clear();
    }

    void Save()
    {
        Services.saveManager.SaveGame();
    }
    
    public void ReLoad()
    {
        
        //update the reference info in all the system managers
        //Services.gameStates.ReInitWhenSceneLoad();
    }
    #endregion
}
