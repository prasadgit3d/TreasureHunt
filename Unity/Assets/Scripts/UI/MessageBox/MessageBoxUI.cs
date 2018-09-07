
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using ButtonPool = SillyGames.SGBase.Utilities.UnityObjectPooler<SillyGames.TreasureHunt.MessageBoxButton, int>;
namespace SillyGames.TreasureHunt
{
    internal class MessageBoxUI : MonoBehaviour, IMessageBoxUI
    {
        internal static int s_defaultButtonCacheCount = 5;

        private Canvas Cavas { get; set; }
        [SerializeField]
        private MessageBoxButton m_buttonPrefab = null;

        [SerializeField]
        private RectTransform m_buttonParent = null;

        [SerializeField]
        private Text m_titleText = null;

        [SerializeField]
        private Text m_sideText = null;

        [SerializeField]
        private Text m_belowText = null;

        [SerializeField]
        private Image m_image = null;


        public string LowerText
        {
            get
            {
                return m_belowText.text;
            }
            set
            {
                m_belowText.text = value;

                m_belowText.gameObject.SetActive(!string.IsNullOrEmpty(value));
            }
        }

        public Sprite Image
        {
            get
            {
                return m_image.sprite;
            }
            set
            {
                m_image.sprite = value;
                m_image.gameObject.SetActive(value != null);
            }
        }

        public string MainText
        {
            get
            {
                return m_sideText.text;
            }
            set
            {
                m_sideText.text = value;

                m_sideText.gameObject.SetActive(!string.IsNullOrEmpty(value));
            }
        }

        public string TitleText
        {
            get
            {
                return m_titleText.text;
            }
            set
            {
                m_titleText.text = value;
            }
        }

        void IMessageBoxUI.Dispose()
        {
            MessageBox.Dispose(this);
        }

        internal void ClearAllButtons()
        {
            foreach (var item in m_lstButtonList)
            {
                RecycleButton(item);
            }
            m_lstButtonList.Clear();
        }

        public void AppendButton(string a_text, Action a_calback, bool a_bUseLocalization = false)
        {
            AddButton(true, a_text, a_calback, a_bUseLocalization);
        }

        public void PrependButton(string a_text, Action a_calback, bool a_bUseLocalization = false)
        {
            AddButton(false, a_text, a_calback, a_bUseLocalization);
        }

        private void AddButton(bool a_bAppend, string a_text, Action a_calback, bool a_bUseLocalization)
        {
            var button = GetNewButton();
            button.Text = a_bUseLocalization ? SGBase.Localization.TextLocalization.Get(a_text) : a_text;
            button.Callback = a_calback;

            if (a_bAppend)
            {
                button.transform.SetAsLastSibling();
                m_lstButtonList.Add(button);
            }
            else
            {
                button.transform.SetAsFirstSibling();
                m_lstButtonList.Insert(0,button);
            }

            button.gameObject.SetActive(true);
        }

        private List<MessageBoxButton> m_lstButtonList = new List<MessageBoxButton>(s_defaultButtonCacheCount);

        public bool IsInited { get; private set; }
        private static int TotalInstances { get; set; }
        private static int RenderingOrder { get; set; }
        public void Init()
        {
            ButtonPool.SetBaseObject(GetInstanceID(), m_buttonPrefab);
            var tempButtonList = new MessageBoxButton[s_defaultButtonCacheCount];
            for (int i = 0; i < s_defaultButtonCacheCount; i++)
            {
                tempButtonList[i] = GetNewButton();

            }
            foreach (var item in tempButtonList)
            {
                RecycleButton(item);
            }
            TitleText = string.Empty;
            MainText = string.Empty;
            LowerText = string.Empty;
            Image = null;
            gameObject.SetActive(false);
            name = string.Format("MsgBoxInstance[{0}]", TotalInstances++);
            Cavas = GetComponent<Canvas>();
            IsInited = true;
        }

        public void Show()
        {
            gameObject.SetActive(true);
            Cavas.sortingOrder = RenderingOrder++;
        }

        private MessageBoxButton GetNewButton()
        {
            var newButton = ButtonPool.RetrieveFromPool(GetInstanceID());
            if(!newButton.IsInited)
            {
                InitButton(newButton);
            }
            return newButton;
        }

        private void InitButton(MessageBoxButton a_newButton)
        {
            a_newButton.Text = string.Empty;
            a_newButton.transform.SetParent(m_buttonParent, false);
            a_newButton.IsInited = true;
        }

        private void RecycleButton(MessageBoxButton a_button)
        {
            a_button.gameObject.SetActive(false);
            ButtonPool.ReturnToPool(GetInstanceID(), a_button);
        }

        public void AppendButton(MessageBox.ButtonArgs buttonArg, bool a_bUseLocalization)
        {
            AppendButton(buttonArg.Text, buttonArg.Action,a_bUseLocalization);
        }

        public void PrependButton(MessageBox.ButtonArgs buttonArg, bool a_bUseLocalization)
        {
            PrependButton(buttonArg.Text, buttonArg.Action, a_bUseLocalization);
        }
    }
}
