using System;
using System.Collections.Generic;
using UnityEngine;

namespace SillyGames.SGBase.Localization
{
    [Serializable]
    public class LocalizableData<T>
    {
		public string group;
        public string key;
        public T data;
    }

    [Serializable]
	public class LocalizationBase<T,V> :LocalizationObject where V : LocalizableData<T>
    {
        [SerializeField]
        private string m_language = string.Empty;
        public string Language 
        { 
            get
            {
                return m_language;
            }
            set
            {
                m_language = value;
            }
        }


		public override Type KeyType()
		{
			return typeof(T);
		}

		public override Type DataType()
		{
			return typeof(V);
		}

        [SerializeField]
		//Praveen:- made public for editor access 
		//Its either increasing visibility or reflection, and this would be less performance hungry
        public List<V> m_rawData = null;

		private static Dictionary<string,Dictionary<string,T>> m_groupDictionary = new Dictionary<string, Dictionary<string, T>> ();
        private Dictionary<string, T> m_data = new Dictionary<string, T>();

        private static LocalizationBase<T, V> s_instance = null;


		//Praveen:- made public for editor access 
		//Its either increasing visibility or reflection, and this would be less performance hungry
        public void UpdateFromRawData()
        {
            m_data.Clear();
			m_groupDictionary.Clear ();
			for (int i = 0; i < m_rawData.Count; ++i)
			{
				var item = m_rawData [i];
				if (!m_groupDictionary.ContainsKey (item.group))
				{
					m_groupDictionary.Add (item.group, new Dictionary<string, T> ());
				}
				this[item.key] = item.data;
				m_groupDictionary [item.group] [item.key] = item.data;
			}
        }

        private T this[string a_strKey]
        {
            get
            {
                T val;
                if(m_data.TryGetValue(a_strKey,out val))
                {
                    return val;
                }
                return default(T);
            }
            set
            {
                m_data[a_strKey] = value;
            }
        }
        
        void OnEnable()
        {
            UpdateFromRawData();
            s_instance = this;
            UpdateLocalizationForAllLocalizers();
        }

		public static T Get(string a_strKey,string a_grpKey = "")
        {
			//if a group key was specified, skip the m_data dictionary check and only look in the groupDictionary
			if (a_grpKey != null && a_grpKey.Length > 0)
			{
				if (m_groupDictionary.ContainsKey (a_grpKey))
				{
					try
					{
						return m_groupDictionary[a_grpKey][a_strKey];
					}
					catch( KeyNotFoundException e)
					{
						Debug.LogWarning("Localization Key: "+a_strKey+" not found in group "+a_grpKey+" for type: "+ typeof(T)+" Details: "+e);
					}
				}
			}

			//if no group was specified, get the first instance of the key
            if(s_instance != null)
            {
                try
                {
                    return s_instance[a_strKey];
                }
                catch( KeyNotFoundException e)
                {
                    Debug.LogWarning("Localization Key: "+a_strKey+" not found for type: "+ typeof(T)+" Details: "+e);
                }
            }
            return default(T);
        }

        public static string CurrentLanguageName
        {
            get
            {
                if(s_instance != null)
                {
                    return s_instance.Language;
                }
                return string.Empty;
            }
        }
        public abstract class Localizer: MonoBehaviour
        {
            [SerializeField]
            private string m_Key = string.Empty;
            public string Key
            {
                get
                {
                    return m_Key;
                }
                
                internal set
                {
                    m_Key = value;
                }
            }

            private string m_language = string.Empty;
            internal string Language
            {
                get
                {
                    return m_language;
                }
            }

            internal void SetLanguage(string a_language)
            {
                m_language = a_language;
            }

            public abstract void Set(T a_data);

            protected virtual void Start()
            {

                LocalizationBase<T,V>.Register(this);
            }

            protected virtual void OnDestroy()
            {
                LocalizationBase<T, V>.Unregister(this);
            }
        }

        public abstract class LocalizerBehaviour<COMPONENT> : Localizer where COMPONENT: Component
        {
            [SerializeField]
            private COMPONENT m_target = null;
            public COMPONENT Target
            {
                get
                {
                    if (m_target == null)
                    {
                        m_target = GetComponent<COMPONENT>();
                    }
                    return m_target;
                }
                protected set
                {
                    m_target = value;
                }
            }
            void Reset()
            {
                m_target = GetComponent<COMPONENT>();
            }
        }
        private static List<Localizer> s_lstLocalizers = new List<Localizer>();
        internal static void Register(Localizer a_localizer)
        {
            if (!s_lstLocalizers.Contains(a_localizer))
            {
                s_lstLocalizers.Add(a_localizer);
                if(s_instance != null)
                {
                    s_instance.UpdateLocalizationForLocalizer(a_localizer);
                }
            }
        }

        internal static void Unregister(Localizer a_localizer)
        {
            s_lstLocalizers.Remove(a_localizer);
        }

        private bool IsLocalizerDirty(Localizer a_localizer)
        {
            return a_localizer.Language == Language;
        }

        private static string GetLanguage()
        {
            return s_instance.Language;
        }

        private void UpdateLocalizationForLocalizer(Localizer a_localizer)
        {
            a_localizer.SetLanguage(Language);

            a_localizer.Set(Get(a_localizer.Key));
        }

        private void UpdateLocalizationForAllLocalizers()
        {
            foreach (var item in s_lstLocalizers)
            {
                UpdateLocalizationForLocalizer(item);
            }
        }
    }

}
