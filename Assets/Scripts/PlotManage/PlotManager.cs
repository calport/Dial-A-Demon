﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using DialADemon.Page;
using Ink.Runtime;
using UnityEngine;
//using UnityEngine.iOS;

public class PlotManagerInfo
{
    public DateTime baseTime;
}
public class PlotManager 
{
    public Dictionary<Plot, DateTime> Calendar = new Dictionary<Plot, DateTime>();
    public bool isSpeedMode = false;
    
    private PlotManagerInfo _plotInfo = new PlotManagerInfo();
    //Calendar system
    private Dictionary<Type, Plot> _plots = new Dictionary<Type, Plot>();
    //
    private Dictionary<Type, Plot> _globalCheckPlot = new Dictionary<Type, Plot>();
    private string _currentTextAssetLocation;


    public void Init()
    {

        //clear notification
        //UnityEngine.iOS.NotificationServices.ClearLocalNotifications();

        //read the save plot
        //AddPlot(TotalTask[_plotNumber]);
        //read the saved plot start time
        //TODO
        
        //for the first time upload
        InitBaseTime();
        InitLeadPlot();
    }
    
    public void Update()
    {
        List<Plot> unusedPlot = new List<Plot>();
        //constantly update the plots' state
        for(int i = 0; i< Calendar.Keys.ToList().Count;i++)
        {
            KeyValuePair<Plot, DateTime> plotPair = Calendar.ElementAt(i);
            if (plotPair.Value < DateTime.Now && plotPair.Key.plotState == plotState.isOnCalendar)
            {
                plotPair.Key.plotState = plotState.isReadyToPlay;
                plotPair.Key.Init();
                continue;
            }

            if (plotPair.Key.plotState == plotState.isReadyToPlay)
            {
                plotPair.Key.plotState = plotState.isPlaying;
                plotPair.Key.Start();
                continue;
            }
            
            if (plotPair.Key.plotState == plotState.isPlaying)
            {
                plotPair.Key.Update();
                continue;
            }

            if (plotPair.Key.plotState == plotState.isFinished)
            {
                plotPair.Key.Clear();
                unusedPlot.Add(plotPair.Key);
                continue;
            }

            if (plotPair.Key.plotState == plotState.isBreak)
            {
                plotPair.Key.Break();
                unusedPlot.Add(plotPair.Key);
                continue;
            }
        }
        
        //clean finished plots
        foreach (var plot in unusedPlot)
        {
            Calendar.Remove(plot);
        }
        
        foreach (var plot in _globalCheckPlot.Values)
        {
            if (plot.plotState == plotState.isStandingBy || plot.plotState == plotState.isChecking)
            {
                bool isLoadable;
                
                if (!isSpeedMode) isLoadable = plot.CheckLoad();
                else isLoadable = plot.SpeedCheckLoad();
                
                if (isLoadable)
                {
                    plot.AddToCalendar();
                }
            }
        }
    }

    public void Clear()
    {
        //save all the list of plots
        //save all the plot start time
        //TODO
        //add notification
    }

    public void ClearCalendar()
    {
        //including save and clean data
        //save TODO
        //clean
        Calendar.Clear();
        _plots.Clear();
        
    }
    void InitBaseTime()
    {
        if (!File.Exists(Application.dataPath + "/Resources/Json/Time.json"))
        {
            _plotInfo.baseTime = DateTime.Now;
        }
        else
        {
            StreamReader sr = new StreamReader(Application.dataPath + "/Resources/Json/Time.json");
            string json = sr.ReadToEnd();
            if (json.Length > 0)
            {
                _plotInfo = JsonUtility.FromJson<PlotManagerInfo>(json);
            }
        }
    }

    void InitLeadPlot()
    {
        var root = GetOrCreatePlots<RootPlot>();
        root.AddToCalendar();
    }

    public T GetOrCreatePlots<T>() where T : Plot
    {
        Plot outPlot;
        _plots.TryGetValue(typeof(T), out outPlot);
        if (outPlot != null)
        {
            return outPlot as T;
        }    
        else
        {
            T newPlot = Activator.CreateInstance<T>();
            newPlot.parent = this;
            newPlot.plotState = plotState.isChecking;
            if(newPlot.isGlobalCheck) _globalCheckPlot.Add(newPlot.GetType(),newPlot);
            _plots.Add(typeof(T), newPlot);
            
            return newPlot;
        }
    }


