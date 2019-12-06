using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DialADemon.Page;
using Ink.Runtime;
using UnityEngine;
//using UnityEngine.iOS;

public class PlotManager 
{
    public Dictionary<Plot, DateTime> Calendar = new Dictionary<Plot, DateTime>();
    public bool isSpeedMode = false;
    
    private DateTime _baseTime = DateTime.MinValue;
    //Calendar system
    private Dictionary<Type, Plot> _plots = new Dictionary<Type, Plot>();
    //
    private Dictionary<Type, Plot> _globalCheckPlot = new Dictionary<Type, Plot>();
    private string _currentTextAssetLocation;
    
    //other quick reference
    
    
    public void Init()
    {

        //clear notification
        //UnityEngine.iOS.NotificationServices.ClearLocalNotifications();

        //for the first time upload
        if (!File.Exists(Services.saveManager.saveJsonPath))
        {
            _baseTime = DateTime.Now;
            InitLeadPlot();
        }

        //TODO: The init here didn't consider the situation that the game is recovering from a save
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
                plotPair.Key.ChangePlotState(plotState.isReadyToPlay);
                plotPair.Key.Init();
                continue;
            }

            if (plotPair.Key.plotState == plotState.isReadyToPlay)
            {
                plotPair.Key.ChangePlotState(plotState.isPlaying);
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
            newPlot.ChangePlotState(plotState.isChecking);
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
            newPlot.ChangePlotState(plotState.isChecking);
            if (newPlot.isGlobalCheck) _globalCheckPlot.Add(newPlot.GetType(), newPlot);
            _plots.Add(type, newPlot);
            
            return newPlot;
        }
    }

    public void StartPlot<T>() where T : Plot
    {
        ClearCalendar();
        var newPlot = GetOrCreatePlots<T>();
        newPlot.AddToCalendarWithSetTime(DateTime.Now);
    }

    public void AddPlot<T>(DateTime startTime) where T : Plot
    {
        var newPlot = GetOrCreatePlots<T>();
        newPlot.AddToCalendarWithSetTime(startTime);
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
            
            if(plotPair.Key.IsSubclassOf(typeof(PhonePlot)))
            {
                PhonePlot plot = plotPair.Value as PhonePlot;
                info.phoneCallState = plot.phoneCallState;
            }
            else
            {
                info.phoneCallState = PhonePlot.PhoneCallState.NotStart;
            }
            
            plotInfos.Add(info);
        }
        Services.saveManager.plotInfo = plotInfos;
        Services.saveManager.plotBaseTime = _baseTime;
    }

    public void Load()
    {
        if (File.Exists(Services.saveManager.saveJsonPath))
        {
            var plotInfo = Services.saveManager.plotInfo;
            foreach (var info in plotInfo)
            {
                var plot = GetOrCreatePlots(info.plotType);
                plot.ChangePlotState(info.plotState);
                plot.relaSpan = info.relaSpan;
                if (info.plotType.IsSubclassOf(typeof(PhonePlot)))
                {
                    PhonePlot phonePlot = plot as PhonePlot;
                    phonePlot.phoneCallState = info.phoneCallState;
                }
                if (info.isOnCalendar) Calendar.Add(plot, info.startTime);
                plot.Reload();
            }
            _baseTime = Services.saveManager.plotBaseTime;
        }
    }

    public void CheckCapableTextWhenLoad()
    {
        
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
        
        protected plotState _plotState = plotState.isStandingBy;
        public plotState plotState
        {
            get { return _plotState; }
        }

        public void ChangePlotState(plotState ps)
        {
            _plotState = ps;
        }

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
            get { return parent._baseTime.Add(absSpan); }
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

        public virtual bool isBreak()
        {
            return false;
        }

        public virtual bool isBreak(DateTime lastMessage)
        {
            return false;
        }
        public virtual void Break(){}
        public virtual void Save(){}
        

        public void AddToCalendar()
        {
            if (_plotState == plotState.isChecking || _plotState == plotState.isStandingBy)
            {
                parent.Calendar.Add(this,plotStartTime);
                this._plotState = plotState.isOnCalendar;
            }
        }
        
        public void AddToCalendarWithSetTime(DateTime startTime)
        {
            if (_plotState == plotState.isChecking || _plotState == plotState.isStandingBy)
            {
                parent.Calendar.Add(this,startTime);
                this._plotState = plotState.isOnCalendar;
            }
        }

        public void CheckChild()
        {
            foreach (var childPlot in _childPlots)
            {
                var plotInit = parent.GetOrCreatePlots(childPlot);
                if (plotInit._plotState == plotState.isChecking || plotInit._plotState == plotState.isStandingBy)
                {
                    plotInit._plotState = plotState.isChecking;
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
                if (plot._plotState != plotState.isFinished)
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
                _childPlots = new List<Type>{typeof(Day1_Text1),typeof(Day1_Phone1)};
            }
    
            public override void Start()
            {
                _plotState = plotState.isFinished;
            }
    
            public override void Clear()
            {
                CheckChild();
            }
        }
        
    public class TextPlot : Plot
        {
            public Story story;
            protected TextManager tm = Services.textManager;
            
            public override void Reload()
            {
                if (_plotState == plotState.isPlaying || _plotState == plotState.isReadyToPlay)
                {
                    tm.currentTextPlot = this;
                    if(tm.inkJson!= "") tm.currentStory.state.LoadJson(tm.inkJson);
                    Services.eventManager.AddHandler<TextFinished>(delegate { OnTextFinished(); });
                }
            }
    
            public override void Init()
            {
                // make sure no two text plots run in the same time
                if (tm.currentTextPlot != null && this != tm.currentTextPlot)
                {
                    tm.currentTextPlot.ChangePlotState(plotState.isBreak);
                    tm.MuteAllKeyboard();
                }
            }
    
            public override void Start()
            {
                tm.currentTextPlot = this;
                Services.eventManager.AddHandler<TextFinished>(delegate{OnTextFinished();});
                
                DateTime startTime;
                parent.Calendar.TryGetValue(this, out startTime);
                tm.LoadDialogueForNewPlotWhenAPPisOff(startTime);
                
                //tm.StartNewStory(story);
            }
            
            public override void Clear()
            {
                CheckChild();
                tm.currentTextPlot = null;
                Services.eventManager.RemoveHandler<TextFinished>(delegate{OnTextFinished();});
            }
            
            public void OnTextFinished()
            {
                _plotState = plotState.isFinished;
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
                if(tm.currentTextPlot == this) Services.saveManager.currentStory = story;
                if (_plotState == plotState.isOnCalendar)
                {
                    
                }
            }
            
            public override bool isBreak()
            {
                if (!tm.isLoadInitDialogueFinished)
                {
                    var msg = Services.saveManager.FindTheLastMessage();
                    if ((DateTime.Now - msg.shootTime) > TimeSpan.FromHours(0.2f))
                    {
                        return true;
                    }
                }
                return false;
            }
    
            public override void Break()
            {
                if(tm.currentTextPlot == this) tm.MuteAllKeyboard();
                    CheckChild();
                Services.eventManager.RemoveHandler<TextFinished>(delegate{OnTextFinished();});
            }
    
            protected void InitStory()
            {
                story = PlotFileAddress.GetStory(GetType());
            }
            
            protected void InitStory(Type storyDictType)
            {
                story = PlotFileAddress.GetStory(storyDictType);
            }
        }
    
    public class Day1_Text1 : TextPlot //contract text
    {
            public Day1_Text1()
            {
                //this part is for initialize the original properties that related to the plot
                _referPlot = typeof(RootPlot);
                relaSpan = TimeSpan.FromMinutes(0.1f);
                _requiredPrePlots = new List<Type>(){typeof(RootPlot)};
                _childPlots = new List<Type>{typeof(Day2_Text1)};
                InitStory();
            }
    
            public override bool CheckLoad()
            {
                return true;
            }
        }
        
    public class Day2_Text1 : TextPlot //day 2 good morning
    {
        public Day2_Text1()
        {
            //this part is for initialize the original properties that related to the plot
            _referPlot = typeof(RootPlot);
            relaSpan = TimeSpan.FromMinutes(0.5f);
            _requiredPrePlots = new List<Type>(){typeof(Day1_Text1)};
            _childPlots = new List<Type>{typeof(Day2_Text2)};
            InitStory();
        }

        public override bool CheckLoad()
        {
            return true;
        }
        
    }

    public class Day2_Text2 : TextPlot //how are you
    {
        public Day2_Text2()
        {
            //initialize properties
            _referPlot = typeof(RootPlot);
            relaSpan = TimeSpan.FromMinutes(0.5f);
            _requiredPrePlots = new List<Type>() {typeof(Day2_Text1)};
            _childPlots = new List<Type>{typeof(Day3_Text3)};
            InitStory();

        }
    }

    public class Day3_Text3 : TextPlot //Never have I ever
    {
        public Day3_Text3()
        {
            //initialize properties
            _referPlot = typeof(RootPlot);
            relaSpan = TimeSpan.FromMinutes(0.5f);
            _requiredPrePlots = new List<Type>() {typeof(Day2_Text2)};
            _childPlots = new List<Type>{typeof(Day4_Text1)};
            InitStory();

        }
    }
    
    public class Day4_Text1 : TextPlot //Day 4 Morning
    {
        public Day4_Text1()
        {
            //initialize properties
            _referPlot = typeof(RootPlot);
            relaSpan = TimeSpan.FromMinutes(0.5f);
            _requiredPrePlots = new List<Type>() {typeof(Day3_Text3)};
            _childPlots = new List<Type>{typeof(Day6_Text2)};
            InitStory();

        }
    }
    
    public class Day6_Text2 : TextPlot //Day 6 Gifting
    {
        public Day6_Text2()
        {
            //initialize properties
            _referPlot = typeof(RootPlot);
            relaSpan = TimeSpan.FromMinutes(0.5f);
            _requiredPrePlots = new List<Type>() {typeof(Day4_Text1)};
            //_childPlots = new List<Type>{typeof(Day4_Text1)};
            InitStory();

        }
    }

    public class PhonePlot : Plot
    {
        public enum PhoneCallState
        {
            NotStart,
            Calling,
            Hanged,
            End
        }
        protected PhoneManager pm = Services.phoneManager;
        public AudioClip callContent;
        public PhoneCallState phoneCallState = PhoneCallState.NotStart;

        public TimeSpan callTimeSpan
        {
            get{return  TimeSpan.FromSeconds(callContent.length);}
        }

        public bool isDemonCall
        {
            get
            {
                if (this.GetType().IsSubclassOf(typeof(DemonPhoneCallPlot))) return true;
                if (this.GetType().IsSubclassOf(typeof(PlayerPhoneCallPlot))) return false;
                return false;
            }
        }

        public override void Init()
        {
            Services.eventManager.AddHandler<PhoneFinished>(delegate { OnPhoneFinished(); });
            Services.eventManager.AddHandler<PhoneHangedUp>(delegate { OnPhoneHangedUp(); });
        }
        
        public override void Start()
        {
            pm.currrentPhonePlot = this;
            phoneCallState = PhoneCallState.Calling;
            Services.eventManager.Fire(new PhoneStart());
        }

        public override void Clear()
        {
            phoneCallState = PhoneCallState.End;
            pm.currrentPhonePlot = null;
            CheckChild();
            Services.eventManager.RemoveHandler<PhoneFinished>(delegate{ OnPhoneFinished();});
            Services.eventManager.RemoveHandler<PhoneHangedUp>(delegate{ OnPhoneHangedUp();});
        }

        public override bool CheckLoad()
        {
            foreach (var plotType in _requiredPrePlots)
            {
                if (plotType.IsSubclassOf(typeof(PhonePlot)))
                {
                    var plot = parent.GetOrCreatePlots(plotType);
                    if (plot.plotState == plotState.isPlaying || plot.plotState == plotState.isReadyToPlay)
                        return false;
                }
            }

            return true;
        }

        public override void Break()
        {
            phoneCallState = PhoneCallState.Hanged;
            pm.currrentPhonePlot = null;
            CheckChild();
        }

        protected void OnPhoneHangedUp()
        {
            if (pm.currrentPhonePlot == this)
            {
                _plotState = plotState.isBreak;
            }
            
        }

        protected void OnPhoneFinished()
        {
            if (pm.currrentPhonePlot == this)
            {
                _plotState = plotState.isFinished;
            }
        }

        protected void InitPhoneClip(Type plotType = null)
        {
            if(plotType == null) callContent = PhoneFileAddress.GetPhoneClip(GetType());
            else callContent = PhoneFileAddress.GetPhoneClip(plotType);
        }
    }

    public class DemonPhoneCallPlot : PhonePlot
    {
    }
    
    public class PlayerPhoneCallPlot : PhonePlot
    {
        public TimeSpan playerWaitTime = TimeSpan.FromSeconds(5f);
    }

    public class Day1_Phone1 : DemonPhoneCallPlot
    {
        public Day1_Phone1()
        {
            //this part is for initialize the original properties that related to the plot
            _referPlot = typeof(RootPlot);
            relaSpan = TimeSpan.FromSeconds(0.1f);
            _requiredPrePlots = new List<Type>(){typeof(RootPlot)};
            _childPlots = new List<Type>();
            InitPhoneClip();
        }
    }
    public class VoiceMailPlot : Plot
    {
        public bool isSend;

        public override void Reload()
        {
            if (_plotState == plotState.isFinished && isSend)
            {
                //TODO upload the voice mail
            }
        }

       

        public override void Start()
        {
            isSend = true;
            //TODO upload the voice mail
            _plotState = plotState.isFinished;
        }

        public override bool CheckLoad()
        {
            foreach (var plotType in _requiredPrePlots)
            {
                var plot = parent.GetOrCreatePlots(plotType);
                if (plot.plotState == plotState.isFinished) return true;
            }

            return false;
        }
    }
    
}
