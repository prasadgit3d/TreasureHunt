using UnityEngine;
using System.Collections;
using SillyGames.SGBase.TaskSystem;
using UnityEngine.Events;

namespace SillyGames.TreasureHunt
{

    public class StateSplashScreen : THGameState
    {
        [SerializeField]
        private UnityEvent m_onEnterEvent = null;

        [SerializeField]
        private UnityEvent m_onExitEvent = null;

        // Use this for initialization
        void Start()
        {

        }

        protected override void OnEnter()
        {
            base.OnEnter();
            m_onEnterEvent.Invoke();
        }

        public void OnFadingInComplete()
        {
            Debug.Log("Fading in complete");
            m_onExitEvent.Invoke();
        }

        public void OnFadingOutComplete()
        {

        }

    }
}