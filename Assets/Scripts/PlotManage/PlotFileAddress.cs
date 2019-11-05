using System;
using System.Collections.Generic;

public class PlotFileAddress
{
    private string a = "InkText/story.json";
    //all based on resources
    internal Dictionary<Type, string> fileAddress = new Dictionary<Type, string>
    {
        {typeof(PlotManager.Day1_Text1),"Day4_Morning.json"},
        {typeof(PlotManager.Day1_Text2),"story.json" }
    };
}
