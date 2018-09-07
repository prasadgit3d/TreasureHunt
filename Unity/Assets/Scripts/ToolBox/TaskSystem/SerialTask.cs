using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine.Events;

namespace SillyGames.SGBase.TaskSystem
{
    public class SerialTask : Task
    {
        [SerializeField]
        private List<Task> m_TaskList = null;

        private Task currentTask = null;

        private List<Task> internalTaskList = new List<Task>();

        public Task CurrentTask
        {
            get
            {
                return currentTask;
            }

            private set
            {
                currentTask = value;
            }
        }

        protected override void OnPause()
        {
            if (CurrentTask != null)
            {
                CurrentTask.IsPaused = true;
            }
            base.OnPause();
        }

        protected override void OnResume()
        {
            if (CurrentTask != null)
            {
                CurrentTask.IsPaused = false;
            }
            base.OnResume();
        }

        protected override void OnStart()
        {
            IsMarkedForStop = false;
            internalTaskList.Clear();
            foreach (Task task in m_TaskList)
            {
                task.EndEvent.RemoveListener(OnInternalTaskEnded);
                task.EndEvent.AddListener(OnInternalTaskEnded);
                internalTaskList.Add(task);
            }
            ExecuteFirstTask();
            base.OnStart();
        }

        private void ExecuteFirstTask()
        {
            CurrentTask = internalTaskList[0];
            CurrentTask.Execute();
        }

        private bool IsMarkedForStop
        {
            get;
            set;
        }

        protected override void OnEnd()
        {
            if (CurrentTask != null)
            {
                IsMarkedForStop = true;
                CurrentTask.Stop();
                CurrentTask = null; 
            }
            base.OnEnd();
        }

        protected override void OnAbort()
        {
            if (CurrentTask != null)
            {
                CurrentTask.Abort();
            }
            base.OnAbort();
        }

        private void OnInternalTaskEnded()
        {
            if (IsMarkedForStop)
            {
                return;
            }

            internalTaskList.RemoveAt(0);
            if (internalTaskList.Count == 0)
            {
                Stop();
            }
            else
            {
                CurrentTask = internalTaskList[0];
                CurrentTask.Execute();
            }
        }
    }
}