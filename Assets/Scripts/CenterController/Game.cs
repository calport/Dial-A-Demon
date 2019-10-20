using System.Collections;
using System.Collections.Generic;
using DialADemon.Library;
using DialADemon.Page;
using HedgehogTeam.EasyTouch;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
    private bool clearDestroy = true;
    #region Life circle

    void Awake()
    {
        if (Services.game == null) Init();
        //when scene switch, reinit some of the game system to let them find the items that only shows in the present scene
        else if (Services.game != this) { clearDestroy = false; Destroy(gameObject);}
        
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        Services.pageState.Start();
        SceneManager.sceneLoaded += OnSceneLoaded;
        //Services.eventManager.AddHandler<SceneChanged>(OnSceneChange);

        Input.simulateMouseWithTouches = true;
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
    }

    void OnDestroy()
    {   
        if(clearDestroy) Clear();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    #endregion

    #region Static functions
    
    //init must include the start of all the components and the reading of all ths save files
    void Init()
    {  
        Services.saveManager.Init();
        Services.saveManager.LoadGame();
        Services.textManager.Load();
        Services.plotManager.Load();
        //import all the data file
        //Services.referenceInfo = FindObjectOfType<ReferenceInfo>();
        Services.easyTouch = FindObjectOfType<EasyTouch>();
        Services.gameSettings.Init();
        Services.dataContract = ScriptableObject.CreateInstance<Data_Contract>();
        Services.dataContract.ResetContract();
        //import the system so it starts working
        Services.game = this;
        Services.textManager.Init();
        
        //some logic problem here that doesnt consider the save situation
        //TODO
        Services.plotManager.Init();
        Services.pageState.Init();
      
    }
    
    public void ReInit()
    {
        
        //update the reference info in all the system managers
        //Services.gameStates.ReInitWhenSceneLoad();
    }

    void Clear()
    {
        //this place is for save and clear everything
                Services.pageState.Clear();
                Services.plotManager.Clear();
                Services.gameSettings.Clear();
                
                Services.plotManager.Save();
                Services.textManager.Save();
                Services.saveManager.SaveGame();
                Services.saveManager.Clear();
                
    }

    #endregion
    
    #region Event
    
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ReInit();
    }
    
    #endregion
}
