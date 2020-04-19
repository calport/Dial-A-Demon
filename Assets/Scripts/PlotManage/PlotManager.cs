
using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using DG.Tweening;
using DialADemon.Page;
using Ink.Runtime;
using SimpleJSON;
using UnityEngine;
using UnityEngine.Experimental.XR;

//using UnityEngine.iOS;

public class PlotManager 
{
    public struct CalendarPlotTimeSpan
    {
        public DateTime startTime;
        public DateTime potentialBreakTime;
    }
    //Calendar contains all the information about the plots which is ready, preparing and finished with certain trigger time
    public Dictionary<Plot, CalendarPlotTimeSpan> Calendar = new Dictionary<Plot, CalendarPlotTimeSpan>();
    public List<Plot> playingPlot = new List<Plot>();
    public bool isSpeedMode = false;
    
    private DateTime _baseTime = DateTime.MinValue;
    //Calendar system
    private Dictionary<string, Plot> _plots = new Dictionary<string, Plot>();
    private string _currentTextAssetLocation;
    
    //other quick reference

    #region Life Cycle

    public void Init()
        {
    
            //TODO: clear notification
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
        //constantly update the plots' state
        foreach (var pair in Calendar)
        {
            switch (pair.Key.plotState)
            {
                case PlotState.Finished:
                case PlotState.Broke:
                    break;
                case PlotState.ReadyToPlay:
                    if (pair.Value.startTime < DateTime.Now)
                    {
                        pair.Key.TuningStartTime();
                        if (pair.Value.startTime < DateTime.Now){
                            pair.Key.Start();
                            pair.Key.ChangePlotState(PlotState.Playing);
                        }
                    }
                    break;
                case PlotState.Playing:
                    pair.Key.Update();
                    _CheckAndBreakIfItsBreakTime(pair);
                    break;
                case PlotState.IsFinishing: 
                    pair.Key.Clear(); 
                    pair.Key.ChangePlotState(PlotState.Finished);
                    break;
                case PlotState.IsBreaking:
                    pair.Key.Break();
                    pair.Key.ChangePlotState(PlotState.Broke);
                    break;
            }
        }

        foreach (var plot in _plots.Values)
        {
            if (plot.plotState == PlotState.StandingBy)
                plot.TryAddToCalendar();
        }
    }

    public void Clear()
    {
        //save all the list of plots
        //save all the plot start time
        //TODO
        //add notification
    }

    #endregion

    #region Public Functions

         public void ClearCalendar()
        {
            //including save and clean data
            //save TODO
            //clean
            Calendar.Clear();
            
        }
    
        void InitLeadPlot()
        {
            GetPlot("root")?.TryAddToCalendar();
        }
    
        public Plot GetPlot(string name)
        {
            if (_plots.ContainsKey(name)) return _plots[name];
            return null;
        }

    #endregion
    
    #region GameTestFunc

    public void Tool_StartPlot(string name)
    {
        ClearCalendar();
        Tool_AddPlotToCalender(name,
            new CalendarPlotTimeSpan {startTime = DateTime.Now, potentialBreakTime = DateTime.MaxValue});
    }

    public void Tool_AddPlotToCalender(string name, CalendarPlotTimeSpan timeSpan)
    {
        GetPlot(name)?.Tool_AddToCalendar(timeSpan);
    }

    #endregion

    #region Save & Load

    /// <summary>
    /// all these happen without plotmanager update
    /// </summary>
    /// <param name="jsonObject"></param>
    /// <returns></returns>
        public JSONObject Save(JSONObject jsonObject)
        {
            var plotJsonObj = new JSONObject();
            var plotList = new JSONArray();
            foreach (var plotPair in Calendar)
            {
                //save each plot
                var plotJson = new JSONObject();
                plotJson.Add("name",plotPair.Key.name);
                var stringStartTime = (SerializeManager.JsonDateTime) plotPair.Value.startTime;
                var stringBreakTime = (SerializeManager.JsonDateTime) plotPair.Value.potentialBreakTime;
                plotJson.Add("startTime",stringStartTime.value.ToString());
                plotJson.Add("breakTime",stringBreakTime.value.ToString());
                plotJson.Add("plotState",plotPair.Key.plotState.ToString());
                
                if(plotPair.Key.GetType().IsSubclassOf(typeof(PhonePlot)))
                {
                    PhonePlot plot = plotPair.Key as PhonePlot;
                    plotJson.Add("phoneCallState", plot.phoneCallState.ToString());
                }
                else
                    plotJson.Add("phoneCallState", PhonePlot.PhoneCallState.NotStart.ToString());
                plotList.Add(plotJson);
            }
            
            plotJsonObj.Add("plotList",plotList);
            var stringBaseTime = (SerializeManager.JsonDateTime) _baseTime;
            plotJsonObj.Add("plotBaseTime",stringBaseTime.value.ToString());
            
            jsonObject.Add("plot",plotJsonObj);
            return jsonObject;
        }

