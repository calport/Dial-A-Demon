using System.Collections;
using System.Collections.Generic;
using HedgehogTeam.EasyTouch;
using UnityEngine;
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
        Services.gameStates.Start();
        SceneManager.sceneLoaded += OnSceneLoaded;
        //Services.eventManager.AddHandler<SceneChanged>(OnSceneChange);

        Input.simulateMouseWithTouches = true;
    }
    
    // Update is called once per frame
    void Update()
    {
        if (Services.gameSettings.SpeedPlot)
        {
            Services.plotManager.SpeedUpdate();
        }
        else
        {
            Services.plotManager.RegularUpdate();
        }

        Services.gameStates.Update();
        Services.textStates.Update();
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
        //import all the library utils

        //import all the data file
        Services.referenceInfo = FindObjectOfType<ReferenceInfo>();
        Services.easyTouch = FindObjectOfType<EasyTouch>();
        Services.gameSettings = new GameSettings();
        Services.gameSettings.Init();
        //import the system so it starts working
        Services.game = this;
        Services.textManager = new TextManager();
        Services.textManager.Init();
        Services.plotManager = new PlotManager();
        //some logic problem here that doesnt consider the save situation
        //TODO
        Services.plotManager.Init();
        Services.gameStates = new GameStates();
        Services.gameStates.Init();
        Services.textStates = new TextStates();
        Services.textStates.Init();
    }
    
    public void ReInit()
    {
        //update the reference info in specific info files
        Debug.Log(FindObjectOfType<ReferenceInfo>());
        Services.referenceInfo = ReferenceInfo.Instance;
        
        //update the reference info in all the system managers
        //Services.gameStates.ReInitWhenSceneLoad();
    }

    void Clear()
    {
        //this place is for save and clear everything
                Services.gameStates.Clear();
                Services.gameStates = null;
                Services.plotManager.Clear();
                Services.plotManager = null;
                Services.gameSettings.Clear();
                Services.gameSettings = null;
                
    }

    #endregion
    
    #region Event
    
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ReInit();
    }
    
    #endregion
}
