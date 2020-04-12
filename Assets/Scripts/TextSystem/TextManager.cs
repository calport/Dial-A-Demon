﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DialADemon.Library;
using Ink.Runtime;
using SimpleJSON;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;
using Yarn.Unity.Example;

public class TextManager
{
	public Story currentStory
	{
		get
		{
			if (!ReferenceEquals(currentTextPlot,null)) return currentTextPlot.story;
			return null;
		}
	}
	public PlotManager.TextPlot currentTextPlot;

	// path of the massage bubble prefabs
    private string _demonTextBox= "Prefabs/MessageBubble_Demon";
    private string _playerTextBox= "Prefabs/MessageBubble_Player";
    private string _demonContract= "Prefabs/DemonContract";
    private string _timeStampBubble = "Prefabs/MessageBubble_TimeStamp";
    private string _endChatBubble = "Prefabs/MessageBubble_EndChat";
    private string _typingDotBubble = "Prefabs/MessageBubble_TypingDot";

    private bool isReloadingDialogueMute = false;
    public bool isLoadInitDialogueFinished = false;
    private bool isEndChatShow = false;

    public Dictionary<string, Keyboard> keyboard
    {
	    get { return TextRunnerInfo.Instance.keyboard; }
    }
    public GameObject pointerTrigger;
    public TextMeshProUGUI textBox
    {
	    get { return TextRunnerInfo.Instance.textBox; }
    }
    public Button sendButton{
	    get { return TextRunnerInfo.Instance.sendButton; }
    }
    private ScrollRect _msgScroll{
	    get { return TextRunnerInfo.Instance.msgScroll; }
    }
    public GameObject content
        {
            get { return _msgScroll.content.gameObject; }
        }
    private GameObject _dot;
    
    //this is a place that record all the dialogue lines, so it will be easier to restore the game
    public List<string> FinishedLog = new List<string>();
    //this is a place to record who says the dialogue in finished log, so it will help restore the game, while 0 is demon and 1 is the player
    public List<int> Speaker = new List<int>();

    private DateTime _lastTimeStamp = DateTime.MinValue;
    private int _dialogueLabel = 0;
    
    #region Lifecycle

    public void Init()
    {
	    Services.eventManager.AddHandler<TextFinished>(_OnTextFinished);
    }

    public void Update()
    {
        if (_msgScroll.verticalNormalizedPosition >1f && !isReloadingDialogueMute)
        {
            if(isLoadInitDialogueFinished) _LoadMoreDialogue();
            isReloadingDialogueMute = true;
            CoroutineManager.DoDelayCertainSeconds(delegate { isReloadingDialogueMute = false; },3f );
        }
    }
    public void Clear()
    {
        //save here
        Services.eventManager.RemoveHandler<TextFinished>(_OnTextFinished);
    }

    #endregion


    #region TextRunner

    public void Start()
	{
	}

	public void StartNewStory(Story newStory)
	{
		RefreshView();
	}

	public void ContinueStory()
	{
		//check if the story is finished, if not, check the json saving file and continue the old story
		RefreshView();
	}

	// This is the main function called every time the story changes. It does a few things:
	// Destroys all the old content and choices.
	// Continues over all the lines of text, then displays all the choices. If there are no choices, the story is finished!
	private void RefreshView()
	{
		
		// Read all the content until we can't continue any more
		if (currentStory.canContinue)
		{
			// Continue gets the next line of the story
			string text = currentStory.Continue();
			// This removes any white space from the text.
			text = text.Trim();
			//check all the tags and use them to rich the text and texting behaviors
			if (isSentenceFileType(currentStory.currentTags))
			{
				
			}
			var behaviorType = CheckTypingBehavior(currentStory.currentTags);
			
			//add text
			var dotSpan = AddTypingBehaviorAndReturnTotalSpan(behaviorType);
			AddNewMessage(MessageBubbleType.Demon,text,DateTime.Now + dotSpan);
			Services.textSequenceTaskRunner.AddTask(delegate
			{
				RefreshView();
			},DateTime.Now + dotSpan);
			
		}
		// Display all the choices, if there are any!
		else if (currentStory.currentChoices.Count > 0)
		{
			for (int i = 0; i < currentStory.currentChoices.Count; i++)
			{
				Choice choice = currentStory.currentChoices[i];
				CreateChoiceView(choice);
				// Tell the button what to do when we press it
				//button.onClick.AddListener(delegate { OnClickChoiceButton(choice); });
			}
		}
		// If we've read all the content and there's no choices, the story is finished!
		else
		{
			//end story
			Services.eventManager.Fire(new TextFinished(DateTime.Now));
			//Button choice = CreateChoiceView("End of story.\nRestart?");
			//choice.onClick.AddListener(delegate { StartStory(); });
		}
		
	}

