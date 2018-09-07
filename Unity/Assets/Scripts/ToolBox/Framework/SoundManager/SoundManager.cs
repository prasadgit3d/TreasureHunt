using UnityEngine;
using System.Collections.Generic;
using System;

namespace SillyGames.SGBase
{
    /// <summary>
    /// works as an audio atlas 
    /// </summary>
    public class SoundManager : MonoBehaviour
    {
        /// <summary>
        /// Hold the editor time data for audio clip, audio segment,
        /// this is then poured in to a hashmap at runtime to treat it as a sound atlas
        /// </summary>
        [Serializable]
        public class AudioFileEntry
        {
            [Tooltip("A key to programmatically include the file entry in sound atlas at runtime")]
            [SerializeField]
            private string m_strEntryKey = string.Empty;
            public string FileEntryKey
            {
                get
                {
                    return m_strEntryKey;
                }
                set
                {
                    m_strEntryKey = value;
                }
            }

            [SerializeField]
            private AudioClip m_audioClip = null;

            /// <summary>
            /// audio clip that will act as an atlas
            /// </summary>
            public AudioClip AudioClip
            {
                get
                {
                    return m_audioClip;
                }

                set
                {
                    m_audioClip = value;
                }
            }

            [Tooltip("If true, will get included in sound atlas on 'Awake()'")]
            [SerializeField]
            private bool m_bAutoInclude = true;
            public bool AutoInclude
            {
                get
                {
                    return m_bAutoInclude;
                }

                set
                {
                    m_bAutoInclude = value;
                }
            }


            [SerializeField]
            private List<AudioSegmentEntry> m_lstSoundEntries = new List<AudioSegmentEntry>();
            public List<AudioSegmentEntry> SoundEntries
            {
                get
                {
                    return m_lstSoundEntries;
                }

            }
        }

        [Serializable]
        public class AudioSegmentEntry
        {
            [SerializeField]
            private string m_strKey = string.Empty;
            public string Key
            {
                get
                {
                    return m_strKey;
                }
                set
                {
                    m_strKey = value;
                }
            }

            [SerializeField]
            private float m_fStart = 0.0f;
            public float Start
            {
                get { return m_fStart; }
                set { m_fStart = value; }
            }

            [SerializeField]
            private float m_fEnd = 0.0f;
            public float End
            {
                get { return m_fEnd; }
                set { m_fEnd = value; }
            }

            public AudioSegmentEntry() { }
            public AudioSegmentEntry(AudioSegmentEntry audioSegmentEntry)
            {
                this.Start = audioSegmentEntry.Start;
                this.End = audioSegmentEntry.End;
                this.Key = audioSegmentEntry.Key;
            }

            public float Duration
            {
                get
                {
                    return End - Start;
                }
                set
                {
                    End = Start + value;
                }
            }

            public AudioClip AudioClip { get; set; }
        }

        //[Tooltip("If left blank, will get assigned automatically by adding a new one")]
        //[SerializeField]
        private AudioSource m_audioSource = null;
        
        [SerializeField]
        private int m_initialAudioSourceCount = 1;
        public int AudioSourcePoolCount
        {
            get
            {
                return m_initialAudioSourceCount;
            }
            set
            {
                m_initialAudioSourceCount = value;
            }
        }

        private List<AudioSource> m_lstAudioSourcePool = new List<AudioSource>();

        private List<AudioSource> m_lstAudioSourceBeingPlayed = new List<AudioSource>();

        public int InitialAudioSourceCount
        {
            get { return m_initialAudioSourceCount; }
            set { m_initialAudioSourceCount = value; }
        }


        [SerializeField]
        private List<AudioFileEntry> m_lstFileEntries = new List<AudioFileEntry>();
        private List<Dictionary<string, AudioSegmentEntry>> m_lstSoundAtlas = new List<Dictionary<string, AudioSegmentEntry>>();


        private static SoundManager s_instance = null;
        public static SoundManager Instance
        {
            get
            {
                return s_instance;
            }
            private set
            {
                s_instance = value;
            }

        }

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Debug.LogWarning("Multiple instances of " + GetType() + " was found, this instance will be ignored: " + this);
                return;
            }

