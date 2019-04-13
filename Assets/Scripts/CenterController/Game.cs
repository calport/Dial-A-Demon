using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    //init must include the start of all the components and the reading of all ths save files
    void Init()
    {
        Services.gameSettings = new GameSettings();
        Services.gameSettings.Init();
        Services.plotManager = new PlotManager();
        //some logic problem here that doesnt consider the save situation
        //TODO
        Services.plotManager.Init();
        Services.gameStates = new GameStates();
        Services.gameStates.Init();
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
        Services.gameStates.Clear();
        Services.gameStates = null;
        Services.plotManager.Clear();
        Services.plotManager = null;
        Services.gameSettings.Clear();
        Services.gameSettings = null;
    }

}
