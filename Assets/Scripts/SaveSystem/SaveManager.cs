using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Antlr4.Runtime;
using Ink.Runtime;
using Newtonsoft.Json;
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
public class MessageContent
{
    public MessageBubbleType messageType;
    public string content;
    public DateTime shootTime;
}

[Serializable]
public class PlotInfo
{
    public bool isOnCalendar;
    public Type plotType;
    public PlotManager.plotState plotState;
    public TimeSpan relaSpan;
    public DateTime startTime;
    //for phone plot
    public PlotManager.PhonePlot.PhoneCallState phoneCallState;
}
public class SaveManager
{
    [System.Serializable]
    internal class Save
    {
        public List<int> dialogueLengthInfo;
        public List<MessageContent> dialogueMessages;
        public bool isLastDialogueFinished;
        public DateTime plotBaseTime;
        public List<PlotInfo> plotInfo;
        public DateTime lastTimeStamp;

    }
    
    private readonly string _saveJsonPath = Application.persistentDataPath + "/save.save";
    public string saveJsonPath
    {
        get { return _saveJsonPath; }
    }
    private readonly string _inkjsonPath = Application.persistentDataPath + "/ink.save";
    public string inkjsonPath
    {
        get { return _inkjsonPath; }
    }

    public List<MessageContent> plotMessages = new List<MessageContent>();
    private List<MessageContent[]> _dialogueMessages = new List<MessageContent[]>();
    public List<MessageContent[]> dialogueMessages
    {
        get { return _dialogueMessages; }
    }

    public DateTime plotBaseTime;
    public List<PlotInfo> plotInfo = new List<PlotInfo>();
    public Story currentStory;
    public DateTime lastTimeStamp;
    public string inkJson;
    

    

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
            LoadPlotSystem(save);


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
        SavePlotSystem(save);
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
        
        inkJson = SerializeManager.ReadJsonString(_inkjsonPath);
        Services.textManager.inkJson = inkJson;
        lastTimeStamp = save.lastTimeStamp;
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

        if(currentStory!= null) SerializeManager.SaveJson(_inkjsonPath, currentStory.state.ToJson());
        else SerializeManager.SaveJson(_inkjsonPath, "");
        
        save.lastTimeStamp = lastTimeStamp;
    }

    private void LoadPlotSystem(Save save)
    {
        plotBaseTime = save.plotBaseTime; 
        plotInfo = save.plotInfo;
    }

    private void SavePlotSystem(Save save)
    {
        save.plotBaseTime = plotBaseTime;
        save.plotInfo = plotInfo;
    }
    
    public MessageContent FindTheLastMessage()
    {
        var pm = Services.saveManager.plotMessages;
        var dm = Services.saveManager.dialogueMessages;

        if (pm.Count == 0 && dm.Count==0) return new MessageContent();
        if (pm.Count != 0) return pm[pm.Count - 1];
        
        var oldDialogue = dm[dm.Count - 1];
        if (oldDialogue.Length != 0) return oldDialogue[oldDialogue.Length - 1];
        return new MessageContent();
    }
}