    public Plot GetOrCreatePlots(Type type)
    {
        Plot outPlot;
        _plots.TryGetValue(type, out outPlot);
        if (outPlot != null)
        {
            return outPlot;
        }
        else
        {
            Plot newPlot = Activator.CreateInstance(type) as Plot;
            newPlot.parent = this;
            newPlot.plotState = plotState.isChecking;
            if (newPlot.isGlobalCheck) _globalCheckPlot.Add(newPlot.GetType(), newPlot);
            _plots.Add(type, newPlot);
            
            return newPlot;
        }
    }

    public void StartPlot<T>() where T : Plot
    {
        ClearCalendar();
        var newPlot = GetOrCreatePlots<T>();
        _plotInfo.baseTime = DateTime.Now;
        newPlot.SetRelatedSpanToZero();
        newPlot.AddToCalendar();
    }

    public void Save()
    {
        List<PlotInfo> plotInfos = new List<PlotInfo>();
        foreach (var plotPair in _plots)
        {
            //save each plot
            plotPair.Value.Save();
            //build the plot info list
            PlotInfo info = new PlotInfo();

            if (Calendar.ContainsKey(plotPair.Value))
            {
                info.isOnCalendar = true;
                DateTime time;
                Calendar.TryGetValue(plotPair.Value, out time);
                info.startTime = time;
            }
            else info.isOnCalendar = false;
            
            info.plotType = plotPair.Value.GetType();
            info.plotState = plotPair.Value.plotState;
            info.relaSpan = plotPair.Value.relaSpan;
            
            plotInfos.Add(info);
        }
        Services.saveManager.plotInfo = plotInfos;
    }

    public void Load()
    {
        if (File.Exists(Services.saveManager.saveJsonPath))
        {
            var plotInfo = Services.saveManager.plotInfo;
            foreach (var info in plotInfo)
            {
                var plot = GetOrCreatePlots(info.plotType);
                plot.plotState = info.plotState;
                plot.relaSpan = info.relaSpan;
                if (info.isOnCalendar) Calendar.Add(plot, info.startTime);
                plot.Reload();
            }
        }
    }
    //when the app quit, save all the info that needed
    //add notification and other stuff to keep track the gameflow
    

    public enum plotState
    {
        isStandingBy,
        isChecking,
        isOnCalendar,
        isReadyToPlay,
        isPlaying,
        isFinished,
        isAbandoned,
        isBreak,
    }


    public abstract class Plot
    {
        public PlotManager parent;
        protected List<Type> _parentPlots = new List<Type>();
        //these plots are only important for a special speed-up mode
        protected List<Type> _requiredPrePlots = new List<Type>();
        protected List<Type> _childPlots = new List<Type>();
        
        public plotState plotState = plotState.isStandingBy;
        
        //true when the checking 
        public  bool isGlobalCheck = false;

        public  bool isInstance = false;
        
        //calendar time
        protected Type _referPlot;
        public TimeSpan relaSpan;

        public TimeSpan absSpan
        {
            get
            {
                TimeSpan time = relaSpan;
                Plot rp = this;
                while(rp._referPlot!= null)
                {
                    rp = parent.GetOrCreatePlots(rp._referPlot);
                    time += rp.relaSpan;
                }

                return time;
            }
        }
        public DateTime plotStartTime
        {
            get { return parent._plotInfo.baseTime.Add(absSpan); }
        }

        public virtual void Reload(){}
        //preInit when the plot is created

        //int when the plot is added to calendar
        public virtual void Init()
        {
        }
        //start when it is time for the plot to run
        public virtual void Start(){}
        public virtual void Update()
        {
        }

        public virtual void Clear()
        {
        }

        public virtual void Break(){}
        public virtual void Save()
        {
        }

        public void AddToCalendar()
        {
            if (plotState == plotState.isChecking || plotState == plotState.isStandingBy)
            {
                parent.Calendar.Add(this,plotStartTime);
                this.plotState = plotState.isOnCalendar;
            }
        }

        public void CheckChild()
        {
            foreach (var childPlot in _childPlots)
            {
                var plotInit = parent.GetOrCreatePlots(childPlot);
                if (plotInit.plotState == plotState.isChecking || plotInit.plotState == plotState.isStandingBy)
                {
                    plotInit.plotState = plotState.isChecking;
                    bool isLoadable;
                    
                    if (!parent.isSpeedMode) isLoadable = plotInit.CheckLoad();
                    else isLoadable = plotInit.SpeedCheckLoad();
                    
                    if (isLoadable)
                    {
                        plotInit.AddToCalendar();
                    }
                }
            }
        }
        