	// Creates a button showing the choice text
	void CreateChoiceView(Choice choice)
	{
		//send all the options to the aiming keyboard
		string text = choice.text.Trim();
		int i = 0;
		string firstLetter = String.Empty;
		while(!System.Text.RegularExpressions.Regex.IsMatch(firstLetter, @"^[a-zA-Z]$+"))
		{
			firstLetter = text.Substring(i, 1).ToUpper();
			i++;
		}

		var key = keyboard[firstLetter.ToUpper()];

		key.isChoice = true;
		key.content = text;
		key.choiceIdex = choice.index;
	}
	
	// called by the sent button to make a selection
	public void CreatePlayerBubble(string text = null)
	{
		Debug.Assert(!(!currentStory.canContinue && text == null),"A empty player bubble has been created");
		
		if (currentStory.canContinue && text == null)
		{
			// Continue gets the next line of the story
			text = currentStory.Continue();
			// This removes any white space from the text.
			text = text.Trim();
		}

		// show text
		AddNewMessage(MessageBubbleType.Player, text, DateTime.Now);
		Services.textSequenceTaskRunner.AddTask(delegate { RefreshView(); }, DateTime.Now);

		// clear the text box
		textBox.text = String.Empty;
		//reset all the keyboard to null
		MuteAllKeyboard();

	}
	
	private void AddNewMessage(MessageBubbleType msgType, string text, DateTime shootTime)
    {
        if ((shootTime - _lastTimeStamp) > TimeSpan.FromMinutes(10f))
        {
            CreateNewTimeStamp(shootTime);
            _lastTimeStamp = shootTime;
        }
        
        switch (msgType)
        {
            case MessageBubbleType.Player:
                //create the msg info and put them in the save manager
                MessageContent msg= new MessageContent();
                msg.messageType = MessageBubbleType.Player;
                msg.content=text;
                msg.shootTime = shootTime;
                _currentDialogueMessages.Add(msg);
                
                //create the bubble
                Services.textSequenceTaskRunner.AddTask(delegate
                {
                     GameObject newTimeBox = GameObject.Instantiate(Resources.Load<GameObject>(_playerTextBox), content.transform);
	                 newTimeBox.GetComponentInChildren<TextMeshProUGUI>().text = text;
                },shootTime);
                break;
            
            case MessageBubbleType.Demon:
                msg= new MessageContent();
                msg.messageType = MessageBubbleType.Demon;
                msg.content=text;
                msg.shootTime = shootTime;
                _currentDialogueMessages.Add(msg);
                
                Services.textSequenceTaskRunner.AddTask(delegate
                {
                    GameObject newTimeBox = GameObject.Instantiate(Resources.Load<GameObject>(_demonTextBox), content.transform);
                    newTimeBox.GetComponentInChildren<TextMeshProUGUI>().text = text;
                },shootTime);
                break;
            case MessageBubbleType.Prefab:
	            Debug.Log("Change to use AddNewFileMessage to initiate a prefab msg.");
	            break;
        }
    }

	/// <summary>
	/// This func is for the plot manager specifically to create new file bubbles
	/// It is only called by outside managers and doesn't belongs to the automatic text manager
	/// </summary>
	/// <param name="prefabType"></param>
	/// <param name="shootTime"></param>
	public void AddNewFileMessage(Type prefabType, DateTime shootTime)
	{
		if ((shootTime - _lastTimeStamp) > TimeSpan.FromMinutes(10f))
		{
			CreateNewTimeStamp(shootTime);
			_lastTimeStamp = shootTime;
		}
		
		var msg = new MessageContent();
		msg.messageType = MessageBubbleType.Prefab;
		msg.prefabType = prefabType;
		msg.shootTime = shootTime;
		_currentDialogueMessages.Add(msg);
		
		Services.textSequenceTaskRunner.AddTask(delegate
		{
			if (prefabType.IsSubclassOf(typeof(PlotManager.TextFilePlot)))
			{
				PlotManager.TextFilePlot tfp =
					Services.plotManager.GetOrCreatePlots(prefabType) as PlotManager.TextFilePlot;
				tfp.CreateBubble();
			}
		},shootTime);
	}
	

