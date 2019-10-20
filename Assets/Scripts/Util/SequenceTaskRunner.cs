using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;

public class SequenceTaskRunner
{
    public delegate void TaskDelegate();
    
    private List<TaskDelegate> taskSequence = new List<TaskDelegate>();
    private List<DateTime> shootTimes = new List<DateTime>();

    void Init(){}

    // Update is called once per frame
    public void Update()
    {
        Debug.Log(taskSequence.Count);
        if (taskSequence.Count != 0 )
        {
            if (shootTimes[0] < DateTime.Now)
            {
                taskSequence[0].Invoke();
                taskSequence.Remove(taskSequence[0]);
                shootTimes.Remove(shootTimes[0]);
            }
        }
    }

    public void AddTask(TaskDelegate task, DateTime shootTime)
    {
        taskSequence.Add(task);
        shootTimes.Add(shootTime);
    }
}