        public void Load(JSONNode jsonObject)
        {
            //Load all plots' info
            _ParsePlotInfo();
            
            //reload the calendar
            var plotJsonObj = jsonObject["plot"];
            _baseTime = new SerializeManager.JsonDateTime(Convert.ToInt64((string)plotJsonObj["plotBaseTime"]));
            foreach (var jsonPlotInfo in plotJsonObj["plotList"].Values)
            {
                var plot = GetPlot(jsonPlotInfo["name"]);
                if(Enum.TryParse<PlotState>(jsonPlotInfo["plotState"], out PlotState s))
                    plot.ChangePlotState(s);
                var timeSpan = new CalendarPlotTimeSpan
                {
                    startTime = new SerializeManager.JsonDateTime(Convert.ToInt64((string)jsonPlotInfo["startTime"])),
                    potentialBreakTime = new SerializeManager.JsonDateTime(Convert.ToInt64((string)jsonPlotInfo["breakTime"]))
                };
                Calendar.Add(plot, timeSpan);
                if (plot.GetType().IsSubclassOf(typeof(PhonePlot)))
                {
                    PhonePlot phonePlot = plot as PhonePlot;
                    Enum.TryParse<PhonePlot.PhoneCallState>(jsonPlotInfo["phoneCallState"], out phonePlot.phoneCallState);
                }
                
                //game load functions
                _LoadPlayingPlots();
                _LoadPlotsThatShouldStart();
            }
        }


        private void _LoadPlayingPlots()
        {
            foreach (var plotPair in Calendar)
            {
                if(plotPair.Key.plotState != PlotState.Playing) return;
                
                _CheckAndBreakIfItsBreakTime(plotPair);
                plotPair.Key.ReloadPlayingPlot();
            }
        }
        private void _LoadPlotsThatShouldStart()
        {
            var sortCalendar = _SortCalenderByStartTime();
            var pendingPlots = sortCalendar.Where(pair =>
                pair.Value.startTime < DateTime.Now - TimeSpan.FromMinutes(3) &&
                pair.Key.plotState == PlotState.ReadyToPlay);
            
            //leave the newest text plot and either abandon or break the old ones
            var textPairs = pendingPlots.Where(pair => pair.Key is TextPlot).ToList();
            var startText = textPairs[textPairs.Count() - 1];
            startText.Key.LateStart();
            startText.Key.ChangePlotState(PlotState.Playing);
            _CheckAndBreakIfItsBreakTime(startText);
            textPairs.Remove(startText);
            foreach (var pair in textPairs)
            {
                pair.Key.OnAbandon();
                pair.Key.ChangePlotState(PlotState.Abandoned);
            } 
            
            //all the phones are set to missed
            //which is break
            var phonePairs = pendingPlots.Where(pair => pair.Key is PhonePlot).ToList();
            foreach (var pair in phonePairs)
            {
                pair.Key.Break();
                pair.Key.ChangePlotState(PlotState.Broke);
            }
        }
    

        #endregion

        #region Private Function

        private void _CheckAndBreakIfItsBreakTime(KeyValuePair<Plot,CalendarPlotTimeSpan> calendarPlot)
        {
            if (calendarPlot.Value.potentialBreakTime < DateTime.Now && calendarPlot.Key.plotState == PlotState.Playing)
                calendarPlot.Key.ChangePlotState(PlotState.IsBreaking);
        }