	private void CreateNewTimeStamp(DateTime time)
	{
		MessageContent msg= new MessageContent();
		msg.messageType = MessageBubbleType.TimeStamp;
		msg.content=String.Empty;
		msg.shootTime = time;
		_currentDialogueMessages.Add(msg);
        
		Services.textSequenceTaskRunner.AddTask(delegate
		{
			GameObject newTimeBox = GameObject.Instantiate(Resources.Load<GameObject>(_timeStampBubble), content.transform);
			newTimeBox.GetComponentInChildren<TextMeshProUGUI>().text = TimeStamp.GetTimeStamp(time);
		},time);
	}
	
	private enum TypingBehavior
	{
		Default,
		Hesitate,
		Delay,
	}

	private TypingBehavior CheckTypingBehavior(List<string> tagList)
	{
		foreach (var tag in tagList)
		{
			if (string.Compare(tag, "Hesitate") == 0 || string.Compare(tag, "hesitate") == 0)
				return TypingBehavior.Hesitate;
			if (string.Compare(tag, "Delay") == 0 || string.Compare(tag, "delay") == 0)
				return TypingBehavior.Delay;
		}
		return TypingBehavior.Default;
	}

	private bool isSentenceFileType(List<string> tagList)
	{
		foreach (var tag in tagList)
		{
			if (string.Compare(tag, "File") == 0 || string.Compare(tag, "file") == 0) return true;
		}

		return false;
	}

	private TimeSpan AddTypingBehaviorAndReturnTotalSpan(TypingBehavior behavior)
	{
		switch (behavior)
		{
			case TypingBehavior.Default:
				Services.textSequenceTaskRunner.AddTask(delegate { AddDot(); }, DateTime.Now );
				Services.textSequenceTaskRunner.AddTask(delegate { DeleteDot(); }, DateTime.Now + TimeSpan.FromSeconds(3f));
				return TimeSpan.FromSeconds(3f);
			case TypingBehavior.Hesitate:
				Services.textSequenceTaskRunner.AddTask(delegate { AddDot(); }, DateTime.Now );
				Services.textSequenceTaskRunner.AddTask(delegate { DeleteDot(); }, DateTime.Now + TimeSpan.FromSeconds(3f) );
				Services.textSequenceTaskRunner.AddTask(delegate { AddDot(); }, DateTime.Now + TimeSpan.FromSeconds(5f));
				Services.textSequenceTaskRunner.AddTask(delegate { DeleteDot(); }, DateTime.Now + TimeSpan.FromSeconds(8f) );
				return TimeSpan.FromSeconds(8f);
			case TypingBehavior.Delay:
				Services.textSequenceTaskRunner.AddTask(delegate { AddDot(); }, DateTime.Now + TimeSpan.FromSeconds(20f));
				return TimeSpan.FromSeconds(20f);
		}
		return TimeSpan.FromSeconds(0f);
	}

	private TimeSpan ReturnTypingBehaviorTotalSpan(TypingBehavior behavior)
	{
		switch (behavior)
		{
			case TypingBehavior.Default:
				return TimeSpan.FromSeconds(3f);
			case TypingBehavior.Hesitate:
				return TimeSpan.FromSeconds(8f);
			case TypingBehavior.Delay:
				return TimeSpan.FromSeconds(20f);
		}
		return TimeSpan.FromSeconds(0f);
	}
	private void AddDot()
	{
		GameObject newBox = GameObject.Instantiate(Resources.Load<GameObject>(_typingDotBubble), content.transform);
		_dot = newBox;
	}

	private void DeleteDot()
	{
		GameObject.Destroy(_dot);
	}

	#endregion
	
	#region Save

	
	public enum MessageBubbleType
	{
		Demon,
		Player,
		TimeStamp,
		Prefab
	}
	
	[Serializable]
	public struct MessageContent
	{
		public MessageBubbleType messageType;
		public string content;
		public DateTime shootTime;
		public Type prefabType;
	}
	
	private List<MessageContent> _currentDialogueMessages = new List<MessageContent>();
	private List<MessageContent[]> _dialogueMessages = new List<MessageContent[]>();

