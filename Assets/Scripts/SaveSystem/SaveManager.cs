using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SimpleJSON;
using UnityEngine;

public class SaveManager
{
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

    public string inkJson;

    public void Init()
    {
        //TODO
        //read all the saved files
    }

    public void Clear()
    {
    }

    public void LoadGame()
    {
        if (File.Exists(_saveJsonPath))
        {
            //read the file
            var json = SerializeManager.ReadJsonString(_saveJsonPath);
            var jsonObject = JSON.Parse(json); 
            inkJson = SerializeManager.ReadJsonString(Services.saveManager.inkjsonPath);
            
            //Load info from save
            //plot should run before text, since text manager needs its info
            Services.plotManager.LoadFromFile(jsonObject);
            Services.textManager.LoadFromFile(jsonObject);
            Services.phoneManager.Load(jsonObject);
            Services.plotManager.UpdatePlayerOffTime();
            Services.textManager.UpdatePlayerOffTime();


            //TODO: maybe pause the game and then at here unpause it
            Debug.Log("Game Loaded");
        }
        else
            Debug.Log("No game saved!");
    }

    public void SaveGame()
    {
        var jsonObject = new JSONObject();
        jsonObject = Services.textManager.Save(jsonObject);
        jsonObject = Services.plotManager.Save(jsonObject);
        jsonObject = Services.phoneManager.Save(jsonObject);
        File.WriteAllText(_saveJsonPath, jsonObject.ToString(), Encoding.UTF8);

    }
}