        private List<KeyValuePair<Plot,CalendarPlotTimeSpan>> _SortCalenderByStartTime()
        {
            var list = Calendar.ToList();
            for(var i = 0; i < list.Count - 1; i++) {
                for(var j = 0; j < list.Count - 1 - i; j++) {
                    if(list[j].Value.startTime > list[j+1].Value.startTime) {       
                        var temp = list[j+1];
                        list[j+1] = list[j];
                        list[j] = temp;
                    }
                }
            }
            return list;
        }

        private List<KeyValuePair<Plot, CalendarPlotTimeSpan>> _GetPlotWithStateFromCalendar(PlotState state)
        {
            var list = new List<KeyValuePair<Plot, CalendarPlotTimeSpan>>();
            foreach (var pair in Calendar)
                if(pair.Key.plotState == state) list.Add(pair);
            return list;
        }
        
        #endregion
    //when the app quit, save all the info that needed
    //add notification and other stuff to keep track the gameflow
    

    public enum PlotState
    {
        StandingBy,
        ReadyToPlay,
        Playing,
        IsFinishing,
        Finished,
        IsBreaking,
        Broke,
        Abandoned
    }

    public enum PlotInitType
    {
        TimeBased,
        PlotBased,
        Mix,
    }

    public class Plot
    {
        public Plot( 
            string name, PlotInitType initType, int priority, TimeSpan timeBasedSendTimeSpan, TimeSpan plotBasedDelayTime,
            TimeSpan waitTimeBeforeBreak, List<string> prePlots)
        {
            this.name = name;
            this.initType = initType;
            this.priority = priority;
            this.timeBasedSendTimeSpan = timeBasedSendTimeSpan;
            this.plotBasedDelayTime = plotBasedDelayTime;
            this.waitTimeBeforeBreak = waitTimeBeforeBreak;
            this.prePlots = prePlots;
        }
        
        public PlotManager parent;

        //these plots are only important for a special speed-up mode
        protected List<Type> _requiredPrePlots = new List<Type>();
        
        protected PlotState _plotState = PlotState.StandingBy;

        public PlotState plotState
        {
            get { return _plotState; }
        }

        public void ChangePlotState(PlotState ps)
        {
            _plotState = ps;
            if(ps == PlotState.Playing) parent.playingPlot.Add(this);
            if (_plotState == PlotState.Playing) parent.playingPlot.Remove(this);
        }

        //new sys
        public string name { get; private set; }
        public PlotInitType initType { get; private set; }
        public int priority { get; private set; } = 0;
        public TimeSpan timeBasedSendTimeSpan { get; private set; }
        public DateTime timeBasedSendTime
        {
            get { return parent._baseTime.Date + timeBasedSendTimeSpan; }
        }

        public TimeSpan plotBasedDelayTime { get; private set; }
        public TimeSpan waitTimeBeforeBreak { get; private set; }
        public List<string> prePlots = new List<string>();
        
        //event delegate
        public Action onPlotStart;
        public Action onPlotBreak;

        private CalendarPlotTimeSpan? _currentCalendarTimeSpan
        {
            get {
                if (parent.Calendar.ContainsKey(this))
                    return parent.Calendar[this];
                return null;
            }
        }

        #region Life Cycle

            public virtual void ReloadPlayingPlot(){}

            //called when reloading
            //for plots that are supposed to happen in between the time that the player is offline
            public virtual void LateStart(){}
            
            //int when the plot is added to calendar
            public virtual void Init()
            {
            }
            //start when it is time for the plot to run
            public virtual void Start()
            {
                onPlotStart.Invoke();
            }
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
    
            public virtual void Break()
            {
                onPlotBreak.Invoke();
            }
            
            public virtual void OnAbandon(){}

        #endregion
        
        #region Public Func

        public void TryAddToCalendar()
        {
            if (_plotState == PlotState.StandingBy)
            {
                if (_IsPlotTimeSpanAbleToSet(out CalendarPlotTimeSpan span))
                {
                    parent.Calendar.Add(this, span);
                    Init();
                    ChangePlotState(PlotState.ReadyToPlay);
                }
            }
        }

        public void Tool_AddToCalendar(CalendarPlotTimeSpan span)
        {
            parent.Calendar.Add(this, span);
            Init();
            ChangePlotState(PlotState.ReadyToPlay);
        }


