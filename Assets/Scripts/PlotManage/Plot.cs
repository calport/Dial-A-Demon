using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;

public abstract class Plot
{
    public float Day;
    public float Hour;
    public float Second;
    
    //This will appear in every subclass so object can set time when they are created
    /*

    public Plot(float day, float hour, float second)
    {
        _day = day;
        _hour = hour;
        _second = second;
    }*/
    
    public virtual void Init(){}
    public virtual void Update(){}
    
    public virtual void Clean(){}
    
    public virtual void RearrangeTime(){}
}

public class TextPlot : Plot
{
    public TextPlot(float day, float hour, float second)
    {
        Day = day;
        Hour = hour;
        Second = second;
    }
    
}
