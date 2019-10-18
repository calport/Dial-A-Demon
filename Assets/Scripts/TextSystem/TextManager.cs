using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using DialADemon.Library;
using Ink.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;
using Yarn.Unity;
using Yarn.Unity.Example;

public class TextManager
{
    public TextRunner textRunner
    {
        get
        {
            return TextRunner.Instance;
        }
    }
    public Story currentStory
    {
        get { return textRunner.currentStory; }
    }
    // path of the massage bubble prefabs
    private string _demonTextBox= "Prefabs/MessageBubble_Demon";
    public string DemonTextBox
    {
        get { return _demonTextBox; }
    }
    private string _playerTextBox= "Prefabs/MessageBubble_Player";
    public string playerTextBox
    {
        get { return _playerTextBox; }
    }
    private string _demonContract= "Prefabs/DemonContract";
    public string DemonContract
    {
        get { return _demonContract; }
    }
    private string _timeStampBubble = "Prefabs/MessageBubble_TimeStamp";
    private string _endChatBubble = "Prefabs/MessageBubble_EndChat";

    private bool isReloadingDialogueMute = false;
    private bool isLoadInitDialogueFinished = false;
    private bool isEndChatShow;

    //this is a place that record all the dialogue lines, so it will be easier to restore the game
    public List<string> FinishedLog = new List<string>();
    //this is a place to record who says the dialogue in finished log, so it will help restore the game, while 0 is demon and 1 is the player
    public List<int> Speaker = new List<int>();

    private GameObject content
    {
        get { return textRunner.msgScroll.content.gameObject; }
    }
    private DateTime _lastTimeStamp = DateTime.MinValue;
    private int _dialogueLabel = 0;
    
    #region Lifecycle

    public void Init()
    {
        LoadInitialDialogue();
    }

    public void Update()
    {
        if (textRunner.msgScroll.verticalNormalizedPosition == 1f && !isReloadingDialogueMute)
        {
            LoadMoreDialogue();
            isReloadingDialogueMute = true;
            CoroutineManager.DoDelayCertainSeconds(delegate { isReloadingDialogueMute = false; },3f );
        }
    }
    public void Clear()
    {
        //save here
    }

    #endregion

    #region Static functions

    public void Save()
    {
        textRunner.PauseRunner();

        //check if the newest text has been recorded by savemanager, if not, add the newest text too
        var pm = Services.saveManager.plotMessages;
        string lastText = FindTheLastDemonTalk().content;
        var text = currentStory.currentText;
        if (string.Compare(text, lastText) != 0)
        {
            //TODO: if have special waiting time
            AddNewMessage(MessageBubbleType.Demon, currentStory.currentText, 
                pm[pm.Count - 1].shootTime.AddSeconds(3f));
        }
        
        Services.saveManager.currentStory = currentStory;
    }

