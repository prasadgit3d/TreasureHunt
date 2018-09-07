using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SillyGames.TreasureHunt
{
    /// <summary>
    /// base class for all the screens
    /// </summary>
    public class UIScreen : MonoBehaviour
    {
        protected virtual void Awake()
        {
            Canvas.enabled = false;
        }

        private Canvas m_canvas = null;
        public Canvas Canvas
        {
            get
            {
                if (m_canvas == null)
                {
                    m_canvas = GetComponentInChildren<Canvas>(true);
                }
                return m_canvas;
            }
        }
        protected internal virtual void HideInternal()
        {
            Canvas.enabled = false;
        }

        protected internal virtual void ShowInternal()
        {
            Canvas.enabled = true;
        }

        public void Hide()
        {
            UIManager.ShowPrevious();
        }

        public void Show()
        {
            UIManager.Show(this);
        }
    }
}