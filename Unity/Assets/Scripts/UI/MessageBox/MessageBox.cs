
using SillyGames.SGBase.Localization;
using System;
using System.Collections.Generic;
using UnityEngine;
using MessageBoxPool = SillyGames.SGBase.Utilities.UnityObjectPooler<SillyGames.TreasureHunt.MessageBoxUI>;

namespace SillyGames.TreasureHunt
{
    public static class MessageBox
    {
        private static GameObject CacheParentGameObject = new GameObject("MessageBoxCache(DontDelete)");
        internal static void Init(MessageBoxUI a_messageBoxPrefab, int a_iPrecreateCount)
        {
            GameObject.DontDestroyOnLoad(CacheParentGameObject);
            MessageBoxPool.BaseObject = a_messageBoxPrefab;
           var tempMessageBoxArray = new MessageBoxUI[a_iPrecreateCount];
            for (int i = 0; i < a_iPrecreateCount; i++)
            {
                tempMessageBoxArray[i] = (MessageBoxUI)GetNew();
            }

            foreach (var item in tempMessageBoxArray)
            {
                Dispose(item);
            }
            
        }

        public static IMessageBoxUI ShowOK( string a_title,
                                            string a_mainText,
                                            string a_okText = "OK",
                                            Action a_callback = null,                                             
                                            bool a_bUseLocalization = false)
        {

            return ShowOK(a_title, a_mainText, null, a_okText, a_callback, a_bUseLocalization);
        }

        public static IMessageBoxUI ShowOK(string a_title,
                                            Sprite a_image,
                                            string a_okText = "OK",
                                            Action a_callback = null,
                                            bool a_bUseLocalization = false)
        {
            return ShowOK(a_title, string.Empty, a_image, a_okText, a_callback, a_bUseLocalization);
        }
        public static IMessageBoxUI ShowOK(string a_title,
                                            string a_mainText,
                                            Sprite a_image,
                                            string a_okText = "OK",
                                            Action a_callback = null,
                                            bool a_bUseLocalization = false)
        {
            var mb = Show(a_title, a_mainText, string.Empty, a_image, a_bUseLocalization);
            var buttonArg = new ButtonArgs(a_okText);
            buttonArg.Action = () => { InvokeCallbackAndDispose(a_callback, mb); };
            mb.AppendButton(buttonArg, a_bUseLocalization);
            return mb;
        }


        public static IMessageBoxUI ShowOKCancel(string a_title,
                                                    string a_mainText,
                                                    Sprite a_image,
                                                    string a_okText = "OK",
                                                    string a_cancelText = "Cancel",
                                                    Action<bool> a_callback = null,
                                                    bool a_bUseLocalization = false)
        {
            var mb = Show(a_title, a_mainText, string.Empty, a_image, a_bUseLocalization);
            var buttonArg1 = new ButtonArgs(a_okText);
            buttonArg1.Action = () => { InvokeCallbackAndDispose(a_callback, true, mb); };

            var buttonArg2 = new ButtonArgs(a_cancelText);
            buttonArg2.Action = () => { InvokeCallbackAndDispose(a_callback, false, mb); };

            mb.AppendButton(buttonArg1, a_bUseLocalization);
            mb.AppendButton(buttonArg2, a_bUseLocalization);
            return mb;
        }

        public static IMessageBoxUI ShowOKCancel(string a_title,
                                                    string a_mainText,
                                                    string a_okText = "OK",
                                                    string a_cancelText = "Cancel",
                                                    Action<bool> a_callback = null,
                                                    bool a_bUseLocalization = false)
        {
            return ShowOKCancel(a_title, a_mainText,null, a_okText,a_cancelText,a_callback, a_bUseLocalization);
        }

        public static IMessageBoxUI ShowOKCancel(string a_title,
                                                    Sprite a_image,
                                                    string a_okText = "OK",
                                                    string a_cancelText = "Cancel",
                                                    Action<bool> a_callback = null,
                                                    bool a_bUseLocalization = false)
        {
            return ShowOKCancel(a_title, string.Empty, a_image, a_okText, a_cancelText, a_callback, a_bUseLocalization);
        }

        public static IMessageBoxUI Show(string a_title,
                                            string a_mainText,
                                            string a_lowerText,
                                            Sprite a_image,
                                            bool a_bUseLocalization,
                                            params ButtonArgs[] a_buttonArgs)
        {
            var mb = (MessageBoxUI)GetNew();
            mb.TitleText = a_bUseLocalization? TextLocalization.Get(a_title):a_title;
            mb.MainText = a_bUseLocalization ? TextLocalization.Get(a_mainText) : a_mainText;
            mb.LowerText = a_bUseLocalization ? TextLocalization.Get(a_lowerText) : a_lowerText;
            mb.Image = a_image;
            mb.ClearAllButtons();
            foreach (var item in a_buttonArgs)
            {
                mb.AppendButton(item,a_bUseLocalization);
            }
            mb.Show();
            return mb;
        }

        public struct ButtonArgs
        {
            public string Text;
            public Action Action;
            public ButtonArgs(string a_text)
            {
                Text = a_text;
                Action = null;
            }

            public ButtonArgs(string a_text, Action a_calback)
            {
                Text = a_text;
                Action = a_calback;
            }

        }

        private static void InvokeCallbackAndDispose(Action a_callback, IMessageBoxUI a_messageBox)
        {
            if (a_callback != null)
            {
                a_callback.Invoke();
            }
            a_messageBox.Dispose();
        }

        private static void InvokeCallbackAndDispose(Action<bool> a_callback, bool a_bValue, IMessageBoxUI a_messageBox)
        {
            if (a_callback != null)
            {
                a_callback.Invoke(a_bValue);
            }
            a_messageBox.Dispose();
        }


        private static IMessageBoxUI GetNew()
        {
            var messageBoxNew = MessageBoxPool.RetrieveFromPool();
            if(!messageBoxNew.IsInited)
            {
                messageBoxNew.Init();
                messageBoxNew.transform.SetParent(CacheParentGameObject.transform);
            }
            return messageBoxNew;
        }
        internal static void Dispose(MessageBoxUI a_messageBoxUI)
        {
            a_messageBoxUI.gameObject.SetActive(false);
            MessageBoxPool.ReturnToPool(a_messageBoxUI);
        }
    }
}
