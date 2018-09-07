using UnityEngine;
using System.Collections;
using SillyGames.SGBase;
using System.Diagnostics;

namespace SillyGames.SGBase
{
    /// <summary>
    /// serves as a base class for all monobehaviour states
    /// it also provides a debug ui overridable to put your debug ui code, this function will be ignored for release builds
    /// by default debug ui is disabled, you have to define 'DEBUG_UI', in order to enable it
    /// </summary>
    public class GameStateBase : MonoBehaviour, IState
    {
        private bool m_bIsRunning = false;
        public bool IsRunning
        {
            get
            {
                return m_bIsRunning;
            }
            private set
            {
                m_bIsRunning = value;
            }
        }

        void Start()
        {
            Init();
        }
        protected virtual void Init() { }
        void IState.OnEnter()
        {
            IsRunning = true;
            OnEnter();
        }
        protected virtual void OnEnter() { }

        void IState.OnExit()
        {
            IsRunning = false;
            OnExit();
        }
        protected virtual void OnExit() { }

        void IState.OnUpdate(float a_fDeltaTime)
        {
            OnUpdate(a_fDeltaTime);
        }
        protected virtual void OnUpdate(float a_fDeltaTime) { }

        #region debug UI
#if UNITY_DEBUG
        [Conditional("DEBUG_UI")]
        void OnGUI()
        {
            if (IsRunning)
            {
                GUILayout.Label("Current State: " + this);
                OnDebugGUI();
            }
        }
#endif
        protected virtual void OnDebugGUI() { }
        #endregion       
        
    }
}