    public void Load()
    {
        textRunner.currentStory = Services.saveManager.currentStory;
        LoadInitialDialogue();
        LoadMoreDialogue();
        LoadDialogueWhenAPPisOff();
        isLoadInitDialogueFinished = true;
    }
    public void LoadDialogueWhenAPPisOff()
    {
        
        //TODO check the last word whether the time of it is before the loading time
        var demonLastMessage = FindTheLastDemonTalk();
        if (demonLastMessage.shootTime > DateTime.Now)
        {
            CoroutineManager.DoDelayCertainSeconds(delegate
            {
                Services.textSequenceTaskRunner.AddTask(delegate
                {
                    GameObject newTimeBox =
                        GameObject.Instantiate(Resources.Load<GameObject>(_demonTextBox), content.transform);
                    newTimeBox.GetComponentInChildren<TextMeshProUGUI>().text = demonLastMessage.content;
                });
                textRunner.isRunning = true;
                textRunner.ContinueStory();
            }, (float)(demonLastMessage.shootTime - DateTime.Now).TotalSeconds);
        }
        else
        {
            var lastShootTime = demonLastMessage.shootTime;
            while (textRunner.currentStory.canContinue)
            {
                var text = textRunner.currentStory.Continue();
                text = text.Trim();
                //TODO check if the future text has special waiting time
                lastShootTime += TimeSpan.FromSeconds(3);
                if(lastShootTime<DateTime.Now) AddNewMessage(MessageBubbleType.Demon,text,lastShootTime);
                else
                {
                    CoroutineManager.DoDelayCertainSeconds(delegate
                    {
                        Services.textSequenceTaskRunner.AddTask(delegate
                        {
                            GameObject newTimeBox =
                                GameObject.Instantiate(Resources.Load<GameObject>(_demonTextBox), content.transform);
                            newTimeBox.GetComponentInChildren<TextMeshProUGUI>().text = demonLastMessage.content;
                        });
                        textRunner.isRunning = true;
                        textRunner.ContinueStory();
                    }, (float)(demonLastMessage.shootTime - DateTime.Now).TotalSeconds);
                    break;
                }
            }
            textRunner.isRunning = true;
            textRunner.ContinueStory();
            
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
                         GameObject newTimeBox = GameObject.Instantiate(Resources.Load<GameObject>(_demonTextBox), content.transform);
                         newTimeBox.GetComponentInChildren<TextMeshProUGUI>().text = msg.content;
                         newTimeBox.transform.SetSiblingIndex(0);
                     });
                    break;
                case MessageBubbleType.Player:
                    Services.textSequenceTaskRunner.AddTask(delegate
                    {
                        GameObject newTimeBox = GameObject.Instantiate(Resources.Load<GameObject>(_playerTextBox), content.transform);
                        newTimeBox.GetComponentInChildren<TextMeshProUGUI>().text = msg.content;
                        newTimeBox.transform.SetSiblingIndex(0);
                    });
                    break;
                case MessageBubbleType.TimeStamp:
                    Services.textSequenceTaskRunner.AddTask(delegate
                    {
                        GameObject newTimeBox = GameObject.Instantiate(Resources.Load<GameObject>(_timeStampBubble), content.transform);
                        newTimeBox.GetComponentInChildren<TextMeshProUGUI>().text = TimeStamp.GetTimeStamp(msg.shootTime);
                        newTimeBox.transform.SetSiblingIndex(0);
                    });
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
            if (isLoadInitDialogueFinished && !isEndChatShow)
            {
                Services.textSequenceTaskRunner.AddTask(delegate
                {
                    GameObject newTimeBox =
                        GameObject.Instantiate(Resources.Load<GameObject>(_endChatBubble), content.transform);
                    newTimeBox.transform.SetSiblingIndex(0);
                });
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
                        GameObject newTimeBox = GameObject.Instantiate(Resources.Load<GameObject>(_demonTextBox), content.transform);
                        newTimeBox.GetComponentInChildren<TextMeshProUGUI>().text = msg.content;
                        newTimeBox.transform.SetSiblingIndex(0);
                    });
                    break;
                case MessageBubbleType.Player:
                    Services.textSequenceTaskRunner.AddTask(delegate
                    {
                        GameObject newTimeBox = GameObject.Instantiate(Resources.Load<GameObject>(_playerTextBox), content.transform);
                        newTimeBox.GetComponentInChildren<TextMeshProUGUI>().text = msg.content;
                        newTimeBox.transform.SetSiblingIndex(0);
                    });
                    break;
                case MessageBubbleType.TimeStamp:
                    Services.textSequenceTaskRunner.AddTask(delegate
                    {
                        GameObject newTimeBox = GameObject.Instantiate(Resources.Load<GameObject>(_timeStampBubble), content.transform);
                        newTimeBox.GetComponentInChildren<TextMeshProUGUI>().text = TimeStamp.GetTimeStamp(msg.shootTime);
                        newTimeBox.transform.SetSiblingIndex(0);
                    });
                    break;
            }
            
        }
        _dialogueLabel--;
    }

    private MessageContent FindTheLastDemonTalk()
    {
        var pm = Services.saveManager.plotMessages;
        var dm = Services.saveManager.dialogueMessages;
        if (pm.Count == 0 && dm.Count==0)
        {
            return new MessageContent();
        }
        
        string demonMessage;
        if (pm.Count != 0)
        {
            for (int i = pm.Count - 1; i > -1; i--)
            {
                if (pm[i].messageType == MessageBubbleType.Demon) return pm[i];
            }
        } 
        var oldDialogue = dm[dm.Count - 1];
        for (int i = oldDialogue.Length - 1; i > -1; i--)
        {
            if (oldDialogue[i].messageType == MessageBubbleType.Demon) return oldDialogue[i];
        }
        return new MessageContent();
    }
    
    
    public void AddNewMessage(MessageBubbleType msgType, string text, DateTime shootTime)
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
                     GameObject newTimeBox = GameObject.Instantiate(Resources.Load<GameObject>(_playerTextBox), content.transform);
                                    newTimeBox.GetComponentInChildren<TextMeshProUGUI>().text = text;
                });
                break;
            
            case MessageBubbleType.Demon:
                msg= new MessageContent();
                msg.messageType = MessageBubbleType.Demon;
                msg.content=text;
                msg.shootTime = shootTime;
                Services.saveManager.plotMessages.Add(msg);
                
                Services.textSequenceTaskRunner.AddTask(delegate
                {
                    GameObject newTimeBox = GameObject.Instantiate(Resources.Load<GameObject>(_demonTextBox), content.transform);
                    newTimeBox.GetComponentInChildren<TextMeshProUGUI>().text = text;
                });
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
            GameObject newTimeBox = GameObject.Instantiate(Resources.Load<GameObject>(_timeStampBubble), content.transform);
            newTimeBox.GetComponentInChildren<TextMeshProUGUI>().text = TimeStamp.GetTimeStamp(time);
        });
    }
    
    public void StartNewStory(TextAsset ta)
         {
             textRunner.StartNewStory(ta);
         }
    
    #endregion
   
    
}
