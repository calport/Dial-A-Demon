using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.iOS;

public class PlotManager
{
    //Calendar system
    public List<Plot> TotalTask { get; private set; }
    private DateTime _startTime;

    public DateTime StartTime
    {
        get { return _startTime; }
    }

    private int _plotNumber = 0;
    public int PlotNumber
    {
        get { return _plotNumber; }
    }

    public void RegisterNewPlot<T>(Plot newPlot) where T : Plot
    {
        TotalTask.Add(newPlot);        
    }

    private void ArrangeCalendar()
    {
        TotalTask = TotalTask.OrderBy(o => o.PlotStartTime.Day).ThenBy(o => o.PlotStartTime.Hour).ThenBy(o => o.PlotStartTime.Minute).ToList();
    }
    

    //The actual functioning system which is in charge of the life circle of plots
    private List<Plot> _pendingPlot = new List<Plot>();
    private List<Plot> _deletedPlot =new List<Plot>();
    private List<Plot> _newPlot = new List<Plot>();
    private List<Plot> _currentPlot = new List<Plot>();

    public List<Plot> DeletedPlot
    {
        get { return _deletedPlot; }
    }

    public List<Plot> CurrentPlot
    {
        get { return _currentPlot; }
    }
    
    public void AddPlot(Plot newPlot)
    {
        _pendingPlot.Add(newPlot);
        //When a plot has been put in pending,then the time of the plot is set and will never been changed in the future
        //TotalTask.Remove(newPlot);
    }

    public void DeletePlot(Plot newPlot)
    {
        _deletedPlot.Add(newPlot);
        _currentPlot.Remove(newPlot);
        newPlot.Clean();
    }

    //initiate everything when the game first start
    //recover everything when the game is logged in
    public void Init()
    {

        //clear notification
        UnityEngine.iOS.NotificationServices.ClearLocalNotifications();

        //register all the plots
        RegisterPlot.RegisterAllPlot();
        
        //initialize when the first time login
        if (!PlayerPrefs.HasKey("Plot"))
        {
            PlayerPrefs.SetInt("Plot", _plotNumber);
            _startTime = System.DateTime.Now;
            foreach (var plot in TotalTask)
            {
                plot.InitTime();
            }
            //not really need this if the order was set in the beginning, but if the register becomes dynamic, the system should be careful with the insert plot
            //because the aiming time is not updated all the time, and there is no use of the arrange calendar function
            //ArrangeCalendar();
        }
        else
        {

            _plotNumber = PlayerPrefs.GetInt("Plot");
        }

        //read the save plot
        //AddPlot(TotalTask[_plotNumber]);
        //read the saved plot start time
        //TODO
    }

    //keep tracking how each plot works in different stage
    public void RegularUpdate()
    {
        //running the plots
        //running plots should be first in case that deleteplot func delete things that was initiated by new plot
        foreach (var plot in _currentPlot)
        {
            plot.Update();
            if (plot.IsFinished())
            {
                DeletePlot(plot);
            }
        }

        //keep rearrange the plot start time
        //if the situation and reached and the time can be settled, then move this plot to new plot
        foreach (var pendingPlot in _pendingPlot)
        {
            if (pendingPlot.IsAbleToSetTimeDown())
            {
                pendingPlot.RearrangeTime();
                _newPlot.Add(pendingPlot);
                _pendingPlot.Remove(pendingPlot);
            }
        }

        //keep check the time of initiation may need future change
        //if the time has reached, then put the pending plot into the current plot
        foreach (var newPlot in _newPlot)
        {
            if (newPlot.IsAbleToStart())
            {
                _newPlot.Remove(newPlot);
                _currentPlot.Add(newPlot);
                _plotNumber++;
                newPlot.Init();
                newPlot.AddPendingPlot();
            }
        }
    }

    public void SpeedUpdate()
    {
        //running the plots
        //running plots should be first in case that deleteplot func delete things that was initiated by new plot
        foreach (var plot in _currentPlot)
        {
            plot.Update();
            if (plot.IsFinished())
            {
                DeletePlot(plot);
            }
        }

        //keep rearrange the plot start time
        //if the situation and reached and the time can be settled, then move this plot to new plot
        foreach (var pendingPlot in _pendingPlot)
        {
            if (pendingPlot.IsAbleToSetTimeDown())
            {
                pendingPlot.ShrinkTime();
                _newPlot.Add(pendingPlot);
                _pendingPlot.Remove(pendingPlot);
            }
        }
        
        //keep check the time of initiation may need future change
        //if the time has reached, then put the pending plot into the current plot
        foreach (var newPlot in _newPlot)
        {
            if (newPlot.IsAbleToStart())
            {
                _newPlot.Remove(newPlot);
                _currentPlot.Add(newPlot);
                _plotNumber++;
                newPlot.Init();
                newPlot.AddPendingPlot();
            }
    }
        
        /*
        //do the cleaning when a plot is finished
        foreach (var deletePlot in _deletePlot)
        {
            _currentPlot.Remove(deletePlot);
            //rearrange all the task times after one of the plot is finished
        }*/
        
    }

    
    //when the app quit, save all the info that needed
    //add notification and other stuff to keep track the gameflow
    public void Clear()
    {
        //save all the list of plots
        //save all the plot start time
        //TODO
        //add notification
        foreach (var newPlot in _newPlot)
        {
            newPlot.NewPlotNote();
        }

        foreach (var currentPlot in _currentPlot)
        {
            currentPlot.CurrentPlotNote();
        }
    }
}
