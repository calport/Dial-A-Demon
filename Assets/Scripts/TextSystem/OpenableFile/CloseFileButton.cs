using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DialADemon.Page;
using HedgehogTeam.EasyTouch;
using UnityEngine;

public class CloseFileButton : MonoBehaviour
{
    public Type plotType;
    public GameObject bubble
    {
        get
        {
            PlotManager.TextFilePlot tfp = Services.plotManager.GetOrCreatePlots(plotType) as PlotManager.TextFilePlot;
            return tfp.bubble;
        }
    }
    public GameObject document
    {
        get
        {
            PlotManager.TextFilePlot tfp = Services.plotManager.GetOrCreatePlots(plotType) as PlotManager.TextFilePlot;
            return tfp.document;
        }
    }
    private PageState ps
    {
        get
        {
            return Services.pageState;
        }
    }
    private PlotManager.TextFilePlot tfp
    {
        get
        {
            return Services.plotManager.GetOrCreatePlots(plotType) as PlotManager.TextFilePlot;
        }
    }

    public void OnSimpleTap()
    {
        Destroy(document);
        ps.TransitToPreviousState();
            
    }
}
