
using System;
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
        //constantly update the plots' state
        foreach (var plot in Calendar.Keys)
        {
            switch (plot.plotState)
            {
                case PlotState.StopTracked:
                    break;
                case PlotState.ReadyToPlay:
                    if (Calendar[plot].startTime < DateTime.Now)
                    {
                        plot.TuningStartTime();
                        if (Calendar[plot].startTime < DateTime.Now){
                            plot.Start();
                            plot.ChangePlotState(PlotState.Playing);
                        }
                    }
                    break;
                case PlotState.Playing:
                    plot.Update();
                    break;
                case PlotState.Finished: 
                    plot.Clear(); 
                    plot.ChangePlotState(PlotState.StopTracked);
                    break;
                case PlotState.Broke:
                    plot.Break();
                    plot.ChangePlotState(PlotState.StopTracked);
                    break;
            }
        }

        foreach (var plot in _plots.Values)
        {
            if (plot.plotState == PlotState.StandingBy)
                plot.AddToCalendar();
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

    public void ClearCalendar()
    {
        //including save and clean data
        //save TODO
        //clean
        Calendar.Clear();
        
    }

    void InitLeadPlot()
    {
        GetPlot("root")?.AddToCalendar();
    }

    public Plot GetPlot(string name)
    {
        if (_plots.ContainsKey(name)) return _plots[name];
        return null;
    }
    public void StartPlot(string name)
    {
        ClearCalendar();
        AddPlotToCalender(name,DateTime.Now);
    }

    public void AddPlotToCalender(string name, DateTime startTime)
    {
        GetPlot(name)?.AddToCalendarWithSetTime(startTime);
    }

    #region Save

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
            _LoadPlotInfo();
            
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
                plot.Reload();
                
            }
        }

    #endregion


    public void CheckCapableTextWhenLoad()
    {
        
    }
    //when the app quit, save all the info that needed
    //add notification and other stuff to keep track the gameflow
    

    public enum PlotState
    {
        StandingBy,
        ReadyToPlay,
        Playing,
        Finished,
        Broke,
        StopTracked,
    }

    public enum PlotInitType
    {
        TimeBased,
        PlotBased,
        Mix,
    }

    public abstract class Plot
    {
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
        public DateTime timeBasedSendTime { get; private set; }
        public TimeSpan plotBasedDelayTime { get; private set; }
        public TimeSpan waitTimeBeforeBreak { get; private set; }
        public List<string> prePlots = new List<string>();
        public string anchorPrePlot;

        public virtual void Reload()
        {
            if (parent.Calendar.ContainsKey(parent.GetPlot(name)))
            {
                var timeSpan = parent.Calendar[parent.GetPlot(name)];
                if (timeSpan.potentialBreakTime < DateTime.Now && plotState == PlotState.Playing)
                    ChangePlotState(PlotState.Broke);
            }
        }
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

                    span.startTime = parent.Calendar[parent._plots[anchorPrePlot]].potentialBreakTime +
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

        public void AddToCalendar()
        {
            if (_plotState == PlotState.StandingBy)
            {
                if (_IsPlotTimeSpanAbleToSet(out CalendarPlotTimeSpan span))
                {
                    parent.Calendar.Add(this,span);
                    Init();
                    ChangePlotState(PlotState.ReadyToPlay);
                }
            }
        }

        //this is for handling conflict when a plot is ready to start
        public virtual void TuningStartTime(){}
        
        public void AddToCalendarWithSetTime(DateTime startTime)
        {
            if (_plotState == PlotState.StandingBy)
            {
                parent.Calendar.Add(this,startTime);
                this._plotState = PlotState.ReadyToPlay;
            }
        }
        
    }
    
