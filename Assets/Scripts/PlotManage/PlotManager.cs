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
    
    //other quick reference
    
    
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
                plot.ChangePlotState(info.plotState);
                plot.relaSpan = info.relaSpan;
                if (info.isOnCalendar) Calendar.Add(plot, info.startTime);
                plot.Reload();
            }
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
            _childPlots = new List<Type>{typeof(Day1_Text1)};
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
                tm.currentStory.state.LoadJson(tm.inkJson);
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
            tm.StartNewStory(story);
            Services.eventManager.AddHandler<TextFinished>(delegate{OnTextFinished();});
            
            DateTime startTime;
            parent.Calendar.TryGetValue(this, out startTime);
            tm.LoadDialogueForNewPlotWhenAPPisOff(startTime);
        }
        
        public override void Clear()
        {
            CheckChild();
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
            Services.saveManager.currentStory = tm.currentStory;
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
            string textAssetLocation;
            var fileAddress = new PlotFileAddress().fileAddress;
            fileAddress.TryGetValue(this.GetType(), out textAssetLocation);
            var ta = SerializeManager.ReadJsonString(Application.dataPath + "/Resources/InkText/" + textAssetLocation);
            Debug.Log(ta);
            //var ta = Resources.Load<TextAsset>(textAssetLocation);
            story = new Story(ta);
        }
        
        protected void InitStory(Type storyDictType)
        {
            string textAssetLocation;
            var fileAddress = new PlotFileAddress().fileAddress;
            fileAddress.TryGetValue(storyDictType, out textAssetLocation);
            var ta = SerializeManager.ReadJsonString(Application.dataPath + "/Resources/InkText/" + textAssetLocation);
            //var ta = Resources.Load<TextAsset>(textAssetLocation);
            story = new Story(ta);
            Debug.Log(ta);
        }
    }
    
    public class Day1_Text1 : TextPlot
    {
        public Day1_Text1()
        {
            //this part is for initialize the original properties that related to the plot
            _referPlot = typeof(RootPlot);
            relaSpan = TimeSpan.FromMinutes(0.5f);
            _requiredPrePlots = new List<Type>(){typeof(RootPlot)};
            _childPlots = new List<Type>{typeof(Day1_Text2)};
            InitStory();
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
            _referPlot = typeof(RootPlot);
            relaSpan = TimeSpan.FromMinutes(0.5f);
            _requiredPrePlots = new List<Type>(){typeof(Day1_Text1)};
            _childPlots = new List<Type>{typeof(Day1_Text3)};
            InitStory(typeof(Day1_Text1));
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
            InitStory(typeof(Day1_Text1));
        }

        public override bool CheckLoad()
        {
            return true;
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
        protected List<VoiceMailPlot> _relatedVoiceMail = new List<VoiceMailPlot>();
        public Dictionary<VoiceMailPlot, DateTime> loadedVoiceMail = new Dictionary<VoiceMailPlot, DateTime>();

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
            Services.eventManager.AddHandler<PhoneFinished>(delegate{ OnPhoneFinished();});
            Services.eventManager.AddHandler<PhoneHangedUp>(delegate{ OnPhoneHangedUp();});
        }

        public override void Reload()
        {
            if (_plotState == plotState.isPlaying || _plotState == plotState.isReadyToPlay)
            {
            }
        }
        
        public override void Clear()
        {
            CheckChild();
            
            Services.eventManager.RemoveHandler<PhoneFinished>(delegate{ OnPhoneFinished();});
            Services.eventManager.RemoveHandler<PhoneHangedUp>(delegate{ OnPhoneHangedUp();});
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
            if (plotState == plotState.isOnCalendar)
            {
                //TODO send notification
            }
        }

        public override void Break()
        {
            CheckChild();
        }

        protected void OnPhoneHangedUp()
        {
            if (pm.currrentPhonePlot == this)
            {
                _plotState = plotState.isFinished;
                phoneCallState = PhoneCallState.Hanged;
                pm.currrentPhonePlot = null;
            }
        }

        protected void OnPhoneFinished()
        {
            if (pm.currrentPhonePlot == this)
            {
                _plotState = plotState.isFinished;
                phoneCallState = PhoneCallState.End;
                pm.currrentPhonePlot = null;
            }
        }
        
        protected void LoadRelatedVoiceMail(){}
    }

    public class DemonPhoneCallPlot : PhonePlot
    {
        public override void Start()
        {
            Services.eventManager.Fire(new DemonPhoneCallIn());
            pm.currrentPhonePlot = this;
            phoneCallState = PhoneCallState.Calling;
        }
    }
    
    public class PlayerPhoneCallPlot : PhonePlot
    {
        
        public override void Start()
        {
            Services.eventManager.Fire(new PlayerPhoneCallOut());
            pm.currrentPhonePlot = this;
            phoneCallState = PhoneCallState.Calling;
        }
    }
    public class VoiceMailPlot : Plot
    {
        protected PhoneManager pm = Services.phoneManager;

        public override void Reload()
        {
            if (_plotState == plotState.isPlaying || _plotState == plotState.isReadyToPlay)
            {
            }
        }
   
        public override void Init()
        {
    
        }

        public override void Start()
        {
        }
        
        public override void Clear()
        {
            CheckChild();
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
            if (plotState == plotState.isOnCalendar)
            {
                //TODO send notification
            }
        }
        
        public override bool isBreak()
        {
            return false;
        }

        public override void Break()
        {
        }
    }
}