        //check if the plot itself is loaded
        public virtual bool CheckLoad()
        {
            return false;
        }

        public bool SpeedCheckLoad()
        {
            int i = 0;
            foreach (var plotType in _requiredPrePlots)
            {
                var plot = parent.GetOrCreatePlots(plotType);
                if (plot.plotState != plotState.isFinished)
                {
                    i++;
                }
            }
            if(i==0)
            {
                SetRelatedSpanToZero();
                return true;
            }
            else
            {
                return false;
            }

        }
        
        public void SetRelatedSpanToZero()
        {
            relaSpan= TimeSpan.Zero;
        }
    }
    
    public class RootPlot : Plot
    {
        public RootPlot()
        {
            _referPlot = null;
            relaSpan = TimeSpan.Zero;
            _childPlots = new List<Type>{typeof(Day1_Text1)};
        }

        public override void Start()
        {
            plotState = plotState.isFinished;
        }

        public override void Clear()
        {
            CheckChild();
        }
    }
    
    public class TextPlot : Plot
    {
        public TextAsset ta;
        internal string textAssetLocation;

        public override void Reload()
        {
            if (plotState == plotState.isPlaying || plotState == plotState.isReadyToPlay)
            {
                Services.textManager.currentStory = new Story(ta.text);
                Services.textManager.currentStory.state.LoadJson(Services.textManager.inkJson);
                Services.eventManager.AddHandler<TextFinished>(delegate { OnTextFinished(); });
            }
        }

        public override void Start()
        {
            Services.textManager.StartNewStory(ta);
            Services.eventManager.AddHandler<TextFinished>(delegate{OnTextFinished();});
        }
        
        public override void Clear()
        {
            CheckChild();
            Services.eventManager.RemoveHandler<TextFinished>(delegate{OnTextFinished();});
        }
        
        public void OnTextFinished()
        {
            plotState = plotState.isFinished;
        }

        public override bool CheckLoad()
        {
            foreach (var plotType in _requiredPrePlots)
            {
                if (plotType.IsSubclassOf(typeof(TextPlot)))
                {
                    var plot = parent.GetOrCreatePlots(plotType);
                    if (plot.plotState == plotState.isPlaying || plot.plotState == plotState.isReadyToPlay)
                        return false;
                }
            }

            return true;
        }

        public override void Save()
        {
            Services.saveManager.currentStory = Services.textManager.currentStory;
        }
    }

    
    public class Day1_Text1 : TextPlot
    {
        public Day1_Text1()
        {
            //this part is for initialize the original properties that related to the plot
            _referPlot = typeof(RootPlot);
            relaSpan = TimeSpan.FromMinutes(0.1f);
            _requiredPrePlots = new List<Type>(){typeof(RootPlot)};
            var fileAddress = new PlotFileAddress().fileAddress;
            fileAddress.TryGetValue(typeof(Day1_Text1), out textAssetLocation);
            ta = Resources.Load<TextAsset>(textAssetLocation);
            _childPlots = new List<Type>{typeof(Day1_Text2)};
        }

        public override bool CheckLoad()
        {
            return true;
        }
        
    }
    
    public class Day1_Text2 : TextPlot
    {
        public Day1_Text2()
        {
            //this part is for initialize the original properties that related to the plot
            _referPlot = typeof(Day1_Text1);
            relaSpan = TimeSpan.FromMinutes(0f);
            _requiredPrePlots = new List<Type>(){typeof(Day1_Text1)};
            var fileAddress = new PlotFileAddress().fileAddress;
            fileAddress.TryGetValue(typeof(Day1_Text1), out textAssetLocation);
            ta = Resources.Load<TextAsset>(textAssetLocation);
            _childPlots = new List<Type>{typeof(Day1_Text3)};
        }

        public override bool CheckLoad()
        {
            return true;
        }
        
    }
    
    public class Day1_Text3 : TextPlot
    {
        public Day1_Text3()
        {
            //this part is for initialize the original properties that related to the plot
            _referPlot = typeof(Day1_Text2);
            relaSpan = TimeSpan.FromMinutes(0f);
            _requiredPrePlots = new List<Type>(){typeof(Day1_Text2)};
            var fileAddress = new PlotFileAddress().fileAddress;
            fileAddress.TryGetValue(typeof(Day1_Text1), out textAssetLocation);
            ta = Resources.Load<TextAsset>(textAssetLocation);
            
        }

        public override bool CheckLoad()
        {
            return true;
        }
        
    }
}
