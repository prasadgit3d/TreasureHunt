using UnityEngine;
using System.Collections;
using SillyGames.SGBase.TaskSystem;
using System;
using System.Collections.Generic;

namespace SillyGames.SGBase.TaskSystem
{
    public class TaskStartStopAbortTask : Task
    {
        [SerializeField]
        public List<TaskStartStopAbort> m_TaskList = null;

        protected override void OnStart()
        {
            foreach (var task in m_TaskList)
            {
                if (task.IncludeInTask)
                {
                    if (task.Task != null)
                    {
                        switch (task.Action)
                        {
                            case TaskStartStopAbort.TaskAction.None:
                                break;
                            case TaskStartStopAbort.TaskAction.Start:
                                task.Task.Execute();
                                break;
                            case TaskStartStopAbort.TaskAction.Stop:
                                task.Task.Stop();
                                break;
                            case TaskStartStopAbort.TaskAction.Abort:
                                task.Task.Abort();
                                break;
                            default:
                                Debug.Log("Invalid Action");
                                break;
                        }
                    }
                }
            }
            Stop();
        }
    }

    [Serializable]
    public class TaskStartStopAbort
    {
        [SerializeField]
        private Task m_task = null;

        public Task Task
        {
            get
            {
                return m_task;
            }
            set
            {
                m_task = value;
            }
        }

        [SerializeField]
        private bool m_ignoreFromTask = true;

        public bool IncludeInTask
        {
            get
            {
                return !m_ignoreFromTask;
            }
            set
            {
                m_ignoreFromTask = !value;
            }
        }

        public enum TaskAction
        {
            None,
            Start,
            Stop,
            Abort,
        };

        [SerializeField]
        private TaskAction m_action = TaskAction.None;

        public TaskAction Action
        {
            get
            {
                return m_action;
            }
            set
            {
                m_action = value;
            }
        }
    }
}
