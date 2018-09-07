using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SillyGames.TreasureHunt
{

    public class UIManager : MonoBehaviour
    {
        private static Dictionary<Type, UIScreen> m_screens = new Dictionary<Type, UIScreen>();
        public static T GetUIScreenOfType<T>() where T : UIScreen
        {
            if (!m_screens.ContainsKey(typeof(T)))
            {
                var newScreen = FindObjectOfType<T>();
                if (newScreen != null)
                {
                    m_screens.Add(newScreen.GetType(), newScreen);
                }
                return newScreen;
            }
            return (T)m_screens[typeof(T)];
        }

        public static UIScreen CurrentScreen { get; private set; }
        public static UIScreen PreviousScreen { get; private set; }

        private static UIManager s_instnace = null;
        private static UIManager Instance
        {
            get
            {
                if (s_instnace == null)
                {
                    var go = new GameObject("UIManager", typeof(UIManager));
                    s_instnace = go.GetComponent<UIManager>();
                }
                return s_instnace;
            }
        }

        public static void Show<T>() where T : UIScreen
        {
            var screen = GetUIScreenOfType<T>();
            if (screen != null)
            {
                Show(screen);
            }
            else
            {
                Debug.LogWarning("Couldnt find a screen type to show: " + typeof(T));
            }
        }

        public static void Show(UIScreen a_screen)
        {
            if (CurrentScreen != a_screen)
            {
                if (CurrentScreen != null)
                {
                    CurrentScreen.HideInternal();
                }
                PreviousScreen = CurrentScreen;
                CurrentScreen = a_screen;
                if (CurrentScreen != null)
                {
                    CurrentScreen.ShowInternal();
                }
            }
            else
            {
                Debug.LogWarning("Same screen was asked to show: " + a_screen);
            }
        }


        public static void ShowPrevious()
        {
            Show(PreviousScreen);
        }
    }
}