using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DialADemon.Library;
using Ink.Runtime;
using JetBrains.Annotations;
using SimpleJSON;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

public class TextManager
{
	//text manager state info
	[CanBeNull]
	public Story currentStory => currentText?.story;
	private PlotManager.Text _currentText;
	public PlotManager.Text currentText
	{
		get
		{
			if (!ReferenceEquals(_currentText, null) && _currentText.plotState == PlotManager.PlotState.Playing)
				return _currentText;
			foreach (var playingPlot in Services.plotManager.playingPlot)
				if (playingPlot is PlotManager.Text)
				{
					_currentText = playingPlot as PlotManager.Text;
					return _currentText;
				}
			return null;
		}
	}

	public bool isChoosing = false;
	private RunState _textManagerState = RunState.Play;

	// path of the massage bubble prefabs
    private string _demonTextBox= "Prefabs/MessageBubble_Demon";
    private string _playerTextBox= "Prefabs/MessageBubble_Player";
    private string _demonContract= "Prefabs/DemonContract";
    private string _timeStampBubble = "Prefabs/MessageBubble_TimeStamp";
    private string _endChatBubble = "Prefabs/MessageBubble_EndChat";
    private string _typingDotBubble = "Prefabs/MessageBubble_TypingDot";

    private bool _isReloadingDialogueMute = false;
    private bool _isEndChatShow = false;

    //text manager related reference
    public Dictionary<string, Keyboard> keyboard => TextRunnerInfo.Instance.keyboard;
    public GameObject pointerTrigger;
    public TextMeshProUGUI textBox => TextRunnerInfo.Instance.textBox;
    public Button sendButton =>  TextRunnerInfo.Instance.sendButton;
    private ScrollRect _msgScroll => TextRunnerInfo.Instance.msgScroll;
	public GameObject content => _msgScroll.content.gameObject;
	private GameObject _dot;
    
    //this is a place that record all the dialogue lines, so it will be easier to restore the game
    public List<string> FinishedLog = new List<string>();
    //this is a place to record who says the dialogue in finished log, so it will help restore the game, while 0 is demon and 1 is the player
    public List<int> Speaker = new List<int>();

    private DateTime _lastTimeStamp = DateTime.MinValue;
    private Dictionary<GameObject, DateTime> _timeStampBoxes = new Dictionary<GameObject, DateTime>();
    private int _dialogueLabel = -1;

    private float _typingSpeed = 0.1f;

    #region Lifecycle

    public void Init()
    {
    }

    public void Update()
    {
        if (_msgScroll.verticalNormalizedPosition >1f && !_isReloadingDialogueMute)
        { 
	        _LoadMoreDialogue();
            _isReloadingDialogueMute = true;
            CoroutineManager.DoDelayCertainSeconds(delegate { _isReloadingDialogueMute = false; },3f );
        }
    }
    public void Clear()
    {
        //save here
    }

    #endregion

    #region TextRunner

    public void ContinueOrStartStory()
	{
		//check if the story is finished, if not, check the json saving file and continue the old story
		_textManagerState = RunState.Play;
		if(currentStory!= null) _RefreshView();
	}

	public void PauseStory()
	{
		_textManagerState = RunState.Pause;
	}

	// This is the main function called every time the story changes. It does a few things:
	// Destroys all the old content and choices.
	// Continues over all the lines of text, then displays all the choices. If there are no choices, the story is finished!
	/// <summary>
	/// This function is only used on real-time conversation
	/// </summary>
	private void _RefreshView()
	{
		if(_textManagerState!=RunState.Play) return;
		// Read all the content until we can't continue any more
		if (currentStory.canContinue)
		{
			// Continue gets the next line of the story
			string text = currentStory.Continue();
			// This removes any white space from the text.
			text = text.Trim();

			_AddNewDemonMessageBox(MessageBubbleType.Demon,text,DateTime.Now, out DateTime finalShootTime);
			Services.textSequenceTaskRunner.AddTask(delegate
			{
				_RefreshView();
			},DateTime.Now);
			
		}
		// Display all the choices, if there are any!
		else if (currentStory.currentChoices.Count > 0)
			_CreateChoices(DateTime.Now);
			// If we've read all the content and there's no choices, the story is finished!
		else
		{
			//end story
			currentText.ChangePlotState(PlotManager.PlotState.Finished);
			//Button choice = CreateChoiceView("End of story.\nRestart?");
			//choice.onClick.AddListener(delegate { StartStory(); });
		}
		
	}