	public JSONObject Save(JSONObject jsonObject)
	{
		//save the data
		var isLastDialogueFinished = false;
		if (_currentDialogueMessages.Count != 0)
			_OnTextFinished(new TextFinished(DateTime.MinValue));
		else
			isLastDialogueFinished = true;
		
		var dialogueLengthInfo = new List<int>(); 
		var dialogueMessages = new List<MessageContent>(); 
		SerializeManager.Serialize2DArray(_dialogueMessages.ToArray(),out dialogueLengthInfo, out dialogueMessages);

		if(currentStory!= null)  File.WriteAllText(Services.saveManager.inkjsonPath, Services.textManager.currentStory.state.ToJson(), Encoding.UTF8);
		else File.WriteAllText(Services.saveManager.inkjsonPath, "");

		//write them in the json file
		var textJsonObj = new JSONObject();
		textJsonObj.Add("isLastDialogueFinished", isLastDialogueFinished);
		var jsonDialogueLengthInfo = new JSONArray();
		foreach (var lengthInfo in dialogueLengthInfo)
			jsonDialogueLengthInfo.Add(lengthInfo);
		textJsonObj.Add("dialogueLengthInfo", jsonDialogueLengthInfo);
		var jsonDialogueContent = new JSONArray();
		foreach (var msg in dialogueMessages)
		{
			var msgObj = new JSONObject();
			msgObj.Add("messageType",msg.messageType.ToString());
			msgObj.Add("content",msg.content);
			var shootTimeString = (SerializeManager.JsonDateTime) msg.shootTime;
			msgObj.Add("shootTime",shootTimeString.value.ToString());
			if(!ReferenceEquals(msg.prefabType,null)) msgObj.Add("prefabType",msg.prefabType.ToString());
			jsonDialogueContent.Add(msgObj);
		}
		textJsonObj.Add("dialogueMessages", jsonDialogueContent);
		var stringTimeStamp = (SerializeManager.JsonDateTime) _lastTimeStamp;
		textJsonObj.Add("lastTimeStamp", stringTimeStamp.value.ToString());
		
		jsonObject.Add("text", textJsonObj);
		return jsonObject;
	}

    public void Load(JSONNode jsonObject)
    {
	    var textJsonObj = jsonObject["text"];
	    
	    bool isLastDialogueFinished = textJsonObj["isLastDialogueFinished"];
	    
	    //prepare finished msg
	    var jsonDialogueLengthInfo = textJsonObj["dialogueLengthInfo"];
	    var jsonDialogueMessages = textJsonObj["dialogueMessages"];
	    var _msgContent = new List<MessageContent>();
	    foreach (var jsonMsg in jsonDialogueMessages.Values)
	    {
		    var msg = new MessageContent(); 
		    Enum.TryParse<MessageBubbleType>(jsonMsg["messageType"], out msg.messageType);
		    msg.content = jsonMsg["content"];
		    msg.shootTime = new SerializeManager.JsonDateTime(Convert.ToInt64((string)jsonMsg["shootTime"]));
		    if(jsonMsg["prefabType"]!=null)
			    msg.prefabType = Type.GetType(jsonMsg["prefabType"]);
		    _msgContent.Add(msg);
	    }
	    var _msgLengthInfo = new List<int>();
	    foreach (var length in jsonDialogueLengthInfo.Values)
		    _msgLengthInfo.Add(length);
	    MessageContent[][] arrayInfo= SerializeManager.Deserialize2DArray(_msgLengthInfo, _msgContent);
	    //get the info from the save file
	    foreach (var msgArray in arrayInfo)
		    _dialogueMessages.Add(msgArray);
	    if(!isLastDialogueFinished) 
	    {
		    if (_dialogueMessages.Count > 0)
		    {
			    var leftDialogue = _dialogueMessages[_dialogueMessages.Count - 1];
			    _currentDialogueMessages = leftDialogue.ToList();
			    _dialogueMessages.Remove(_dialogueMessages[_dialogueMessages.Count - 1]);
		    }
	    }

	    _lastTimeStamp = new SerializeManager.JsonDateTime(Convert.ToInt64((string)textJsonObj["lastTimeStamp"]));
	    _LoadInitialDialogue();
	    _LoadMoreDialogue();
	    _LoadDialogueForOldPlotWhenAPPisOff();
	    
	    isLoadInitDialogueFinished = true;
    }
    private void _LoadDialogueForOldPlotWhenAPPisOff()
    {
	    //protect when currentStory doesnt exist
	    if(currentTextPlot== null) return;
	    
	    var lastMsg = FindTheLastMessage();
	    var startTime = lastMsg.shootTime;
	    while (currentStory.canContinue)
	    {
		    var text = currentStory.Continue();
		    text = text.Trim();
		    var behaviorType = CheckTypingBehavior(currentStory.currentTags);
		    var dotSpan = ReturnTypingBehaviorTotalSpan(behaviorType);
		    var shootTime = startTime + dotSpan;
		    if (shootTime > DateTime.Now)
		    {
			    dotSpan = AddTypingBehaviorAndReturnTotalSpan(behaviorType);
			    AddNewMessage(MessageBubbleType.Demon,text,shootTime);
			    Services.textSequenceTaskRunner.AddTask(delegate
				    {
					    RefreshView();
				    },shootTime);
			    return;
		    }
		    
		    AddNewMessage(MessageBubbleType.Demon,text,shootTime);
		    startTime += dotSpan;
	    }

	    if (currentTextPlot.isBreak()) currentTextPlot.ChangePlotState(PlotManager.PlotState.isBreak);
	    
	    if (currentStory.currentChoices.Count > 0)
	    {
		    for (int i = 0; i < currentStory.currentChoices.Count; i++)
		    {
			    Choice choice = currentStory.currentChoices[i];
			    CreateChoiceView(choice);
		    }
	    }
	    else
	    {
		    //end story
		    Services.eventManager.Fire(new TextFinished(startTime));
	    }
    }

