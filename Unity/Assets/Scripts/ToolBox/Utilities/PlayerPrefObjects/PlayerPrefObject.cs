using SillyGames.SGBase;
using System;
using UnityEngine;

namespace SillyGames.SGBase.Utilities
{
    [System.Serializable]
    public class PlayerPrefObject<T>
    {
        public PlayerPrefObject()
        {
            Init(null, null);
        }
        public PlayerPrefObject(string a_keyName)
        {
            Init(a_keyName, null);
        }

        public PlayerPrefObject(string a_keyName, Func<string, T> a_parser)
        {
            Init(a_keyName, a_parser);
        }
        private void Init(string a_keyName, Func<string, T> a_parser)
        {
            KeyName = a_keyName;
            Parser = a_parser;
        }
        public Func<string, T> Parser { get; set; }


        [SerializeField]
        private T m_value = default(T);
        private bool IsClean { get; set; }

        public T Value
        {
            get
            {
                if (!IsClean)
                {
                    if (!string.IsNullOrEmpty(KeyName))
                    {
                        string strVal = Misc.getFromVault(KeyName);

                        if (Parser != null)
                        {
                            m_value = Parser(strVal);
                            IsClean = true;
                        }
                    }
                }
                return m_value;
            }

            set
            {
                m_value = value;
                Misc.setToVault(KeyName, value.ToString());
                IsClean = true;
            }
        }

        public string KeyName
        {
            get;
            set;
        }
    }
}