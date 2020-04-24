using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class TextFile : Tag,IGameContributeTag
{
    public string fileBubble;
    public string fileContent;
    public void SetVariables()
    { 
        Debug.Log(attribute.Count);
        fileBubble = attribute["fileBubbleName"];
        fileContent = attribute["fileContentName"];
    }
}
