using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SillyGames.SGBase.UI
{

    [RequireComponent(typeof(UnityEngine.UI.Toggle))]
    public class UIToggle : Toggle
    {
        private UnityEngine.UI.Toggle m_unityToggle = null;
        internal UnityEngine.UI.Toggle UnityToggle 
        {
            get
            {
                if (m_unityToggle == null)
                {
                    m_unityToggle = GetComponent<UnityEngine.UI.Toggle>();
                }
                return m_unityToggle;
            }
        }

        //protected override void OnDeactivatedStatusChanged(UIElement a_targetElement)
        //{
        //    base.OnDeactivatedStatusChanged(a_targetElement);
        //    UnityToggle.interactable = !Deactivated;
        //}
    }
}