    public void _LoadDialogueForNewPlotWhenAPPisOff(DateTime originalStartTime)
    {
	    //protect when currentStory doesnt exist
	    if(currentTextPlot== null) return;
	    
	    var startTime = originalStartTime;
	    while (currentStory.canContinue)
	    {
		    var text = currentStory.Continue();
		    text = text.Trim();
		    var behaviorType = CheckTypingBehavior(currentStory.currentTags);
		    var dotSpan = ReturnTypingBehaviorTotalSpan(behaviorType);
		    var shootTime = startTime + dotSpan;
		    if (shootTime > DateTime.Now)
		    {
			    dotSpan = AddTypingBehaviorAndReturnTotalSpan(behaviorType);
			    AddNewMessage(MessageBubbleType.Demon,text,shootTime);
			    Services.textSequenceTaskRunner.AddTask(delegate
			    {
				    RefreshView();
			    },shootTime);
			    return;
		    }
		    
		    AddNewMessage(MessageBubbleType.Demon,text,shootTime);
		    startTime += dotSpan;
	    }

	    if (currentTextPlot.isBreak()) currentTextPlot.ChangePlotState(PlotManager.PlotState.isBreak);
	    
	    if (currentStory.currentChoices.Count > 0)
	    {
		    for (int i = 0; i < currentStory.currentChoices.Count; i++)
		    {
			    Choice choice = currentStory.currentChoices[i];
			    CreateChoiceView(choice);
		    }
	    }
	    else
	    {
		    //end story
		    Services.eventManager.Fire(new TextFinished(startTime));
	    }
    }
    private void _LoadInitialDialogue()
    {
        _dialogueLabel = _dialogueMessages.Count;
        if(_currentDialogueMessages.Count!=0) 
	        for (int i = _currentDialogueMessages.Count-1; i>-1; i--)
			{
	            var msg = _currentDialogueMessages[i];
	            switch (msg.messageType)
	            {
	                case MessageBubbleType.Demon:
	                    Services.textSequenceTaskRunner.AddTask(delegate
	                     {
	                         GameObject newTimeBox = GameObject.Instantiate(Resources.Load<GameObject>(_demonTextBox), content.transform);
	                         newTimeBox.GetComponentInChildren<TextMeshProUGUI>().text = msg.content;
	                         newTimeBox.transform.SetSiblingIndex(0);
	                     }, msg.shootTime);
	                    break;
	                case MessageBubbleType.Player:
	                    Services.textSequenceTaskRunner.AddTask(delegate
	                    {
	                        GameObject newTimeBox = GameObject.Instantiate(Resources.Load<GameObject>(_playerTextBox), content.transform);
	                        newTimeBox.GetComponentInChildren<TextMeshProUGUI>().text = msg.content;
	                        newTimeBox.transform.SetSiblingIndex(0);
	                    },msg.shootTime);
	                    break;
	                case MessageBubbleType.TimeStamp:
	                    Services.textSequenceTaskRunner.AddTask(delegate
	                    {
	                        GameObject newTimeBox = GameObject.Instantiate(Resources.Load<GameObject>(_timeStampBubble), content.transform);
	                        newTimeBox.GetComponentInChildren<TextMeshProUGUI>().text = TimeStamp.GetTimeStamp(msg.shootTime);
	                        newTimeBox.transform.SetSiblingIndex(0);
	                    },msg.shootTime);
	                    break;
	                case MessageBubbleType.Prefab:
		                Services.textSequenceTaskRunner.AddTask(delegate
		                {
			                Debug.Log("the msg was read");
			                var prefabType = msg.prefabType;
			                if (prefabType.IsSubclassOf(typeof(PlotManager.TextFilePlot)))
			                {
				                PlotManager.TextFilePlot tfp =
					                Services.plotManager.GetOrCreatePlots(prefabType) as PlotManager.TextFilePlot;
				                tfp.CreateBubble();
			                }
		                },msg.shootTime);
		                break;
	            }
			}

        _dialogueLabel --;
    }
    
