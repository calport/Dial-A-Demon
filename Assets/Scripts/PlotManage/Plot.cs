using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;
using Yarn.Unity;
using Yarn.Unity.Example;

public abstract class Plot
{
    
    protected int Day;
    protected int Hour;
    protected int Minute;
    protected DateTime plotStartTime;
    
    protected bool _plotFinished = false;
    public void PlotFinishedStateChange(bool state)
    {
        _plotFinished = state;
    }
    //This will appear in every subclass so object can set time when they are created
    /*

    public Plot(float day, float hour, float second)
    {
        _day = day;
        _hour = hour;
        _second = second;
    }*/
    
    //prepare to start the plot before it update
    public virtual void Init(){}
    
    public virtual void Update(){}
    
    public virtual void Clean(){}

    public virtual void Save(){}
    
    public virtual bool IsAbleToSetTimeDown()
    {
        return true;}

    public virtual bool IsAbleToStart()
    {
        var dt = System.DateTime.Now;
        var timeSpan = dt - plotStartTime;
        return timeSpan.TotalSeconds > 0;
    }

    public virtual bool IsFinished()
    {
        return _plotFinished;
    }
    
    public virtual void RearrangeTime(){}

    public void ShrinkTime()
    {
        var dt = DateTime.Now;
        plotStartTime = dt.AddSeconds(10f);
    }
    
    public virtual void NewPlotNote(){}
    
    public virtual void CurrentPlotNote(){}

    public virtual void AddPendingPlot()
    {
        var plotNum = Services.plotManager.PlotNumber + 1;
        Services.plotManager.AddPlot(Services.plotManager.TotalTask[plotNum]);
    }
}

public class TextPlot : Plot
{
    private string _fileLocation;
    private DialogueRunner _dialogueRunner;
    private DialogueUIBehaviour _dialogueUI;
    private Plot prePlot;
    public TextPlot(int day, int hour, int minute, string fileLocation, Plot prePlot)
    {
        Day = day;
        Hour = hour;
        Minute = minute;
        _fileLocation = fileLocation;
    }

    public override void Init()
    {
        //find the dialogueRunner to control and get info
        _dialogueRunner = GameObject.FindObjectOfType<DialogueRunner>();
        _dialogueRunner.sourceText[0] = Resources.Load(_fileLocation) as TextAsset;
        _dialogueUI = _dialogueRunner.dialogueUI;
        //register the dialogueRunner
        _dialogueUI.controlPlot = this;
    }
    
    public virtual void Update(){}

    public override void Clean()
    {
        _dialogueRunner.sourceText[0] = null;
        //stop the dialogrunner
        //unregister the control plot for dialoguerunner
        _dialogueUI.controlPlot = null;
    }

    public override void Save()
    {
        //save the node
    }
    
    public override bool IsAbleToSetTimeDown()
    {
        if (prePlot != null) return Services.plotManager.DeletedPlot.Contains(prePlot);
        return true;
    }
   
    public virtual void RearrangeTime(){}

    public void ShrinkTime()
    {
        var dt = DateTime.Now;
        plotStartTime = dt.AddSeconds(10f);
    }
    
    public virtual void NewPlotNote(){}
    
    public virtual void CurrentPlotNote(){}

}
