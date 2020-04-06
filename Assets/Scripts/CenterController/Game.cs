using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DialADemon.Library;
using DialADemon.Page;
using HedgehogTeam.EasyTouch;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
    #region Life circle

    void Awake()
    {
        if (Services.game == null) Init();
        //when scene switch, reinit some of the game system to let them find the items that only shows in the present scene
        
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        Services.pageState.Start();
        //Services.eventManager.AddHandler<SceneChanged>(OnSceneChange);

        Input.simulateMouseWithTouches = true;
        var time = DateTime.Now;
        print(time);
        var json = JsonUtility.ToJson((SerializeManager.JsonDateTime) time);
        print(json);
        DateTime timeFromJson = JsonUtility.FromJson<SerializeManager.JsonDateTime>(json);
        print(timeFromJson);
    }
    
    // Update is called once per frame
    void Update()
    {
        Services.plotManager.Update();
        /*if (Services.gameSettings.SpeedPlot)
        {
            Services.plotManager.SpeedUpdate();
        }
        else
        {
            Services.plotManager.RegularUpdate();
        }*/

        Services.pageState.Update();
        Services.textSequenceTaskRunner.Update();
        Services.textManager.Update();
        Services.phoneManager.Update();
        Services.plotManager.Update();
    }

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
    void Init()
    {  
        Services.saveManager.Init();
        Services.saveManager.LoadGame();
        
        //import all the data file
        //Services.referenceInfo = FindObjectOfType<ReferenceInfo>();
        Services.easyTouch = FindObjectOfType<EasyTouch>();
        Services.dataContract = ScriptableObject.CreateInstance<Data_Contract>();
        Services.dataContract.ResetContract();
        //import the system so it starts working
        Services.game = this;
        Services.textManager.Init();
        
        //some logic problem here that doesnt consider the save situation
        //TODO
        Services.plotManager.Init();
        Services.pageState.Init();
        Services.phoneManager.Init();
      
    }

    void Clear()
    {
        //this place is for save and clear everything
        Services.phoneManager.Clear();
        Services.pageState.Clear();
        Services.plotManager.Clear();
        Services.saveManager.Clear();
        Services.textManager.Clear();
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