	private void _CreateChoices(DateTime chooseTime)
	{
		isChoosing = true;
		currentText.UpdateBreakTime(chooseTime);
		for (int i = 0; i < currentStory.currentChoices.Count; i++)
		{
			Choice choice = currentStory.currentChoices[i];
			_CreateChoiceView(choice);
		}
	}
	// Creates a button showing the choice text
	private void _CreateChoiceView(Choice choice)
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
		_AddNewDemonMessageBox(MessageBubbleType.Player, text, DateTime.Now, out DateTime finalShootTime);
		Services.textSequenceTaskRunner.AddTask(delegate { _RefreshView(); }, DateTime.Now);
	}

	public void CleanChoices()
	{
		isChoosing = false;
		// clear the text box
		textBox.text = String.Empty;
		//reset all the keyboard to null
		MuteAllKeyboard();
	}
	
	/// <summary>
	/// Adding new message box with raw data that comes from a fresh new ink file
	/// </summary>
	/// <param name="msgType"></param>
	/// <param name="text"></param>
	/// <param name="shootTime"></param>
	/// <param name="isAddedToTop"></param>
	private void _AddNewDemonMessageBox(MessageBubbleType msgType, string text, DateTime rawShootTime, out DateTime shootTime)
	{
		//processing label elements in the raw text and clean them to get the real text
		text = TextEffectManager.ProcessingTags(text, out List<UndefinedTagInfo> undefinedTagInfos, out List<Tag> gameRelatedTags);
		shootTime = _AddTypingBehaviorBox(text, gameRelatedTags, rawShootTime);
		
		//Time Stamp appearing logic
        if ((shootTime - _lastTimeStamp) > TimeSpan.FromMinutes(10f))
        {
	        MessageContent ts= new MessageContent();
	        ts.messageType = MessageBubbleType.TimeStamp;
	        ts.content=String.Empty;
	        ts.shootTime = shootTime;
	        _currentDialogueMessages.Add(ts);
            
	        _AddMessageBox(ts);
            _lastTimeStamp = shootTime;
        }
        
        MessageContent msg= new MessageContent();
        msg.messageType = msgType;
        msg.content=text;
        msg.shootTime = shootTime;
        _currentDialogueMessages.Add(msg);
        
        _AddMessageBox(msg);

        //Prefab Logic
        foreach (var textFiles in gameRelatedTags.Where(tag => tag is TextFile).ToList())
        {
	        var file = textFiles as TextFile;
	        
	        if ((shootTime - _lastTimeStamp) > TimeSpan.FromMinutes(10f))
	        {
		        MessageContent ts= new MessageContent();
		        ts.messageType = MessageBubbleType.TimeStamp;
		        ts.content=String.Empty;
		        ts.shootTime = shootTime + TimeSpan.FromSeconds(1);
		        _currentDialogueMessages.Add(ts);
            
		        _AddMessageBox(ts);
		        _lastTimeStamp = shootTime;
	        }
		
	        msg = new MessageContent();
	        msg.messageType = MessageBubbleType.Prefab;
	        msg.content = string.Empty;
	        msg.fileBubbleName = file.fileBubble;
	        msg.fileContentName = file.fileContent;
	        msg.shootTime = shootTime + TimeSpan.FromSeconds(1);
	        _currentDialogueMessages.Add(msg);
		
	        _AddMessageBox(msg);
        }
    }

	/// <summary>
	/// Create message boxes according to msg data
	/// </summary>
	/// <param name="msg"></param>
	/// <param name="shootTime"></param>
	/// <param name="isAddedToTop"></param>
	private void _AddMessageBox(MessageContent msg, bool isAddedToTop = false)
	{
		string boxPrefab = string.Empty;
		if (msg.messageType == MessageBubbleType.Player) boxPrefab = _playerTextBox;
		if (msg.messageType == MessageBubbleType.Demon) boxPrefab = _demonTextBox;
		if (msg.messageType == MessageBubbleType.TimeStamp) boxPrefab = _timeStampBubble;
		if (msg.messageType == MessageBubbleType.End) boxPrefab = _endChatBubble;
		if (msg.messageType == MessageBubbleType.Prefab) boxPrefab = "Prefabs/FileBubble/" + msg.fileBubbleName;

		string textContent = String.Empty;
		if (msg.messageType == MessageBubbleType.TimeStamp) textContent = TimeStamp.GetTimeStamp(msg.shootTime);
		if (msg.messageType == MessageBubbleType.End) textContent = "This is the end of the chat";
		else textContent = msg.content;

		Services.textSequenceTaskRunner.AddTask(delegate
		{
			GameObject newBox = GameObject.Instantiate(Resources.Load<GameObject>(boxPrefab), content.transform);
			
			if(!ReferenceEquals(newBox.GetComponentInChildren<TextMeshProUGUI>(),null))
				newBox.GetComponentInChildren<TextMeshProUGUI>().text = textContent;
			if(!ReferenceEquals(newBox.GetComponentInChildren<OpenFileButton>(),null))
				newBox.GetComponentInChildren<OpenFileButton>().fileContentName = msg.fileContentName;
			
			if(isAddedToTop) newBox.transform.SetSiblingIndex(0);
			if(msg.messageType == MessageBubbleType.TimeStamp) _timeStampBoxes.Add(newBox,msg.shootTime);
		},msg.shootTime);
		
	}

	private void _UpdateTimeStamp()
	{
		foreach (var timeStamp in _timeStampBoxes)
			timeStamp.Key.GetComponent<TextMeshProUGUI>().text = TimeStamp.GetTimeStamp(timeStamp.Value);
	}

	private DateTime _AddTypingBehaviorBox(string finalText, List<Tag> tags,DateTime rawShootTime)
	{
		float basicTypingTime = finalText.Length * _typingSpeed;
		float delaySec = 0f;
		
		var typeTags =tags.FindAll(t => t is TypingBehavior);
		Debug.Log(typeTags.Count);
		var delayList = typeTags.Where(t => t is delay);
		foreach (var t in typeTags.Where(t => t is delay))
		{
			delay d = t as delay;
			delaySec += d.length; 
		}
		rawShootTime += TimeSpan.FromSeconds(delaySec);

		fast fast = typeTags.Find(t => t is fast) as fast;
		slow slow = typeTags.Find(t => t is slow) as slow;
		doubt doubt = typeTags.Find(t => t is doubt) as doubt;
		if (!ReferenceEquals(fast, null)) basicTypingTime *= fast.length;
		if (!ReferenceEquals(slow, null)) basicTypingTime *= slow.length;
		
		if (!ReferenceEquals(doubt, null))
		{
			//if final bubble shoot time is before DateTime.Now
			//no effects would be shown
			if(rawShootTime + TimeSpan.FromSeconds(basicTypingTime+doubt.length)< DateTime.Now)
				return rawShootTime + TimeSpan.FromSeconds(basicTypingTime+doubt.length);
			
			float doubtTime = 0f;
			float dotExistLength = 0f;
			while (doubtTime< doubt.length)
			{
				Services.textSequenceTaskRunner.AddTask(DeleteDot,rawShootTime+ TimeSpan.FromSeconds(doubtTime));
				doubtTime += UnityEngine.Random.Range(0f, 2f);
				Services.textSequenceTaskRunner.AddTask(AddDot,rawShootTime+ TimeSpan.FromSeconds(doubtTime));
				dotExistLength = UnityEngine.Random.Range(0f, 4f);
				doubtTime += dotExistLength;
			}
			Services.textSequenceTaskRunner.AddTask(DeleteDot,rawShootTime+ TimeSpan.FromSeconds(doubtTime-dotExistLength+basicTypingTime));
			return rawShootTime + TimeSpan.FromSeconds(doubtTime-dotExistLength+basicTypingTime);
		}
		else
		{
			//if final bubble shoot time is before DateTime.Now
			//no effects would be shown
			if(rawShootTime + TimeSpan.FromSeconds(basicTypingTime)< DateTime.Now)
				return rawShootTime + TimeSpan.FromSeconds(basicTypingTime);
			
			Services.textSequenceTaskRunner.AddTask(AddDot,rawShootTime);
			Services.textSequenceTaskRunner.AddTask(DeleteDot,rawShootTime+ TimeSpan.FromSeconds(basicTypingTime));
			return rawShootTime + TimeSpan.FromSeconds(basicTypingTime);
		}
		
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
		Prefab,
		End
	}
	
	[Serializable]
	public struct MessageContent
	{
		public MessageBubbleType messageType;
		public string content;
		public DateTime shootTime;
		public string fileBubbleName;
		public string fileContentName;
	}
	
	private List<MessageContent> _currentDialogueMessages = new List<MessageContent>();
	private List<MessageContent[]> _dialogueMessages = new List<MessageContent[]>();

	public JSONObject Save(JSONObject jsonObject)
	{
		//save the data
		var isLastDialogueFinished = false;
		if (_currentDialogueMessages.Count != 0)
			WrapAndSavePlotDialogues();
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
			if(!ReferenceEquals(msg.fileBubbleName,null)) msgObj.Add("fileBubbleName",msg.fileBubbleName);
			if(!ReferenceEquals(msg.fileContentName,null)) msgObj.Add("fileBubbleName",msg.fileContentName);
			jsonDialogueContent.Add(msgObj);
		}
		textJsonObj.Add("dialogueMessages", jsonDialogueContent);
		if (_lastTimeStamp != DateTime.MinValue)
		{
			var stringTimeStamp = (SerializeManager.JsonDateTime) _lastTimeStamp;
			textJsonObj.Add("lastTimeStamp", stringTimeStamp.value.ToString());
		}
		
		
		jsonObject.Add("text", textJsonObj);
		return jsonObject;
	}

    public void LoadFromFile(JSONNode jsonObject)
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
		    if(jsonMsg["fileBubbleName"]!=null) msg.fileBubbleName = jsonMsg["fileBubbleName"];
		    if(jsonMsg["fileContentName"]!=null) msg.fileContentName = jsonMsg["fileContentName"];
		    _msgContent.Add(msg);
	    }
	    var _msgLengthInfo = new List<int>();
	    foreach (var length in jsonDialogueLengthInfo.Values)
		    _msgLengthInfo.Add(length);
	    MessageContent[][] arrayInfo= SerializeManager.Deserialize2DArray(_msgLengthInfo, _msgContent);
	    
	    if(Services.saveManager.inkJson!= "") currentStory.state.LoadJson(Services.saveManager.inkJson);
	    
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
	    if(textJsonObj["lastTimeStamp"]!= null)
		    _lastTimeStamp = new SerializeManager.JsonDateTime(Convert.ToInt64((string)textJsonObj["lastTimeStamp"]));
	    
	    //recreate dialogues
	    _LoadInitialDialogue();
	    _LoadMoreDialogue();
	    _LoadCurrentPlotMessageDuringPlayerOffTime();
    }

    public void UpdatePlayerOffTime()
    {
	   
    }

    public void _LoadDialogueForNewPlotWhenAPPisOff(DateTime originalStartTime)
    {
	    //protect when currentStory doesnt exist
	    if(currentText== null) return;
	    
	    var rawShootTime = originalStartTime;
	    while (currentStory.canContinue)
	    {
		    var text = currentStory.Continue();
		    text = text.Trim();
		    
		    _AddNewDemonMessageBox(MessageBubbleType.Demon,text,rawShootTime, out DateTime finalShootTime);
		    rawShootTime = finalShootTime;
		    if (finalShootTime > DateTime.Now)
		    {
			    Services.textSequenceTaskRunner.AddTask(delegate
			    {
				    _RefreshView();
			    },finalShootTime);
			    return;
		    }
	    }

	    if (currentStory.currentChoices.Count > 0)
	    {
		    _CreateChoices(rawShootTime);
		    if (currentText.CheckAndBreakIfItsBreakTime())
			    CleanChoices();
	    }
	    else
	    {
		    //end story
		    currentText.ChangePlotState(PlotManager.PlotState.Finished);
	    }
    }
    private void _LoadInitialDialogue()
    {
        _dialogueLabel = _dialogueMessages.Count;
        if(_currentDialogueMessages.Count!=0) 
	        for (int i = _currentDialogueMessages.Count-1; i>-1; i--)
			{
	            var msg = _currentDialogueMessages[i];
	            //TODO might need some typing behaviors
	            _AddMessageBox(msg,true);
			}

        _dialogueLabel --;
    }
    
    private void _LoadMoreDialogue()
    {
	    if (_dialogueLabel < 0)
        {
            if (!_isEndChatShow)
            {
	            MessageContent msg= new MessageContent();
	            msg.messageType = MessageBubbleType.End;
	            msg.content = String.Empty;
	            msg.shootTime = DateTime.Now;
	            
	            _AddMessageBox(msg,true);
                _isEndChatShow = true;
            }

            return;
        }
        
        MessageContent[] dialogueArray = _dialogueMessages[_dialogueLabel];
        for (int i = dialogueArray.Length-1; i>-1; i--)
        {
            var msg = dialogueArray[i];
            _AddMessageBox(msg,true);
        }
        _dialogueLabel--;
    }
    
    private void _LoadCurrentPlotMessageDuringPlayerOffTime()
    {
	    //protect when currentStory doesnt exist
	    if(currentText== null) return;
	    
	    var lastMsg = FindTheLastMessage();
	    var rawShootTime = lastMsg.shootTime;
	    while (currentStory.canContinue)
	    {
		    var text = currentStory.Continue();
		    text = text.Trim();
		    _AddNewDemonMessageBox(MessageBubbleType.Demon,text,rawShootTime, out DateTime finalShootTime);
		    rawShootTime = finalShootTime;
		    if (finalShootTime > DateTime.Now)
		    {
			    Services.textSequenceTaskRunner.AddTask(delegate
			    {
				    _RefreshView();
			    },finalShootTime);
			    return;
		    }
	    }

	    //the story is not end yet
	    if (currentStory.currentChoices.Count > 0)
	    {
		    _CreateChoices(rawShootTime);
		    if (currentText.CheckAndBreakIfItsBreakTime())
			    CleanChoices();
	    }
	    //story ends
	    else
	    {
		    //end story
		    currentText.ChangePlotState(PlotManager.PlotState.Finished);
	    }
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
		    key.isChoice = false;
    }

    #endregion

    #region Events

    public void WrapAndSavePlotDialogues()
    {
	    //pack the msg and put them into the big list for saving
	    var msgArray =_currentDialogueMessages.ToArray();
	    _dialogueMessages.Add(msgArray);
	    _currentDialogueMessages = new List<MessageContent>();
    }

    #endregion
}
