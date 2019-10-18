using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Antlr4.Runtime;
using Ink.Runtime;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;
using UnityEngine.Serialization;

public enum MessageBubbleType
{
    Demon,
    Player,
    TimeStamp
}

[Serializable]
public struct MessageContent
{
    public MessageBubbleType messageType;
    public string content;
    public DateTime shootTime;
}

[Serializable]
public struct PlotInfo
{
    public bool isOnCalendar;
    public Type plotType;
    public PlotManager.plotState plotState;
    public TimeSpan relaSpan;
    public DateTime startTime;
}
public class SaveManager
{
    [System.Serializable]
    internal class Save
    {
        public List<int> dialogueLengthInfo;
        public List<MessageContent> dialogueMessages;
        public bool isLastDialogueFinished;
        public List<PlotInfo> plotInfo;
        public Story currentStory;

    }
    
    private readonly string _saveJsonPath = Application.persistentDataPath + "/save.save";
    private readonly string _inkjsonPath = Application.persistentDataPath + "/ink.save";

    public List<MessageContent> plotMessages = new List<MessageContent>();
    private List<MessageContent[]> _dialogueMessages = new List<MessageContent[]>();
    public List<PlotInfo> plotInfo = new List<PlotInfo>();
    public Story currentStory = null;
    

    public List<MessageContent[]> dialogueMessages
    {
        get { return _dialogueMessages; }
    }

    public void Init()
    {
        //TODO
        //read all the saved files
        Services.eventManager.AddHandler<TextFinished>(delegate{OnTextFinished();});
    }

    public void Clear()
    {
        Services.eventManager.RemoveHandler<TextFinished>(delegate{OnTextFinished();});
    }

    private void OnTextFinished()
    {
        //pack the msg and put them into the big list for saving
        var msgArray =plotMessages.ToArray();
        _dialogueMessages.Add(msgArray);
        plotMessages = new List<MessageContent>();
    }

    public void LoadGame()
    {
        if (File.Exists(_saveJsonPath))
        {
            //read the file
            Save save = SerializeManager.ReadFromJson<Save>(_saveJsonPath);
            
            //Load info from save
            LoadTextSystem(save);
            plotInfo = save.plotInfo;


            //TODO: maybe pause the game and then at here unpause it
            Debug.Log("Game Loaded");
        }
        else
        {
            Debug.Log("No game saved!");
        }
    }

    public void SaveGame()
    {
        
        Save save = new Save();
        
        SaveTextSystem(save);
        save.plotInfo = plotInfo;
        SerializeManager.SaveToJson(_saveJsonPath,save);
        
        

    }
   
    private void LoadTextSystem(Save save)
    {
        MessageContent[][] arrayInfo= SerializeManager.Deserialize2DArray(save.dialogueLengthInfo, save.dialogueMessages);
        //get the info from the save file
        foreach (var msgArray in arrayInfo)
        {
            _dialogueMessages.Add(msgArray);
        }
        
        if(!save.isLastDialogueFinished) 
        {
            if (_dialogueMessages.Count > 0)
            {
                var leftDialogue = _dialogueMessages[_dialogueMessages.Count - 1];
                plotMessages = leftDialogue.OfType<MessageContent>().ToList();
                _dialogueMessages.Remove(_dialogueMessages[_dialogueMessages.Count - 1]);
            }
        }

        currentStory = save.currentStory;
        var inkJson = SerializeManager.ReadJsonString(_inkjsonPath);
        currentStory.state.LoadJson(inkJson);
    }

    private void SaveTextSystem(Save save)
    {
        if (plotMessages.Count != 0)
        {
            OnTextFinished();
            save.isLastDialogueFinished = false;
        }
        else
        {
            save.isLastDialogueFinished = true;
        }
        var infoDic = SerializeManager.Serialize2DArray(_dialogueMessages.ToArray());
        foreach (var pair in infoDic)
        {
            save.dialogueLengthInfo =pair.Key;
            save.dialogueMessages = pair.Value;
        }

        save.currentStory = currentStory;
        SerializeManager.SaveJson(_inkjsonPath, currentStory.state.ToJson());
    }
    
}

