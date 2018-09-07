using UnityEngine;
using System.Collections;
using SillyGames.SGBase.TaskSystem;

namespace SillyGames.SGBase.TaskSystem
{
    public class RepetitionTask : Task
    {
        [SerializeField]
        private Task m_targetTask = null;
        public Task TargetTask
        {
            get
            { 
                return m_targetTask; 
            }

            set
            {
                m_targetTask = value;
            }
        }

        [SerializeField]
        private int m_iterations = 1;

        public int Iterations
        {
            get
            {
                return m_iterations;
            }

            set
            {
                m_iterations = value;
            }
        }

        private int m_iIterationsComplete;
        protected override void OnStart()
        {
            m_iIterationsComplete = 0;
            TargetTask.EndEvent.RemoveListener(OnTargetTaskEnded);            
            TargetTask.EndEvent.AddListener(OnTargetTaskEnded);
            TargetTask.Execute();
            base.OnStart();
        }

        protected override void OnEnd()
        {
            TargetTask.EndEvent.RemoveListener(OnTargetTaskEnded);
            base.OnEnd();
        }
        private void OnTargetTaskEnded()
        {
            if (Iterations == 0)
            {
                TargetTask.Execute();
                
            }
            else
            {
                m_iIterationsComplete++;

                if (m_iIterationsComplete < Iterations)
                {
                    TargetTask.Execute();    
                }
                else
                {
                    Stop();
                }
            }
        }


    }
}
