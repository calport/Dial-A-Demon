using System;
using System.Collections.Generic;
using Ink.Runtime;
using UnityEditor;
using UnityEngine;

public static class PlotFileAddress
{
    private static string a = "InkText/story.json";
    //all based on resources
    internal static Dictionary<Type, string> inkFileAddress = new Dictionary<Type, string>
    {
        {typeof(PlotManager.Day1_Text1), "Day1_Contract.json"},
        {typeof(PlotManager.Day2_Text1), "Day2_Morning.json"}, 
        {typeof(PlotManager.Day2_Text2), "Day2_Feeling.json"},
        {typeof(PlotManager.Day3_Text1), "Day3_morning.json"},
        {typeof(PlotManager.Day3_Text3), "Day3_NeverHaveIEver.json"},
        {typeof(PlotManager.Day4_Text1), "Day4_morning.json"}, 
        {typeof(PlotManager.Day6_Text2), "Day6_Gifting.json"}
        //
    };
    internal static Dictionary<Type, string> textBubblePrefabAddress = new Dictionary<Type, string>();
    internal static Dictionary<Type, string> textDocumentsAddress = new Dictionary<Type, string>();
    
    public static Story GetStory(Type plotType)
    {
        string textAssetLocation;
        inkFileAddress.TryGetValue(plotType, out textAssetLocation);
        var ta = SerializeManager.ReadJsonString(Application.dataPath + "/Resources/InkText/" + textAssetLocation);
        //var ta = Resources.Load<TextAsset>(textAssetLocation);
        return new Story(ta);
    }
    
    public static GameObject GetBubble(Type plotType)
    {
        string textAssetLocation;
        textBubblePrefabAddress.TryGetValue(plotType, out textAssetLocation);
        var bubble = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Resources/Prefabs/" + textAssetLocation);
        return bubble;
    }
    
    public static GameObject GetDocument(Type plotType)
    {
        string textAssetLocation;
        textDocumentsAddress.TryGetValue(plotType, out textAssetLocation);
        var file = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Resources/Prefabs/" + textAssetLocation);
        return file;
    }
}
