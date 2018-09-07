using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;
using System;

namespace SillyGames.SGBase.TaskSystem
{
    /// <summary>
    /// Base class for all tasks in the task system.
    /// </summary>
    public class Task : MonoBehaviour
    {
        [SerializeField]
        private string m_TaskName = string.Empty;

        /// <summary>
        /// Name of the specific task. Makes it easier for management when handling a lot of tasks.
        /// </summary>
        public string TaskName
        {
            get
            {
                return m_TaskName;
            }
            set
            {
                m_TaskName = value;
            }
        }

        [SerializeField]
        private bool executeOnStart = false;

        /// <summary>
        /// Bool to set the task to execute on application start.
        /// </summary>
        public bool ExecuteOnStart
        {
            get
            {
                return executeOnStart;
            }
            set
            {
                executeOnStart = value;
            }
        }

        private bool runUpdate = false;

        private bool isPaused = false;

        [SerializeField]
        private UnityEvent OnStartEvent = null;

        [SerializeField]
        private UnityEvent OnUpdateEvent = null;

        [SerializeField]
        private UnityEvent OnEndEvent = null;

        /// <summary>
        /// Unity event invoked when any task is started.
        /// </summary>
        public UnityEvent StartEvent
        {
            get
            {
                return OnStartEvent;
            }
        }
        
        /// <summary>
        /// Unity event invoked when update of any task is called.
        /// </summary>
        public UnityEvent UpdateEvent
        {
            get
            {
                return OnUpdateEvent;
            }
        }

        /// <summary>
        /// Unity event invoked when any task ends.
        /// </summary>
        public UnityEvent EndEvent
        {
            get
            {
                return OnEndEvent;
            }
        }

        /// <summary>
        /// Bool property to get the pause state of the task.
        /// Also used to set pause or resume state of any task.
        /// </summary>
        public bool IsPaused
        {
            get
            {
                return isPaused;
            }

            set
            {
                if (isPaused != value)
                {
                    isPaused = value;
                    if (IsPlaying)
                    {
                        if (value)
                        {
                            OnPause();
                        }
                        else 
                        {
                            OnResume();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Bool property to get the running state of the task. Can be set only privately.
        /// Sets/returns a bool which decides if the task's update should be executed or not.
        /// </summary>
        public bool IsPlaying
        {
            get
            {
                return runUpdate;
            }

            private set { runUpdate = value; }
        }

        /// <summary>
        /// It is protected so that a compile time warning is shown to any derived class trying to implement start method as it is a mono behavior.
        /// </summary>
        protected virtual void Start()
        {
            if (executeOnStart)
            {
                Execute();
            }
        }

        /// <summary>
        /// Method to be called to start any task.
        /// Nothing happens if the task is already running.
        /// </summary>
        public void Execute()
        {
            if (!IsPlaying)
            {
                IsPlaying = true;
                OnStart();
                OnStartEvent.Invoke();
            }
            if (IsPaused)
            {
                OnPause();
            }
        }

        /// <summary>
        /// Method to be called to stop any task.
        /// </summary>
        public void Stop()
        {
            if (IsPlaying)
            {
                IsPlaying = false;
                IsPaused = false;
                OnEnd();
                OnEndEvent.Invoke();
            }
        }

        /// <summary>
        /// Method to be called to abort any task.
        /// </summary>
        public void Abort()
        {
            IsPlaying = false;
            IsPaused = false;
            OnAbort();
        }

        /// <summary>
        /// This method gets called right after a task is marked as IsPlaying and right before the OnStartEvent public event is invoked.
        /// </summary>
        protected virtual void OnStart() { }

        /// <summary>
        /// This method gets called as soon as a task is marked as aborted.
        /// </summary>
        protected virtual void OnAbort() { }

        /// <summary>
        /// This method gets called every frame when a task is running and not paused.
        /// </summary>
        protected virtual void OnUpdate() { }

        /// <summary>
        /// This method is called as soon as a task is marked for stop.
        /// </summary>
        protected virtual void OnEnd() { }

        /// <summary>
        /// This method is called as soon as a task is marked for pause.
        /// </summary>
        protected virtual void OnPause() { }

        /// <summary>
        /// This method is called as soon as the task is marked for resume. Called only if the application is playing.
        /// </summary>
        protected virtual void OnResume() { }

        // Update is called once per frame
        void Update()
        {
            if (IsPlaying && !IsPaused)
            {
                OnUpdate();
                OnUpdateEvent.Invoke();
            }
        }

        /// <summary>
        /// This method returns the name of the task.
        /// </summary>
        public override string ToString()
        {
            return TaskName;
        }

    }
}