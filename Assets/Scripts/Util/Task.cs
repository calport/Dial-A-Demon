using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Task
{

    public enum TaskStatus : byte
    {
        Pending, // Task has not been initialized
        Working, // Task has been initialized
        Success, // Task completed successfully
        Failed, // Task completed unsuccessfully
        Aborted // Task was aborted
    }

// The only member variable that a base task has is its status
    private TaskStatus _status = TaskStatus.Pending;

    public TaskStatus Status
    {
        get { return _status; }
        set { _status = value; }
    }

// Convenience status checking
    public bool IsPending
    {
        get { return Status == TaskStatus.Pending; }
    }

    public bool IsWorking
    {
        get { return Status == TaskStatus.Working; }
    }

    public bool IsSuccessful
    {
        get { return Status == TaskStatus.Success; }
    }

    public bool IsFailed
    {
        get { return Status == TaskStatus.Failed; }
    }

    public bool IsAborted
    {
        get { return Status == TaskStatus.Aborted; }
    }

    public bool IsFinished
    {
        get { return (Status == TaskStatus.Failed || Status == TaskStatus.Success || Status == TaskStatus.Aborted); }
    }

// Convenience method for external classes to abort the task
    public void Abort()
    {
        SetStatus(TaskStatus.Aborted);
    }

// A method for changing the status of the task
// It's marked internal so that the runner can access it
// assuming tasks and their manager are in the same assembly
    internal void SetStatus(TaskStatus newStatus)
    {
        if (Status == newStatus) return;

        Status = newStatus;

        switch (newStatus)
        {
            case TaskStatus.Working:
                Init();
                break;

            case TaskStatus.Success:
                OnSuccess();
                CleanUp();
                break;

            case TaskStatus.Aborted:
                OnAbort();
                CleanUp();
                break;

            case TaskStatus.Failed:
                OnFail();
                CleanUp();
                break;

            case TaskStatus.Pending:
                break;

            default:
                break;
                //throw new ArgumentOutOfRangeException(newStatus.ToString(), newStatus, null);
        }
    }


// Subclasses can override these to respond to status changes
    protected virtual void OnAbort()
    {
    }

    protected virtual void OnSuccess()
    {
    }

    protected virtual void OnFail()
    {
    }


// Override this to handle initialization of the task.
// This is called when the task enters the Working state
    protected virtual void Init()
    {
    }

// Called whenever the TaskRunner updates. Your tasks' work
// generally goes here
    internal virtual void Update()
    {
    }

// This is called when the tasks completes (i.e. is aborted,
// fails, or succeeds). It is called after the status change
// handlers are called
    protected virtual void CleanUp()
    {
    }
}
