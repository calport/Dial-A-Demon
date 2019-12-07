using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
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
    

    public void OnSimpleTap()
    {
            //Services.textStates.ChangeGameState<TextStates.NormalText>(new TextStates.NormalText());
            document.transform.DOScale(0.1f, 1f).onComplete = delegate { Destroy(document); };
    }
}
