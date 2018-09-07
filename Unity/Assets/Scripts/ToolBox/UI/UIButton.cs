using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SillyGames.SGBase.UI
{
    public class UIButton : Button
    {
        private UnityEngine.UI.Button m_unityButton = null;
        public UnityEngine.UI.Button UnityButton 
        {
            get
            {
                if (m_unityButton == null)
                {
                    m_unityButton = GetComponent<UnityEngine.UI.Button>();
                }
                return m_unityButton;
            }
        }

        public bool Activated
        {
            get
            {
                return UnityButton.interactable;
            }

            set
            {
                if (value != UnityButton.interactable)
                {
                    UnityButton.interactable = value;
                }
            }
        }

        public bool IsShown
        {
            get
            {
                return gameObject.activeSelf;
            }

            private set
            {
                gameObject.SetActive(value);
            }
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        //protected override void OnDeactivatedStatusChanged(UIElement a_element)
        //{
        //    base.OnDeactivatedStatusChanged(a_element);
        //    UnityButton.interactable = !Deactivated;
        //}
    }
}