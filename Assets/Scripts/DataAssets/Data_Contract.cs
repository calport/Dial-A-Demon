using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Data_Contract : Data
{
    public Dictionary<string, bool> DataBool = new Dictionary<string, bool>{
        {"Notification", false},
        {"Mail",false},
        {"Camera",false},
        {"Album",false},
        {"Location",false},
        {"YourPhone",false},
        {"YourLove",false},
        {"Yourself",false}        
    };

    public override void ResetContract()
    {

    }

    public void ChangeData(string dataName, bool data)
    {
        DataBool.Remove(dataName);
        DataBool.Add(dataName,data);
    }
}