        //this is for handling conflict when a plot is ready to start
        public void TuningStartTime()
        {
            foreach (var playingPlot in parent.playingPlot)
            {
                if (this is TextPlot)
                {
                    if (playingPlot is TextPlot)
                    {
                        // make sure no two text plots run in the same time
                        if (priority > playingPlot.priority)
                            playingPlot.ChangePlotState(PlotState.IsBreaking);
                        continue;
                    }

                    if (playingPlot is PhonePlot)
                    {
                        var span = parent.Calendar[this];
                        span.startTime += TimeSpan.FromMinutes(3f);
                        span.potentialBreakTime += TimeSpan.FromMinutes(3f);
                        parent.Calendar[this] = span;
                        continue;
                    }
                }

                if(this is PhonePlot)
                {
                    if (playingPlot is TextPlot)
                    {
                        // make sure no two text plots run in the same time
                        if (priority > playingPlot.priority)
                        {
                            playingPlot.ChangePlotState(PlotState.IsBreaking);
                            Services.textManager.MuteAllKeyboard();
                        }
                        continue;
                    }

                    if (playingPlot is PhonePlot)
                    {
                        if (priority > playingPlot.priority)
                        { 
                            var span = parent.Calendar[this];
                            span.startTime += TimeSpan.FromMinutes(3f);
                            span.potentialBreakTime += TimeSpan.FromMinutes(3f);
                            parent.Calendar[this] = span;
                            continue;
                        }
                        else
                        {
                            ChangePlotState(PlotState.IsBreaking);
                        }
                       
                    }
                }
                if (playingPlot.GetType().IsSubclassOf(typeof(PhonePlot)))
                {
                    var timeSpan = parent.Calendar[this];
                    timeSpan.startTime += TimeSpan.FromMinutes(1);
                    timeSpan.potentialBreakTime += TimeSpan.FromMinutes(1);
                    parent.Calendar[this] = timeSpan;
                }
            }
        }
        
        #endregion
        

        private void _CheckAndBreakIfItsBreakTime()
        {
            if (_currentCalendarTimeSpan?.potentialBreakTime < DateTime.Now && plotState == PlotState.Playing)
                ChangePlotState(PlotState.IsBreaking);
        }

        private bool _IsPlotTimeSpanAbleToSet(out CalendarPlotTimeSpan span)
        {
            span = new CalendarPlotTimeSpan();
            switch (initType)
            {
                case PlotInitType.TimeBased:
                    span.startTime = timeBasedSendTime;
                    span.potentialBreakTime = timeBasedSendTime + waitTimeBeforeBreak;
                    return true;
                case PlotInitType.PlotBased:
                    foreach (var plot in prePlots)
                    {
                        if (parent._plots.ContainsKey(plot))
                        {
                            var state = parent._plots[plot].plotState;
                            if (state == PlotState.StandingBy || state == PlotState.ReadyToPlay ||
                                state == PlotState.Playing) 
                                return false;
                            continue;
                        }
                        Debug.Assert(true,"plot name " + plot + " not found");
                    }

                    span.startTime = parent.Calendar[parent._plots[prePlots[0]]].potentialBreakTime +
                                     plotBasedDelayTime;
                    span.potentialBreakTime = span.startTime + waitTimeBeforeBreak;
                    return true;
                case PlotInitType.Mix:
                    if (DateTime.Now > timeBasedSendTime) return false;
                    
                    foreach (var plot in prePlots)
                    {
                        if (parent._plots.ContainsKey(plot))
                        {
                            var state = parent._plots[plot].plotState;
                            if (state == PlotState.StandingBy || state == PlotState.ReadyToPlay ||
                                state == PlotState.Playing) 
                                return false;
                            continue;
                        }
                        Debug.Assert(true,"plot name " + plot + " not found");
                    }
                    span.startTime = timeBasedSendTime;
                    span.potentialBreakTime = timeBasedSendTime + waitTimeBeforeBreak;
                    return true;
            }

            return false;
        }
    }

    public class TextPlot : Plot
    {
        public TextPlot(string name, PlotInitType initType, int priority, TimeSpan timeBasedSendTimeSpan,
            TimeSpan plotBasedDelayTime,
            TimeSpan waitTimeBeforeBreak, List<string> prePlots) : base(name, initType, priority, timeBasedSendTimeSpan,
            plotBasedDelayTime,
            waitTimeBeforeBreak, prePlots){}

