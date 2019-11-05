using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TaskRunner : Task
    {
        // The tasks this runner is managing
        protected readonly List<Task> Tasks = new List<Task>();

        // To avoid problems with adding and deleting tasks
        // to our list while we are iterating them we are
        // going to keep track of all the additions/deletions
        // and perform them all after we are done with the
        // processing the tasks.
        private readonly List<Task> _pendingAdd = new List<Task>();
        private readonly List<Task> _pendingRemove = new List<Task>();

        // A convenience method to know when there's work left to be done
        public bool HasTasks {get { return Tasks.Count > 0; }}

        // This is where we schedule tasks to be run...
        public void Add(Task task)
        {
            // First we set the runner's status to working since now
            // it has something to do (in case it wasn't already)
            SetStatus(TaskStatus.Working);
            // Just in case this task has it's status set to something
            // other than pending (i.e. about to do work) we enforce it
            // here.
            task.SetStatus(TaskStatus.Pending);
            // and we add the task to the list of tasks to be sheduled
            // (just in case this is being called in the middle of a loop)
            _pendingAdd.Add(task);
        }

        // This is where we handle removing tasks when we discover it's
        // finished while updating.

        // P.S. - Notice that this is private  and there's no
        // method for Remove(Task task). This is because if you
        // want to remove a task you should call abort on the task.
        // This makes it much easier for the task runner than if it
        // had to handle tasks arbitrarily being yanked out of it's
        // list at random times.
        private void HandleCompletion(Task task)
        {
            // Set it aside to be removed...
            _pendingRemove.Add(task);
            // reset it to pending so it can be reused
            // (in theory - in practice it's fussy to reuse tasks
            // with this implementation)...
            task.SetStatus(TaskStatus.Pending);
            // If we completed all tasks then this runner is done
            // (for now at least)
            if (Tasks.Count == 0)
            {
                SetStatus(TaskStatus.Success);
            }
        }

        // This is where we do the actual work of moving individual
        // tasks through their lifecycle.
        protected void ProcessTask(Task task)
        {
            // If the task is waiting to start...
            if (task.IsPending)
            {
                // Start working...
                task.SetStatus(TaskStatus.Working);
            }

            // If the task is aborted, failed or succeeded...
            if (task.IsFinished)
            {
                // Get rid of it...
                HandleCompletion(task);
            }
            else
            {
                // Otherwise update it...
                task.Update();
                // Here we check if it's done and get rid of it immediately
                // technically we could wait until the next time the task is
                // processed, but I'm impatient (and want to keep runners as
                // empty as I can)
                if (task.IsFinished)
                {
                    HandleCompletion(task);
                }
            }
        }

        // After subclasses update we need to clean up and deal with
        // any pending added/removed tasks
        // Subclasses of TaskRunner **NEED** to call this after update
        protected void PostUpdate()
        {
            // First we remove all the ones waiting to be removed...
            foreach (var task in _pendingRemove)
            {
                Tasks.Remove(task);
            }
            _pendingRemove.Clear();

            // Then we add the ones waiting to be added
            foreach (var task in _pendingAdd)
            {
                Tasks.Add(task);
            }
            _pendingAdd.Clear();

            // if there are no more tasks pending then the task is complete
            if (!HasTasks)
            {
                SetStatus(TaskStatus.Success);
            }

        }

        public void Clear()
        {
            foreach (var t in Tasks)
            {
                t.Abort();
            }
        }

        protected override void OnAbort()
        {
            foreach (var task in Tasks)
            {
                task.Abort();
            }
        }

        // Sometimes we may want to find out if there's
        // specific kinds of tasks pending or being
        // worked on, and these two methods help us with that.
        // For example:
        // if (!runner.HasTask<EndLevel>()) runner.Add(new EndLevel());
        public T GetTask<T>() where T : Task
        {
            foreach (var task in Tasks)
            {
                if (task.GetType() == typeof(T)) return (T)task;
            }
            return null;
        }

        public bool HasTask<T>() where T : Task
        {
            return GetTask<T>() != null;
        }
    }

// This is the "classic" form of the task runner: updating the tasks one at
// a time and only moving on to the next one when the previous one is done
public class SerialTasks : TaskRunner
{
    internal override void Update()
    {
        PostUpdate();
        // Only update the first task
        if (HasTasks)
        {
            var first = Tasks[0];
            ProcessTask(first);
            
        }
    }
}

// Sometimes you might want to run a bunch of tasks in parallel.
// This is most often useful as a way to combine a bunch of different
// sequences and might be your "singleton" task runner, that handles
// running all the others.
public class ParallelTasks : TaskRunner
{
    internal override void Update()
    {
        // Update all the tasks
        foreach(var task in Tasks)
        {
            ProcessTask(task);
        }
        PostUpdate();
    }
}
