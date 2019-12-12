using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DialADemon.Page;
using HedgehogTeam.EasyTouch;
using UnityEngine;

public class OpenFileButton : MonoBehaviour
{
    public Type plotType;

    public GameObject document
    {
        get
        {
            PlotManager.TextFilePlot tfp = Services.plotManager.GetOrCreatePlots(plotType) as PlotManager.TextFilePlot;
            return tfp.documentPre;
        }
    }

    PlotManager.TextFilePlot tfp
    {
        get
        {
            return Services.plotManager.GetOrCreatePlots(plotType) as PlotManager.TextFilePlot;
        }
    }

    private PageState ps
    {
        get
        {
            return Services.pageState;
        }
    }

    public void OnSimpleTap()
    {
        ps.ChangeGameState(ps.CSM.stateList.Front_Main);
        tfp.CreateDocument();
    }
}