        private Story _story;

        public Story story
        {
            get
            {
                if (ReferenceEquals(_story, null))
                {
                    var textAsset = Resources.Load<TextAsset>("InkText/" + name);
                    //var ta = Resources.Load<TextAsset>(textAssetLocation);
                    _story = new Story(textAsset.text);
                    return _story;
                }

                return _story;
            }
        }
        protected TextManager tm = Services.textManager;
        protected List<Type> _attachedFileType = new List<Type>();

        public Action onPlotPause;
        public Action onPlotUnPause;

        public override void ReloadPlayingPlot()
        {
            Services.eventManager.AddHandler<TextFinished>(_OnTextFinished);
        }

        public override void LateStart()
        {
            parent.Calendar.TryGetValue(this, out CalendarPlotTimeSpan timeSpan);
            Debug.Log("????");
            tm._LoadDialogueForNewPlotWhenAPPisOff(timeSpan.startTime);
            Services.eventManager.AddHandler<TextFinished>(_OnTextFinished);
        }

        public override void Start()
        {
            base.Start();
            //the normal start setting
            Services.eventManager.AddHandler<TextFinished>(_OnTextFinished);
            tm.StartNewStory(story);
        }

        public override void Clear()
        {
            Services.eventManager.RemoveHandler<TextFinished>(_OnTextFinished);
        }

        private void _OnTextFinished(TextFinished e)
        {
            _plotState = PlotState.IsFinishing;
        }

        public override bool isBreak()
        {
            //when the loading is checking
            //if the last msg sent has been more than 0.2 hours ago, then this plot will be cancel
            if (!tm.isLoadInitDialogueFinished)
            {
                var msg = Services.textManager.FindTheLastMessage();
                if ((DateTime.Now - msg.shootTime) > TimeSpan.FromHours(0.2f))
                {
                    return true;
                }
            }

            return false;
        }

        public override void Break()
        {
            if (tm.currentTextPlot == this) tm.MuteAllKeyboard();
            Services.eventManager.RemoveHandler<TextFinished>(_OnTextFinished);
        }

        public void Pause()
        {

        }

        public void UnPause()
        {

        }
    }

    public class PhonePlot : Plot
    {
        public PhonePlot(string name, PlotInitType initType, int priority, TimeSpan timeBasedSendTimeSpan,
            TimeSpan plotBasedDelayTime,
            TimeSpan waitTimeBeforeBreak, List<string> prePlots) : base(name, initType, priority, timeBasedSendTimeSpan,
            plotBasedDelayTime,
            waitTimeBeforeBreak, prePlots){}
        public enum PhoneCallState
        {
            NotStart,
            Calling,
            Hanged,
            End
        }
        protected PhoneManager pm = Services.phoneManager;
        private AudioClip _callContent;

        public AudioClip callContent
        {
            get
            {
                if (ReferenceEquals(_callContent, null))
                {
                    _callContent = Services.audioManager.GetAudioClip(name,"PhoneCall/");
                    return _callContent;
                }
                return _callContent;
            }
        }
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
            Services.eventManager.RemoveHandler<PhoneFinished>(delegate{ OnPhoneFinished();});
            Services.eventManager.RemoveHandler<PhoneHangedUp>(delegate{ OnPhoneHangedUp();});
        }

        public override void Break()
        {
            phoneCallState = PhoneCallState.Hanged;
            pm.currrentPhonePlot = null;
        }

        protected void OnPhoneHangedUp()
        {
            if (pm.currrentPhonePlot == this)
            {
                _plotState = PlotState.IsBreaking;
            }
            
        }

