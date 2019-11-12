using System;
using System.Collections.Generic;

public class PlotFileAddress
{
    private string a = "InkText/story.json";
    //all based on resources
    internal Dictionary<Type, string> fileAddress = new Dictionary<Type, string>
    {
        {typeof(PlotManager.Day1_Text1),"Day1_Contract.json"},
        {typeof(PlotManager.Day2_Text1),"Day2_Morning.json" }, 
        {typeof(PlotManager.Day2_Text2), "Day2_Feeling.json"},
        {typeof(PlotManager.Day3_Text3), "Day3_NeverHaveIEver.json"},
        {typeof(PlotManager.Day4_Text1), "Day4_morning.json"}, 
        {typeof(PlotManager.Day6_Text2), "Day6_Gifting.json"}
        //
    };
}
