using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SillyGames.SGBase.UI
{

    [RequireComponent(typeof(UnityEngine.UI.InputField))]
    public class UITextBox : InputField
    {
        private UnityEngine.UI.InputField m_unityInputField = null;
        private UnityEngine.UI.InputField UnityInputField 
        {
            get
            {
                if(m_unityInputField == null)
                {
                    m_unityInputField = GetComponent<UnityEngine.UI.InputField>();
                }
                return m_unityInputField;
            }
        }

        protected override void Start()
        {
            base.Start();
            UnityInputField.onEndEdit.AddListener(OnEndEdit);
            UnityInputField.onValueChanged.AddListener(OnValueChange);
        }

        //protected override void OnDeactivatedStatusChanged(UIElement a_targetElement)
        //{
        //    base.OnDeactivatedStatusChanged(a_targetElement);
        //    UnityInputField.interactable = !Deactivated;
        //}
        
        protected virtual void OnEndEdit(string a_text){}

        protected virtual void OnValueChange(string a_text) { }

        public string Text
        {
            get
            {
                return UnityInputField.text;
            }

            set
            {
                UnityInputField.text = value;
            }
        }
    }
}