using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using DialADemon.Library;
using Ink.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;
using UnityEngine.UI;
using Yarn.Unity;
using Yarn.Unity.Example;

public class TextManager
{
	public Story currentStory
	{
		get
		{
			if (currentTextPlot != null) return currentTextPlot.story;
			else return null;
		}
	}
	public string inkJson;
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
    public Text textBox
    {
	    get { return TextRunnerInfo.Instance.textBox; }
    }
    public Button sendButton{
	    get { return TextRunnerInfo.Instance.sendButton; }
    }
    private ScrollRect _msgScroll{
	    get { return TextRunnerInfo.Instance.msgScroll; }
    }
    private GameObject _content
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
    }

    public void Update()
    {
        if (_msgScroll.verticalNormalizedPosition >1f && !isReloadingDialogueMute)
        {
            if(isLoadInitDialogueFinished) LoadMoreDialogue();
            isReloadingDialogueMute = true;
            CoroutineManager.DoDelayCertainSeconds(delegate { isReloadingDialogueMute = false; },3f );
        }
    }
    public void Clear()
    {
        //save here
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
			Debug.Log(text);
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
			Services.eventManager.Fire(new TextFinished());
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
	public void CreatePlayerBubble(string text)
	{
		// show text
		AddNewMessage(MessageBubbleType.Player,text,DateTime.Now);
		Services.textSequenceTaskRunner.AddTask(delegate
			{
				RefreshView();
			},DateTime.Now);
		//send the text to text manager as a record
		Services.textManager.FinishedLog.Add(textBox.text);
		Services.textManager.Speaker.Add(1);
		
		// clear the text box
		textBox.text = String.Empty;
		//reset all the keyboard to null
		MuteAllKeyboard();
	}
	
	private void AddNewMessage(MessageBubbleType msgType, string text, DateTime shootTime)
    {
        if ((shootTime - _lastTimeStamp) > TimeSpan.FromMinutes(0.5f))
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
                Services.saveManager.plotMessages.Add(msg);
                
                //create the bubble
                Services.textSequenceTaskRunner.AddTask(delegate
                {
                     GameObject newTimeBox = GameObject.Instantiate(Resources.Load<GameObject>(_playerTextBox), _content.transform);
	                 newTimeBox.GetComponentInChildren<TextMeshProUGUI>().text = text;
                },shootTime);
                break;
            
            case MessageBubbleType.Demon:
                msg= new MessageContent();
                msg.messageType = MessageBubbleType.Demon;
                msg.content=text;
                msg.shootTime = shootTime;
                Services.saveManager.plotMessages.Add(msg);
                
                Services.textSequenceTaskRunner.AddTask(delegate
                {
                    GameObject newTimeBox = GameObject.Instantiate(Resources.Load<GameObject>(_demonTextBox), _content.transform);
                    newTimeBox.GetComponentInChildren<TextMeshProUGUI>().text = text;
                },shootTime);
                break;
        }
    }
	
	private void CreateNewTimeStamp(DateTime time)
	{
		MessageContent msg= new MessageContent();
		msg.messageType = MessageBubbleType.TimeStamp;
		msg.content=String.Empty;
		msg.shootTime = time;
		Services.saveManager.plotMessages.Add(msg);
        
		Services.textSequenceTaskRunner.AddTask(delegate
		{
			GameObject newTimeBox = GameObject.Instantiate(Resources.Load<GameObject>(_timeStampBubble), _content.transform);
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
		GameObject newBox = GameObject.Instantiate(Resources.Load<GameObject>(_typingDotBubble), _content.transform);
		_dot = newBox;
	}

	private void DeleteDot()
	{
		GameObject.Destroy(_dot);
	}

	#endregion
	
	#region Save

	public void Save()
	{
		Services.saveManager.lastTimeStamp = _lastTimeStamp;
    }

    public void Load()
    {
	    if(File.Exists(Services.saveManager.saveJsonPath)){
		    _lastTimeStamp = Services.saveManager.lastTimeStamp;
		    LoadInitialDialogue();
		    LoadMoreDialogue();
		    LoadDialogueForOldPlotWhenAPPisOff();
	    }
	    isLoadInitDialogueFinished = true;
    }
    private void LoadDialogueForOldPlotWhenAPPisOff()
    {
	    var lastMsg = Services.saveManager.FindTheLastMessage();
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

	    if (currentTextPlot.isBreak()) currentTextPlot.ChangePlotState(PlotManager.plotState.isBreak);
	    
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
		    Services.eventManager.Fire(new TextFinished());
	    }
    }

    public void LoadDialogueForNewPlotWhenAPPisOff(DateTime originalStartTime)
    {
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

	    if (currentTextPlot.isBreak()) currentTextPlot.ChangePlotState(PlotManager.plotState.isBreak);
	    
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
		    Services.eventManager.Fire(new TextFinished());
	    }
    }
    private void LoadInitialDialogue()
    {
        var plotMessage = Services.saveManager.plotMessages;
        var dialogueMessage = Services.saveManager.dialogueMessages;

        if(plotMessage.Count!=0) for (int i = plotMessage.Count-1; i>-1; i--)
        {
            var msg = plotMessage[i];
            switch (msg.messageType)
            {
                case MessageBubbleType.Demon:
                    Services.textSequenceTaskRunner.AddTask(delegate
                     {
                         GameObject newTimeBox = GameObject.Instantiate(Resources.Load<GameObject>(_demonTextBox), _content.transform);
                         newTimeBox.GetComponentInChildren<TextMeshProUGUI>().text = msg.content;
                         newTimeBox.transform.SetSiblingIndex(0);
                     }, msg.shootTime);
                    break;
                case MessageBubbleType.Player:
                    Services.textSequenceTaskRunner.AddTask(delegate
                    {
                        GameObject newTimeBox = GameObject.Instantiate(Resources.Load<GameObject>(_playerTextBox), _content.transform);
                        newTimeBox.GetComponentInChildren<TextMeshProUGUI>().text = msg.content;
                        newTimeBox.transform.SetSiblingIndex(0);
                    },msg.shootTime);
                    break;
                case MessageBubbleType.TimeStamp:
                    Services.textSequenceTaskRunner.AddTask(delegate
                    {
                        GameObject newTimeBox = GameObject.Instantiate(Resources.Load<GameObject>(_timeStampBubble), _content.transform);
                        newTimeBox.GetComponentInChildren<TextMeshProUGUI>().text = TimeStamp.GetTimeStamp(msg.shootTime);
                        newTimeBox.transform.SetSiblingIndex(0);
                    },msg.shootTime);
                    break;
            }
            
        }

        _dialogueLabel = dialogueMessage.Count - 1;
    }
    
    private void LoadMoreDialogue()
    {
        var dialogueMessage = Services.saveManager.dialogueMessages;

        if (_dialogueLabel < 0)
        {
            if (!isEndChatShow)
            {
                Services.textSequenceTaskRunner.AddSideTask(delegate
                {
                    GameObject newTimeBox =
                        GameObject.Instantiate(Resources.Load<GameObject>(_endChatBubble), _content.transform);
                    newTimeBox.transform.SetSiblingIndex(0);
                },DateTime.Now);
                isEndChatShow = true;
            }

            return;
        }
        
        MessageContent[] dialogueArray = dialogueMessage[_dialogueLabel];
        for (int i = dialogueArray.Length-1; i>-1; i--)
        {
            var msg = dialogueArray[i];
            switch (msg.messageType)
            {
                case MessageBubbleType.Demon:
                    Services.textSequenceTaskRunner.AddTask(delegate
                    {
                        GameObject newTimeBox = GameObject.Instantiate(Resources.Load<GameObject>(_demonTextBox), _content.transform);
                        newTimeBox.GetComponentInChildren<TextMeshProUGUI>().text = msg.content;
                        newTimeBox.transform.SetSiblingIndex(0);
                    },msg.shootTime);
                    break;
                case MessageBubbleType.Player:
                    Services.textSequenceTaskRunner.AddTask(delegate
                    {
                        GameObject newTimeBox = GameObject.Instantiate(Resources.Load<GameObject>(_playerTextBox), _content.transform);
                        newTimeBox.GetComponentInChildren<TextMeshProUGUI>().text = msg.content;
                        newTimeBox.transform.SetSiblingIndex(0);
                    },msg.shootTime);
                    break;
                case MessageBubbleType.TimeStamp:
                    Services.textSequenceTaskRunner.AddTask(delegate
                    {
                        GameObject newTimeBox = GameObject.Instantiate(Resources.Load<GameObject>(_timeStampBubble), _content.transform);
                        newTimeBox.GetComponentInChildren<TextMeshProUGUI>().text = TimeStamp.GetTimeStamp(msg.shootTime);
                        newTimeBox.transform.SetSiblingIndex(0);
                    },msg.shootTime);
                    break;
            }
            
        }
        _dialogueLabel--;
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
}
