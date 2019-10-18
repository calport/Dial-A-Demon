 using System;
 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeStamp
{
    public static string GetTimeStamp(DateTime time)
    {
        var currentTime = DateTime.Now;
        var timeSpan= currentTime.Date - time.Date;
        if (timeSpan < TimeSpan.FromDays(1))
        {
            return time.ToString("hh:mm tt"); 
        }
        
        if (timeSpan < TimeSpan.FromDays(2))
        {
            return "Yesterday " + time.ToString("hh:mm tt");
        }
         
        if(timeSpan < TimeSpan.FromDays(7))
        {
            return time.DayOfWeek + " " + time.ToString("hh:mm tt");
        }

        return time.ToShortDateString() + " " + time.ToString("hh:mm tt");
    }
    
}
