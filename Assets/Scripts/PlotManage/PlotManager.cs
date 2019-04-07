using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class PlotManager : MonoBehaviour
{
    //Calendar system
    public List<Plot> TotalTask { get; private set; }
    
    public void RegisterPlot<T>(Plot newPlot) where T : Plot
    {
        TotalTask.Add(newPlot);
        ArrangeCalendar();
    }

    private void ArrangeCalendar()
    {
        TotalTask.OrderBy(o => o.Day).ThenBy(o => o.Hour).ThenBy(o => o.Second);
    }
    
    
    //The actual functioning system which is in charge of the life circle of plots
    private List<Plot> _pendingPlot;
    private List<Plot> _deletePlot;
    public List<Plot> CurrentPlot { get; private set; }
    
    public void AddPlot(Plot newPlot)
    {
        _pendingPlot.Add(newPlot);
        //When a plot has been put in pending,then the time of the plot is set and will never been changed in the future
        TotalTask.Remove(newPlot);
    }

    public void DeletePlot(Plot newPlot)
    {
        _deletePlot.Add(newPlot);
    }

    // Update is called once per frame
    void Update()
    {
        //clear the pending plot list and put them all in current plot so they can start working
        //Should keep check the time of initiation may need future change
        //if the time has reached, then put the pending plot into the current plot
        //TODO
        foreach (var pendingPlot in _pendingPlot)
        {
            CurrentPlot.Add(pendingPlot);
            pendingPlot.Init();
            _pendingPlot.Remove(pendingPlot);
            
        }

        //delete all the plots that are waiting in the deletePlot list
        foreach (var deletePlot in _deletePlot)
        {
            CurrentPlot.Remove(deletePlot);
            deletePlot.Clean();
            //rearrange all the task times after one of the plot is finished
            foreach (var plot in TotalTask)
            {
                plot.RearrangeTime();
            }
        }
        
        //update all the plot that is running right now
        foreach (var plot in CurrentPlot)
        {
            plot.Update();
        }
    }
}
