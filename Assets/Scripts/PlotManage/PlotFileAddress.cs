using System;
using System.Collections.Generic;

public class PlotFileAddress
{
    private string a = "InkText/story.json";
    //all based on resources
    internal Dictionary<Type, string> fileAddress = new Dictionary<Type, string>
    {
        {typeof(PlotManager.Day1_Text1), "Day1_Contract.ink.json"},
        {typeof(PlotManager.Day2_Text1), "Day2_Morning.ink.json"}, 
        {typeof(PlotManager.Day2_Text2), "Day2_Feeling.ink.json"},
        {typeof(PlotManager.Day3_Text3), "Day3_NeverHaveIEver.ink.json"},
        {typeof(PlotManager.Day4_Text1), "Day4_morning.ink.json"}, 
        {typeof(PlotManager.Day6_Text2), "Day6_Gifting.ink.json"}
        //
    };
}
