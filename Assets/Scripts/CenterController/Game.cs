using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    private bool clearDestroy = true;
    // Start is called before the first frame update
    void Awake()
    {
        if (Services.game == null) Init();
        //when scene switch, reinit some of the game system to let them find the items that only shows in the present scene
        else if (Services.game != this) { ReInit();
            clearDestroy = false; Destroy(gameObject);}
        
        DontDestroyOnLoad(gameObject);
    }

    //init must include the start of all the components and the reading of all ths save files
    void Init()
    {
        //import all the data file
        Services.referenceInfo = ReferenceInfo.Instance;
        
        //import the system so it starts working
        Services.game = this;
        Services.gameSettings = new GameSettings();
        Services.gameSettings.Init();
        Services.plotManager = new PlotManager();
        //some logic problem here that doesnt consider the save situation
        //TODO
        Services.plotManager.Init();
        Services.gameStates = new GameStates();
        Services.gameStates.Init();
    }

    void ReInit()
    {
        //update the reference info in specific info files
        Services.referenceInfo = ReferenceInfo.Instance;
        Debug.Log(Services.referenceInfo.ToMainMenuPage[0]);
        
        //update the reference info in all the system managers
        Services.gameStates.ReInitWhenSceneLoad();
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
    }

    private void OnDestroy()
    {   
        if(clearDestroy) ClearDestroy();
    }

    void ClearDestroy()
    {
        //this place is for save and clear everything
        Services.gameStates.Clear();
        Services.gameStates = null;
        Services.plotManager.Clear();
        Services.plotManager = null;
        Services.gameSettings.Clear();
        Services.gameSettings = null;
    }

}
