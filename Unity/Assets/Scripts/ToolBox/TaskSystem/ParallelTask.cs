using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SillyGames.SGBase.TaskSystem
{
    public class ParallelTask : Task
    {
        [SerializeField]
        private List<Task> m_TaskList = null;

        private List<Task> activeTasks = new List<Task>();

        private List<Task> activeTasksInternal = new List<Task>();
        public bool IsMarkedForStop
        {
            get;
            set;
        }

        protected override void OnStart()
        {
            IsMarkedForStop = false;
            activeTasks.Clear();
            activeTasksInternal.Clear();
            foreach (Task task in m_TaskList)
            {
                task.EndEvent.RemoveListener(OnInternalTaskEnded);
                task.EndEvent.AddListener(OnInternalTaskEnded);

                if(!activeTasks.Contains(task)) 
                {
                    activeTasks.Add(task);
                    activeTasksInternal.Add(task);
                }
                else
                {
                    Debug.LogWarningFormat("Task: {0} is already added to the active tasks list. Will not be added again.", task.ToString());
                }
            }
            ExecuteAllTasks();
            base.OnStart();
        }

        protected override void OnEnd()
        {
            IsMarkedForStop = true;
            foreach (Task task in activeTasks)
            {
                task.Stop();
            }
            base.OnEnd();
        }

        protected override void OnPause()
        {
            if (activeTasks.Count > 0)
            {
                foreach (Task task in activeTasks)
                {
                    task.IsPaused = true;
                }
            }
            base.OnPause();
        }

        protected override void OnResume()
        {
            if (activeTasks.Count > 0)
            {
                foreach (Task task in activeTasks)
                {
                    task.IsPaused = false;
                }
            }
            base.OnResume();
        }

        protected override void OnAbort()
        {
            if (activeTasks.Count > 0)
            {
                foreach (Task task in activeTasks)
                {
                    task.Abort();
                }
            }
            base.OnAbort();
        }

        private void OnInternalTaskEnded()
        {
            if (IsMarkedForStop)
            {
                return;
            }

            foreach (var task in activeTasks)
            {
                if (!task.IsPlaying)
                {
                    activeTasks.Remove(task);
                    break;
                }
            }

            if (activeTasks.Count == 0)
            {
                Stop();
            }
        }

        private void ExecuteAllTasks()
        {
            foreach (Task task in activeTasksInternal)
            {
                task.Execute();
            }
        }
    }
}
