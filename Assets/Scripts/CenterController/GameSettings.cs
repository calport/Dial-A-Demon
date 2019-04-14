using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettings
{                       
    private bool _speedPlot = false;

    public void SetSpeedPlot(bool speedPlotOnOrOff)
    {
        _speedPlot = speedPlotOnOrOff;
    }

    public bool SpeedPlot
    {
        get { return _speedPlot; }
    }

    public void Init()
    {
        //if there has save file, read it.
        //otherwise create one and save all the initial settings
    }
    
    public void Clear(){}
}