    private void _LoadMoreDialogue()
    {
	    if (_dialogueLabel < 0)
        {
            if (!isEndChatShow)
            {
                Services.textSequenceTaskRunner.AddSideTask(delegate
                {
                    GameObject newTimeBox =
                        GameObject.Instantiate(Resources.Load<GameObject>(_endChatBubble), content.transform);
                    newTimeBox.transform.SetSiblingIndex(0);
                },DateTime.Now);
                isEndChatShow = true;
            }

            return;
        }
        
        MessageContent[] dialogueArray = _dialogueMessages[_dialogueLabel];
        for (int i = dialogueArray.Length-1; i>-1; i--)
        {
            var msg = dialogueArray[i];
            switch (msg.messageType)
            {
                case MessageBubbleType.Demon:
                    Services.textSequenceTaskRunner.AddTask(delegate
                    {
                        GameObject newTimeBox = GameObject.Instantiate(Resources.Load<GameObject>(_demonTextBox), content.transform);
                        newTimeBox.GetComponentInChildren<TextMeshProUGUI>().text = msg.content;
                        newTimeBox.transform.SetSiblingIndex(0);
                    },msg.shootTime);
                    break;
                case MessageBubbleType.Player:
                    Services.textSequenceTaskRunner.AddTask(delegate
                    {
                        GameObject newTimeBox = GameObject.Instantiate(Resources.Load<GameObject>(_playerTextBox), content.transform);
                        newTimeBox.GetComponentInChildren<TextMeshProUGUI>().text = msg.content;
                        newTimeBox.transform.SetSiblingIndex(0);
                    },msg.shootTime);
                    break;
                case MessageBubbleType.TimeStamp:
                    Services.textSequenceTaskRunner.AddTask(delegate
                    {
                        GameObject newTimeBox = GameObject.Instantiate(Resources.Load<GameObject>(_timeStampBubble), content.transform);
                        newTimeBox.GetComponentInChildren<TextMeshProUGUI>().text = TimeStamp.GetTimeStamp(msg.shootTime);
                        newTimeBox.transform.SetSiblingIndex(0);
                    },msg.shootTime);
                    break;
                case MessageBubbleType.Prefab:
	                Services.textSequenceTaskRunner.AddTask(delegate
	                {
		                var prefabType = msg.prefabType;
		                if (prefabType.IsSubclassOf(typeof(PlotManager.TextFilePlot)))
		                {
			                PlotManager.TextFilePlot tfp =
				                Services.plotManager.GetOrCreatePlots(prefabType) as PlotManager.TextFilePlot;
			                tfp.CreateBubble();
		                }
	                },msg.shootTime);
	                break;
            }
            
        }
        _dialogueLabel--;
    }

    
    public MessageContent FindTheLastMessage()
    {
	    if (_currentDialogueMessages.Count == 0 && _dialogueMessages.Count==0) return new MessageContent();
	    if (_currentDialogueMessages.Count != 0) return _currentDialogueMessages[_currentDialogueMessages.Count - 1];
        
	    var oldDialogue = _dialogueMessages[_dialogueMessages.Count - 1];
	    if (oldDialogue.Length != 0) return oldDialogue[oldDialogue.Length - 1];
	    return new MessageContent();
    }
    
    #endregion

    #region ExtraFunc

    public void MuteAllKeyboard()
    {
	    foreach (var key in keyboard.Values)
	    {
		    key.isChoice = false;
	    }
    }

    #endregion

    #region Events

    private void _OnTextFinished(TextFinished e)
    {
	    //pack the msg and put them into the big list for saving
	    var msgArray =_currentDialogueMessages.ToArray();
	    _dialogueMessages.Add(msgArray);
	    _currentDialogueMessages = new List<MessageContent>();
    }

    #endregion
}
