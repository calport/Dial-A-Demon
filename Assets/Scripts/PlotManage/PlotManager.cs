using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;
//using UnityEngine.iOS;
public class PlotManagerInfo
{
    public DateTime baseTime;
}
public class PlotManager
{
    public PlotManagerInfo plotInfo = new PlotManagerInfo();
    //Calendar system
    public Dictionary<Type, Plot> plots = new Dictionary<Type, Plot>();
    public Dictionary<Type, Plot> globalCheckPlot = new Dictionary<Type, Plot>();
    public Dictionary<Plot, DateTime> Calendar = new Dictionary<Plot, DateTime>();



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
        foreach (var plotPair in Calendar)
        {
            if (plotPair.Value > DateTime.Now && plotPair.Key.plotState == plotState.isOnCalendar)
            {
                plotPair.Key.plotState = plotState.isPlaying;
                plotPair.Key.Start();
            }

            if (plotPair.Key.plotState == plotState.isPlaying)
            {
                plotPair.Key.Update();
            }

            if (plotPair.Key.plotState == plotState.isFinished)
            {
                plotPair.Key.Clear();
                unusedPlot.Add(plotPair.Key);
            }

            if (plotPair.Key.plotState == plotState.isBreak)
            {
                plotPair.Key.Break();
                unusedPlot.Add(plotPair.Key);
            }
        }
        
        //clean finished plots
        foreach (var plot in unusedPlot)
        {
            Calendar.Remove(plot);
        }
        
        foreach (var plot in globalCheckPlot.Values)
        {
            if (plot.plotState == plotState.isStandingBy || plot.plotState == plotState.isChecking)
            {
                bool isLoadable = plot.CheckLoad();
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
        plots.Clear();
        
    }
    void InitBaseTime()
    {
        if (!File.Exists(Application.dataPath + "/Resources/Json/Time.json"))
        {
            plotInfo.baseTime = DateTime.Now;
            ;
        }
        else
        {
            StreamReader sr = new StreamReader(Application.dataPath + "/Resources/Json/Time.json");
            string json = sr.ReadToEnd();
            if (json.Length > 0)
            {
                plotInfo = JsonUtility.FromJson<PlotManagerInfo>(json);
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
        plots.TryGetValue(typeof(T), out outPlot);
        if (outPlot != null)
        {
            return outPlot as T;
        }
        else
        {
            T newPlot = Activator.CreateInstance<T>();
            newPlot.parent = this;
            newPlot.Init();
            newPlot.plotState = plotState.isChecking;
            if(newPlot.isGlobalCheck) globalCheckPlot.Add(newPlot.GetType(),newPlot);
            plots.Add(typeof(T), newPlot);
            return newPlot;
        }
    }
    
    public Plot GetOrCreatePlots(Type type)
    {
        Plot outPlot;
        plots.TryGetValue(type, out outPlot);
        if (outPlot != null)
        {
            return outPlot;
        }
        else
        {
            Plot newPlot = Activator.CreateInstance(type) as Plot;
            newPlot.parent = this;
            newPlot.Init();
            newPlot.plotState = plotState.isChecking;
            if(newPlot.isGlobalCheck) globalCheckPlot.Add(newPlot.GetType(),newPlot);
            plots.Add(type, newPlot);
            return newPlot;
        }
    }

    public void StartPlot<T>() where T : Plot
    {
        ClearCalendar();
        var newPlot = GetOrCreatePlots<T>();
        plotInfo.baseTime = DateTime.Now;
        newPlot.SetRelatedSpanToZero();
        newPlot.AddToCalendar();
    }

    //when the app quit, save all the info that needed
    //add notification and other stuff to keep track the gameflow
    

    public enum plotState
    {
        isStandingBy,
        isChecking,
        isOnCalendar,
        isPlaying,
        isFinished,
        isAbandoned,
        isBreak,
    }

    public abstract class Plot
    {
        public PlotManager parent;
        protected  Dictionary<Type, Plot> parentPlots = new Dictionary<Type, Plot>();
        protected  Dictionary<Type, Plot> childPlots = new Dictionary<Type, Plot>();
        
        public plotState plotState = plotState.isStandingBy;
        
        //true when the checking 
        public  bool isGlobalCheck = false;

        public  bool isInstance = false;
        
        //calendar time
        protected Plot _referPlot;
        protected TimeSpan _relaSpan;

        public TimeSpan absSpan
        {
            get
            {
                TimeSpan time = _relaSpan;
                Plot rp = this;
                while(rp._referPlot.GetType() != typeof(RootPlot))
                {
                    rp = parent.GetOrCreatePlots(rp._referPlot.GetType());
                    time += rp._relaSpan;
                }

                return time;
            }
        }
        public DateTime plotStartTime
        {
            get { return parent.plotInfo.baseTime.Add(absSpan); }
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
        public virtual void Init()
        {
        }

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
            foreach (var childPlot in childPlots.Values)
            {
                var plotInit = parent.GetOrCreatePlots(childPlot.GetType());
                if (plotInit.plotState == plotState.isChecking || plotInit.plotState == plotState.isStandingBy)
                {
                    bool isLoadable = plotInit.CheckLoad();
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

        public void SetRelatedSpanToZero()
        {
            _relaSpan= TimeSpan.Zero;
        }
    }

    public class RootPlot : Plot
    {
        RootPlot()
        { 
            childPlots = new Dictionary<Type, Plot>{{typeof(Day1_Text1),new Day1_Text1()}};; 
            _referPlot = new RootPlot();
             _relaSpan = TimeSpan.Zero;
        }

        public override void Start()
        {
            CheckChild();
            plotState = plotState.isFinished;
        }
    }
    
    public class Day1_Text1 : Plot{}
}