/*    public class RootPlot : Plot
        {
            public RootPlot()
            {
                _referPlot = null;
                relaSpan = TimeSpan.Zero;
                _childPlots = new List<Type>{typeof(Day1_Text1),typeof(Day1_Phone1)};
            }
    
            public override void Start()
            {
                _plotState = PlotState.Finished;
            }
    
            public override void Clear()
            {
                CheckChild();
            }
        }*/
        
    public class TextPlot : Plot
        {
            public Story story;
            protected TextManager tm = Services.textManager;
            protected List<Type> _attachedFileType = new List<Type>();
            
            public override void Reload()
            {
                if (_plotState == PlotState.Playing)
                {
                    tm.currentTextPlot = this;
                    if(Services.saveManager.inkJson!= "") tm.currentStory.state.LoadJson(Services.saveManager.inkJson);
                    Services.eventManager.AddHandler<TextFinished>(_OnTextFinished);
                }
            }
    
            public override void Start()
            {
                //the normal start setting
                tm.currentTextPlot = this;
                Services.eventManager.AddHandler<TextFinished>(_OnTextFinished);
                
                //for late start
                //for plots not load weird, the first two sentence of a plot should not have fancy "thinking time"
                //that takes more than 5- seconds
                DateTime startTime;
                parent.Calendar.TryGetValue(this, out startTime);
                if ((DateTime.Now - startTime) > TimeSpan.FromSeconds(5))
                {
                    Debug.Log("????");
                    tm._LoadDialogueForNewPlotWhenAPPisOff(startTime);
                }
                
                tm.StartNewStory(story);
            }
            
            public override void Clear()
            {
                CheckChild();
                tm.currentTextPlot = null;
                Services.eventManager.RemoveHandler<TextFinished>(_OnTextFinished);
            }
            
            private void _OnTextFinished(TextFinished e)
            {
                CheckAttachedFile(e.ShootTime);
                _plotState = PlotState.Finished;
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
                if(tm.currentTextPlot == this) tm.MuteAllKeyboard();
                    CheckChild();
                Services.eventManager.RemoveHandler<TextFinished>(_OnTextFinished);
            }
    
            protected void InitStory()
            {
                story = PlotFileAddress.GetStory(GetType());
            }
            
            protected void InitStory(Type storyDictType)
            {
                story = PlotFileAddress.GetStory(storyDictType);
            }

            
            //TODO this has to be changed to a more proper way so we dont need the file to be only able to attached in the end of a conversation
            public void CheckAttachedFile(DateTime shootTime)
            {
                foreach (var filePlotType in _attachedFileType)
                {
                    Debug.Log(filePlotType);
                    var plot = parent.GetOrCreatePlots(filePlotType);
                    if (plot.GetType().IsSubclassOf(typeof(TextFilePlot)))
                    {
                        var plotText = plot as TextFilePlot;
                        tm.AddNewFileMessage(plotText.GetType(), shootTime);
                    }
                    
                }
            }

            public override void TuningStartTime()
            {
                foreach (var playingPlot in parent.playingPlot)
                {
                    if (playingPlot.GetType().IsSubclassOf(typeof(TextPlot)))
                    {
                        // make sure no two text plots run in the same time
                        if (priority > playingPlot.priority)
                        {
                            playingPlot.ChangePlotState(PlotState.Broke);
                            tm.MuteAllKeyboard();
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
                _attachedFileType.Add(typeof(Day1_ContractFile));
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
            _childPlots = new List<Type>{typeof(Day3_Text1)};
            InitStory();

        }
    }

    public class Day3_Text1 : TextPlot
    {
        public Day3_Text1()
        {
            //initialize properties
            _referPlot = typeof(RootPlot);
            relaSpan = TimeSpan.FromMinutes(0.5f);
            _requiredPrePlots = new List<Type>() {typeof(Day2_Text2)};
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
            _requiredPrePlots = new List<Type>() {typeof(Day3_Text1)};
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
                _plotState = PlotState.Broke;
            }
            
        }

        protected void OnPhoneFinished()
        {
            if (pm.currrentPhonePlot == this)
            {
                _plotState = PlotState.Finished;
            }
        }

        protected void InitPhoneClip(Type plotType = null)
        {
            if(plotType == null) callContent = PhoneFileAddress.GetPhoneClipName(GetType());
            else callContent = PhoneFileAddress.GetPhoneClipName(plotType);
        }
        
        public override void TuningStartTime()
        {
            foreach (var playingPlot in parent.playingPlot)
            {
                if (playingPlot.GetType().IsSubclassOf(typeof(TextPlot)))
                {
                    // make sure no two text plots run in the same time
                    if (priority > playingPlot.priority)
                    {
                        playingPlot.ChangePlotState(PlotState.Broke);
                        tm.MuteAllKeyboard();
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
            if (_plotState == PlotState.Finished && isSend)
            {
                //TODO upload the voice mail
            }
        }

       

        public override void Start()
        {
            isSend = true;
            //TODO upload the voice mail
            _plotState = PlotState.Finished;
        }

        public override bool CheckLoad()
        {
            foreach (var plotType in _requiredPrePlots)
            {
                var plot = parent.GetOrCreatePlots(plotType);
                if (plot.plotState == PlotState.Finished) return true;
            }

            return false;
        }
    }

    /// <summary>
    /// Special Class
    /// This class will not be hanged in the calender
    /// </summary>
    public class TextFilePlot : Plot
    {
        protected Type importDocumentPairBelongedPlot = null;
        protected GameObject _bubblePre;
        public GameObject bubblePre
        {
            get
            {
                if (importDocumentPairBelongedPlot == null) importDocumentPairBelongedPlot = GetType();
                if (!_bubblePre)
                {
                    _bubblePre = PlotFileAddress.GetBubblePrefab(importDocumentPairBelongedPlot);
                }
                return _bubblePre;
            }
        }

        protected GameObject _documentPre;
        public GameObject documentPre
        {
            get
            {
                if (importDocumentPairBelongedPlot == null) importDocumentPairBelongedPlot = GetType();
                if (!_documentPre)
                {
                    _documentPre = PlotFileAddress.GetDocumentPrefab(importDocumentPairBelongedPlot);
                }
                return _documentPre;
            }
        }

        public GameObject bubble;
        public GameObject document;
        protected TextManager tm = Services.textManager;
        
        public void CreateBubble()
        {
            bubble = GameObject.Instantiate(bubblePre, tm.content.transform);
            bubble.GetComponentInChildren<OpenFileButton>().plotType = GetType();
            Debug.Log("1");
        }

        public void CreateDocument()
        {
            foreach (var item in Services.pageState.GetGameState("Front_Main").relatedObj)
            {
                if (item.CompareTag("PageObj"))
                {
                    Debug.Log("documentpre" + documentPre);
                    document = GameObject.Instantiate(documentPre, item.transform);
                    document.GetComponentInChildren<CloseFileButton>().plotType = GetType();
                    document.transform.localScale = new Vector3(0.1f,0.1f,0.1f);
                    document.transform.DOScale(1f, 1f);
                    break;
                }
            }
        }
    }

    public class Day1_ContractFile : TextFilePlot
    {

    }

    #region Plot Input

    private void _LoadPlotInfo(TextAsset plot)
    {
        
    }

    #endregion
}
