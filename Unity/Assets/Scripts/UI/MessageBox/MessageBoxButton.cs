using System;
using UnityEngine;
using UnityEngine.UI;

namespace SillyGames.TreasureHunt
{
    internal class MessageBoxButton: Button
    {
        protected override void Awake()
        {
            onClick.AddListener(OnButtonClicked);
        }
        public Action Callback { get; set;}

        [SerializeField]
        private Text m_text = null;

        public string Text { get { return m_text.text; } set { m_text.text = value; } }

        public void OnButtonClicked()
        {
            if(Callback != null)
            {
                Callback.Invoke();
            }
        }

        public bool IsInited { get; set; }
    }
}