        protected void OnPhoneFinished()
        {
            if (pm.currrentPhonePlot == this)
            {
                _plotState = PlotState.IsFinishing;
            }
        }
    }

    public class DemonPhoneCallPlot : PhonePlot
    {
        public DemonPhoneCallPlot(string name, PlotInitType initType, int priority, TimeSpan timeBasedSendTimeSpan,
            TimeSpan plotBasedDelayTime,
            TimeSpan waitTimeBeforeBreak, List<string> prePlots) : base(name, initType, priority, timeBasedSendTimeSpan,
            plotBasedDelayTime,
            waitTimeBeforeBreak, prePlots){}
    }
    
    public class PlayerPhoneCallPlot : PhonePlot
    {
        public PlayerPhoneCallPlot(string name, PlotInitType initType, int priority, TimeSpan timeBasedSendTimeSpan,
            TimeSpan plotBasedDelayTime,
            TimeSpan waitTimeBeforeBreak, List<string> prePlots) : base(name, initType, priority, timeBasedSendTimeSpan,
            plotBasedDelayTime,
            waitTimeBeforeBreak, prePlots){}
        public TimeSpan playerWaitTime = TimeSpan.FromSeconds(5f);
    }

    public class VoiceMailPlot : Plot
    {
        public VoiceMailPlot(string name, PlotInitType initType, int priority, TimeSpan timeBasedSendTimeSpan,
            TimeSpan plotBasedDelayTime,
            TimeSpan waitTimeBeforeBreak, List<string> prePlots) : base(name, initType, priority, timeBasedSendTimeSpan,
            plotBasedDelayTime,
            waitTimeBeforeBreak, prePlots){}
        
        public bool isSend;

        public override void ReloadPlayingPlot()
        {
            if (_plotState == PlotState.IsFinishing && isSend)
            {
                //TODO upload the voice mail
            }
        }
        
        public override void Start()
        {
            isSend = true;
            //TODO upload the voice mail
            _plotState = PlotState.IsFinishing;
        }
    }

    #region Plot Input

    private void _ParsePlotInfo()
    {
        var plotDataCSV = Resources.Load<TextAsset>("Data/PlotInfo");
        
        _plots.Clear();
        
        var lines = plotDataCSV.text.Split('\n');
        for (var i = 1; i < lines.Length; i++)
        {
            var splitLine = lines[i].Split(',');
            if (splitLine.Length != 10) continue;

            var name = splitLine[0];
            var initType = (PlotInitType)Enum.Parse(typeof(PlotInitType),splitLine[2]);
            var priority = int.Parse(splitLine[3]);
            var timeBasedSendTimeSpan = splitLine[4] == "" ? TimeSpan.Zero : _ParseTimeSpan(splitLine[4]);
            var plotBasedDelayTimeSpan = splitLine[5] == "" ? TimeSpan.Zero : _ParseTimeSpan(splitLine[5]);
            var waitTimeBeforeBreak = splitLine[6] == "" ? TimeSpan.Zero : _ParseTimeSpan(splitLine[6]);
            var anchorPlotName = splitLine[7];
            var prePlotsName = splitLine.Skip(8).ToList();
            prePlotsName.Remove(anchorPlotName);
            prePlotsName.Insert(0,anchorPlotName);
            
            object[] para = new object[]{name,initType,priority,timeBasedSendTimeSpan,plotBasedDelayTimeSpan,waitTimeBeforeBreak,prePlotsName};
            Assembly assembly = Assembly.GetExecutingAssembly();
            var toAdd = assembly.CreateInstance("PlotManager+" + splitLine[1].Trim(),true,System.Reflection.BindingFlags.Default,null,para,null,null);
            if(!ReferenceEquals(toAdd,null) && toAdd.GetType().IsSubclassOf(typeof(Plot))) _plots.Add(name,(Plot) toAdd);
        }
    }

    private DateTime _ParseDateTime(DateTime contrast, string toParse)
    {
        var toReturn = new DateTime();
        var timeSpan = _ParseTimeSpan(toParse);

        toReturn = contrast.Date + timeSpan;
        return toReturn;
    }

    private TimeSpan _ParseTimeSpan(string toParse)
    {
        var toReturn = TimeSpan.Zero;
        var infoList = toParse.Trim(new char[]{'d', 'h', 'm'}).Split(new char[]{'d', 'h', 'm'},StringSplitOptions.RemoveEmptyEntries);
        toReturn += TimeSpan.FromDays(int.Parse(infoList[0]));
        toReturn += TimeSpan.FromHours(int.Parse(infoList[1]));
        toReturn += TimeSpan.FromMinutes(int.Parse(infoList[2]));
        return toReturn;
    }

    #endregion
}
