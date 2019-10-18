using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;

public class SequenceTaskRunner
{
    internal class SequenceTaskRunnerMonoBehavior : MonoBehaviour{}
    public delegate void TaskDelegate();
    
    private List<TaskDelegate> taskSequence = new List<TaskDelegate>();

    void Init(){}

    // Update is called once per frame
    public void Update()
    {
        if (taskSequence.Count != 0)
        {
            taskSequence[0].Invoke();
            taskSequence.Remove(taskSequence[0]);
        }
    }

    public void AddTask(TaskDelegate task)
    {
        taskSequence.Add(task);
    }
}
