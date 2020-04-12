
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
    public Dictionary<Plot, DateTime> Calendar = new Dictionary<Plot, DateTime>();
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
            List<Plot> unusedPlot = new List<Plot>();
            //constantly update the plots' state
            for(int i = 0; i< Calendar.Keys.ToList().Count;i++)
            {
                KeyValuePair<Plot, DateTime> plotPair = Calendar.ElementAt(i);
                if (plotPair.Value < DateTime.Now && plotPair.Key.plotState == PlotState.isOnCalendar)
                {
                    plotPair.Key.ChangePlotState(PlotState.isReadyToPlay);
                    plotPair.Key.Init();
                    continue;
                }
    
                if (plotPair.Key.plotState == PlotState.isReadyToPlay)
                {
                    plotPair.Key.ChangePlotState(PlotState.isPlaying);
                    plotPair.Key.Start();
                    continue;
                }
                
                if (plotPair.Key.plotState == PlotState.isPlaying)
                {
                    plotPair.Key.Update();
                    continue;
                }
    
                if (plotPair.Key.plotState == PlotState.isFinished)
                {
                    plotPair.Key.Clear();
                    unusedPlot.Add(plotPair.Key);
                    continue;
                }
    
                if (plotPair.Key.plotState == PlotState.isBreak)
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
                if (plot.plotState == PlotState.isStandingBy || plot.plotState == PlotState.isChecking)
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
        var root = GetOrCreatePlots<RootPlot>();
        root.AddToCalendar();
    }

    public Plot GetPlot(string name)
    {
        if (_plots.ContainsKey(name)) return _plots[name];
        return null;
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

    #region Save

        [Serializable]
        public class PlotInfo
        {
            public bool isOnCalendar;
            public Type plotType;
            public PlotManager.PlotState plotState;
            public TimeSpan relaSpan;
            public DateTime startTime;
            //for phone plot
            public PlotManager.PhonePlot.PhoneCallState phoneCallState;
        }
        
        public JSONObject Save(JSONObject jsonObject)
        {
            var plotJsonObj = new JSONObject();
            var plotList = new JSONArray();
            foreach (var plotPair in _OLDplots)
            {
                //save each plot
                var plotJson = new JSONObject();
                if (Calendar.ContainsKey(plotPair.Value))
                {
                    plotJson.Add("isOnCalendar",true);
                    Calendar.TryGetValue(plotPair.Value, out DateTime time);
                    var stringTime = (SerializeManager.JsonDateTime) time;
                    plotJson.Add("startTime",stringTime.value.ToString());
                }
                else
                {
                    plotJson.Add("isOnCalendar",false);
                }
                
                plotJson.Add("plotType",plotPair.Value.GetType().ToString());
                plotJson.Add("plotState",plotPair.Value.plotState.ToString());
                var stringSpan = (SerializeManager.JsonTimeSpan) plotPair.Value.relaSpan;
                plotJson.Add("relaSpan",stringSpan.value);
                
                if(plotPair.Key.IsSubclassOf(typeof(PhonePlot)))
                {
                    PhonePlot plot = plotPair.Value as PhonePlot;
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
            var plotJsonObj = jsonObject["plot"];
            _baseTime = new SerializeManager.JsonDateTime(Convert.ToInt64((string)plotJsonObj["plotBaseTime"]));
            foreach (var jsonPlotInfo in plotJsonObj["plotList"].Values)
            {
                var plotType = Type.GetType(jsonPlotInfo["plotType"]);
                var plot = GetOrCreatePlots(plotType);
                if(Enum.TryParse<PlotState>(jsonPlotInfo["plotState"], out PlotState s))
                    plot.ChangePlotState(s);
                plot.relaSpan = new SerializeManager.JsonTimeSpan(Convert.ToInt64((string)jsonPlotInfo["relaSpan"]));
                if (plotType.IsSubclassOf(typeof(PhonePlot)))
                {
                    PhonePlot phonePlot = plot as PhonePlot;
                    Enum.TryParse<PhonePlot.PhoneCallState>(jsonPlotInfo["phoneCallState"], out phonePlot.phoneCallState);
                }
                if (plotJsonObj["isOnCalendar"]) Calendar.Add(plot, new SerializeManager.JsonDateTime(Convert.ToInt64((string)jsonPlotInfo["startTime"])));
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
        isStandingBy,
        isOnCalendar,
        isReadyToPlay,
        isPlaying,
        isFinished,
        isAbandoned,
        isBreak,
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
        protected List<Type> _parentPlots = new List<Type>();
        //these plots are only important for a special speed-up mode
        protected List<Type> _requiredPrePlots = new List<Type>();
        protected List<Type> _childPlots = new List<Type>();
        
        protected PlotState _plotState = PlotState.isStandingBy;
        public PlotState plotState
        {
            get { return _plotState; }
        }

        public void ChangePlotState(PlotState ps)
        {
            _plotState = ps;
        }

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

        //new sys
        public string name { get; private set; }
        public Type plotType { get; private set; }
        public PlotInitType initType { get; private set; }
        public int priority { get; private set; } = 0;
        public DateTime timeBasedSendTime { get; private set; }
        public TimeSpan plotBasedDelayTime { get; private set; }
        public TimeSpan waitTimeBeforeBreak { get; private set; }
        public List<string> prePlots = new List<string>();

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
        

        public void AddToCalendar()
        {
            if (_plotState == PlotState.isStandingBy)
            {
                parent.Calendar.Add(this,plotStartTime);
                this._plotState = PlotState.isOnCalendar;
            }
        }
        
        public void AddToCalendarWithSetTime(DateTime startTime)
        {
            if (_plotState == PlotState.isStandingBy)
            {
                parent.Calendar.Add(this,startTime);
                this._plotState = PlotState.isOnCalendar;
            }
        }

        public void CheckChild()
        {
            foreach (var childPlot in _childPlots)
            {
                var plotInit = parent.GetOrCreatePlots(childPlot);
                if (plotInit._plotState == PlotState.isChecking || plotInit._plotState == PlotState.isStandingBy)
                {
                    plotInit._plotState = PlotState.isChecking;
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
                if (plot._plotState != PlotState.isFinished)
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
                _plotState = PlotState.isFinished;
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
            protected List<Type> _attachedFileType = new List<Type>();
            
            public override void Reload()
            {
                if (_plotState == PlotState.isPlaying || _plotState == PlotState.isReadyToPlay)
                {
                    tm.currentTextPlot = this;
                    if(Services.saveManager.inkJson!= "") tm.currentStory.state.LoadJson(Services.saveManager.inkJson);
                    Services.eventManager.AddHandler<TextFinished>(_OnTextFinished);
                }
            }
    
            public override void Init()
            {
                // make sure no two text plots run in the same time
                if (tm.currentTextPlot != null && this != tm.currentTextPlot)
                {
                    tm.currentTextPlot.ChangePlotState(PlotState.isBreak);
                    tm.MuteAllKeyboard();
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
                _plotState = PlotState.isFinished;
            }
    
            public override bool CheckLoad()
            {
                foreach (var plotType in _requiredPrePlots)
                {
                    if (plotType.IsSubclassOf(typeof(TextPlot)))
                    {
                        var plot = parent.GetOrCreatePlots(plotType);
                        if (plot.plotState == PlotState.isPlaying || plot.plotState == PlotState.isReadyToPlay)
                            return false;
                    }
                }
    
                return true;
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

        public override bool CheckLoad()
        {
            foreach (var plotType in _requiredPrePlots)
            {
                if (plotType.IsSubclassOf(typeof(PhonePlot)))
                {
                    var plot = parent.GetOrCreatePlots(plotType);
                    if (plot.plotState == PlotState.isPlaying || plot.plotState == PlotState.isReadyToPlay)
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
                _plotState = PlotState.isBreak;
            }
            
        }

        protected void OnPhoneFinished()
        {
            if (pm.currrentPhonePlot == this)
            {
                _plotState = PlotState.isFinished;
            }
        }

        protected void InitPhoneClip(Type plotType = null)
        {
            if(plotType == null) callContent = PhoneFileAddress.GetPhoneClipName(GetType());
            else callContent = PhoneFileAddress.GetPhoneClipName(plotType);
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
            if (_plotState == PlotState.isFinished && isSend)
            {
                //TODO upload the voice mail
            }
        }

       

        public override void Start()
        {
            isSend = true;
            //TODO upload the voice mail
            _plotState = PlotState.isFinished;
        }

        public override bool CheckLoad()
        {
            foreach (var plotType in _requiredPrePlots)
            {
                var plot = parent.GetOrCreatePlots(plotType);
                if (plot.plotState == PlotState.isFinished) return true;
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
