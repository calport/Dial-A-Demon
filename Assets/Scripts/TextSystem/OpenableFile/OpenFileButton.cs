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
            return tfp.document;
        }
    }

    PlotManager.TextFilePlot tfp
    {
        get
        {
            return Services.plotManager.GetOrCreatePlots(plotType) as PlotManager.TextFilePlot;
        }
    }

    private PageState ps;

    public void OnSimpleTap()
    {
        ps.ChangeGameState(ps.CSM.stateList.Front_Main);
        document.transform.localScale = new Vector3(0.1f,0.1f,0.1f);
        PlotManager.TextFilePlot tfp =
            Services.plotManager.GetOrCreatePlots(plotType) as PlotManager.TextFilePlot;
        foreach (var item in ps.GetGameState("Front_Main").relatedObj)
        {
            if (item.CompareTag("PageObj"))
            {
                document.transform.parent = item.transform;
            }
        }

        document.transform.DOScale(1f, 1f);
    }
}