            for (int i = 0; i < InitialAudioSourceCount; i++)
            {
                AddNewAudioSource();
            }
            if(m_lstAudioSourcePool.Count <= 0)
            {
                AddNewAudioSource();
            }
            m_audioSource = m_lstAudioSourcePool[0];
            RegisterAllAutoIncludedAatlases();
        }

        private AudioSource AddNewAudioSource()
        {
            var audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;

            m_lstAudioSourcePool.Add(audioSource);
            
            return audioSource;
        }

        private void RegisterAllAutoIncludedAatlases()
        {
            for (int i = 0; i < m_lstFileEntries.Count; i++)
            {
                var fileEntry = m_lstFileEntries[i];
                if (fileEntry.AutoInclude)
                {
                    RegisterAtlas(fileEntry);
                }
            }
        }

        private void RegisterAtlas(AudioFileEntry a_fileEntry)
        {
            if (a_fileEntry.AudioClip != null)
            {
                //if (Debug.isDebugBuild)
                //{
                //    Debug.Log("Registering fileEntry entry with a key: " + a_fileEntry.FileEntryKey);
                //}
            }
            else
            {
                Debug.LogWarning("Audio clip for AudioFileEntry was null, it will be ignored, Key was: " + a_fileEntry.FileEntryKey);
                return;
            }
            var dictionary = new Dictionary<string, AudioSegmentEntry>();
            foreach (var item in a_fileEntry.SoundEntries)
            {
                //if (Debug.isDebugBuild)
                //{
                //    Debug.Log("Adding Audio segment with a key: " + item.Key);
                //}
                item.AudioClip = a_fileEntry.AudioClip;
                dictionary.Add(item.Key, item);
            }
            m_lstSoundAtlas.Add(dictionary);
        }

        public void RegisterAtalsByeKey(string a_strKey)
        {
            foreach (var item in m_lstFileEntries)
            {
                if (a_strKey.Equals(item.FileEntryKey, StringComparison.InvariantCultureIgnoreCase))
                {
                    RegisterAtlas(item);
                    return;
                }
            }
        }

        public float GetAudioSegmentLength(string a_strKey)
        {
            foreach (var item in m_lstSoundAtlas)
            {
                if (item.ContainsKey(a_strKey))
                {
                    AudioSegmentEntry entry = item[a_strKey];
                    return entry.Duration;
                }
            }
            return 0.0f;
        }

        /// <summary>
        /// plays a audio segment with default audio source
        /// </summary>
        /// <param name="a_strKey">audio entry key</param>
        /// <returns>true if audio segment is played, false otherwise</returns>
        public bool Play(string a_strKey)
        {
            return Play(a_strKey,true);
        }
        
        /// <summary>
        /// plays a audio segment with default audio source
        /// </summary>
        /// <param name="a_strKey">audio entry key</param>
        /// <param name="a_bAdditive">if true, plays the audio in a different audioSource, causing audio to be played independently</param>
        /// <returns></returns>
        public bool Play(string a_strKey,bool a_bAdditive)
        {
            //if (Debug.isDebugBuild)
            //{
            //    Debug.Log("Playing: " + a_strKey + ", additive: " + a_bAdditive + " at time: " + Time.time);
            //}
            if (a_bAdditive)
            {
                var audioSource = GetNewAudioSource();
                if (Play(a_strKey, audioSource))
                {
                    return true;
                }
                else
                {
                    RecycleOldAudioSource(audioSource);
                    return false;
                }
            }
            else
            {
                return Play(a_strKey, m_audioSource);
            }
        }

        private AudioSource GetNewAudioSource()
        {
            if(m_lstAudioSourcePool.Count <= 1)
            {
                Debug.LogWarning("Audio source pool exausted, adding a new one, currently being played count: " + m_lstAudioSourceBeingPlayed.Count);
                if(Debug.isDebugBuild)
                {
                    string str = "Total " + m_lstSegmentsBeingPlayed.Count + " sounds are being played, that are as follows..";
                    for (int i = 0; i < m_lstSegmentsBeingPlayed.Count; i++)
                    {
                        str += "\n" + m_lstSegmentsBeingPlayed[0].ToString();
                    }

                    Debug.Log(str);
                }
                m_lstAudioSourcePool.Add(gameObject.AddComponent<AudioSource>());
            }

            var lastItemIndex = m_lstAudioSourcePool.Count - 1;
            var item = m_lstAudioSourcePool[lastItemIndex];
            m_lstAudioSourcePool.RemoveAt(lastItemIndex);
            m_lstAudioSourceBeingPlayed.Add(item);
            return item;
        }

        private void RecycleOldAudioSource(AudioSource a_audioSource)
        {
            System.Diagnostics.Debug.Assert(a_audioSource != null);
            if(m_lstAudioSourceBeingPlayed.Remove(a_audioSource))
            {
                m_lstAudioSourcePool.Add(a_audioSource);
            }
            else
            {
                Debug.LogWarning("The audio source is not in the 'Audiosouce being played' list: " + a_audioSource);
            }
        }

        public bool Play(string a_strKey, AudioSource a_audioSource)
        {
            foreach (var item in m_lstSoundAtlas)
            {
                if (item.ContainsKey(a_strKey))
                {
                    AudioSegmentEntry entry = item[a_strKey];
                    //m_audioSource.clip = entry.AudioClip;
                    Play(a_audioSource, entry.AudioClip, entry.Start, entry.End);
                    return true;
                }
            }
            return false;
        }

        private void Play(AudioSource a_audioSource, AudioClip a_clip, float a_fStart, float a_fEnd)
        {
            bool bUsedExistingEntry = false;

            for (int i = 0; i < m_lstSegmentsBeingPlayed.Count; i++)
            {
                if (m_lstSegmentsBeingPlayed[i].AudioSource == a_audioSource && m_lstSegmentsBeingPlayed[i].AudioSource.clip == a_clip)
                {
                    var item = m_lstSegmentsBeingPlayed[i];
                    item.StartTime = Time.time;
                    m_lstSegmentsBeingPlayed[i] = item;
                    bUsedExistingEntry = true;
                }
            }

            a_audioSource.Stop();
            a_audioSource.clip = a_clip;
            a_audioSource.time = a_fStart;
            a_audioSource.Play();
            if (!bUsedExistingEntry)
            {
                m_lstSegmentsBeingPlayed.Add(new AudioSegmentsBeingPlayed(a_audioSource, a_fEnd - a_fStart));
            }
        }

        #region Keeping track of each audio segment being played

        void Update()
        {
            for (int i = 0; i < m_lstSegmentsBeingPlayed.Count; i++)
            {

                ///if the audiosource is assigned some other audioclip, of the time has run out, then remove this entry;
                if (m_lstSegmentsBeingPlayed[i].AudioSource.clip != m_lstSegmentsBeingPlayed[i].AudioClip)
                {
                    m_lstSegmentsBeingPlayed.RemoveAt(i);
                    --i;
                    continue;
                }
                else
                {
                    if (m_lstSegmentsBeingPlayed[i].ElapsedTime >= m_lstSegmentsBeingPlayed[i].Duration)
                    {
                        m_lstSegmentsBeingPlayed[i].AudioSource.Stop();
                        m_lstSegmentsBeingPlayed.RemoveAt(i);
                        --i;
                    }
                }
            }

            for (int i = 1; i < m_lstAudioSourceBeingPlayed.Count; i++)
            {
                var audioSource = m_lstAudioSourceBeingPlayed[i];
                if (!audioSource.isPlaying)
                {
                    if (audioSource != m_audioSource)
                    {
                        RecycleOldAudioSource(audioSource);
                    }
                    
                    i--;
                }
            }
        }

        private struct AudioSegmentsBeingPlayed
        {
            AudioSource m_audioSource;
            public AudioSource AudioSource
            {
                get
                {
                    return m_audioSource;
                }
            }

            AudioClip m_audioClip;
            public AudioClip AudioClip
            {
                get
                {
                    return m_audioClip;
                }
            }
            private float m_fStartTime;
            public float StartTime
            {
                get { return m_fStartTime; }
                set { m_fStartTime = value; }
            }

            public float ElapsedTime
            {
                get
                {
                    return Time.time - StartTime;
                }
            }

            private float m_fDuration;
            public float Duration
            {
                get { return m_fDuration; }
                //set { m_fDuration = value; }
            }

            public AudioSegmentsBeingPlayed(AudioSource a_audioSource, float a_fDuration)
            {
                m_audioSource = a_audioSource;
                m_audioClip = a_audioSource.clip;
                m_fStartTime = Time.time;
                m_fDuration = a_fDuration;
            }

            public override string ToString()
            {
                return string.Format("Audio: '{0}', Clip: '{1}', start: {2}, Duration: {3}", AudioSource, AudioClip, StartTime, Duration);
            }
        }

        [SerializeField]
        private List<AudioSegmentsBeingPlayed> m_lstSegmentsBeingPlayed = new List<AudioSegmentsBeingPlayed>();
        private List<AudioSegmentsBeingPlayed> SegmentsBeingPaid
        {
            get
            {
                return m_lstSegmentsBeingPlayed;
            }
        }
        #endregion

        public void AddNewFileEntryPrivate(AudioFileEntry audioFileEntry)
        {
            m_lstFileEntries.Add(audioFileEntry);
        }

        public List<AudioFileEntry> FileEntriesPrivate
        {
            get
            {
                return m_lstFileEntries;
            }
        }
    }
}