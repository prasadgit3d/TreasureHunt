using UnityEngine;
using System.Collections;
using System;

namespace SillyGames.SGBase
{
    public class Timer : MonoBehaviour
    {

        private static Timer s_instance = null;
        private static Timer Instance
        {
            get
            {
                if (s_instance == null)
                {
                    s_instance = GameObject.FindObjectOfType<Timer>();
                    if (s_instance == null)
                    {
                        s_instance = (new GameObject(typeof(Timer).Name)).AddComponent<Timer>();
                    }
                }
                return s_instance;
            }
            set
            {
                s_instance = value;
            }
        }
        void Awake()
        {
            if (Instance != null)
            {
                if (Instance != this)
                {
                    Debug.LogWarning("There seems to be multiple Timer Instances... this instance will be deleted: " + this);
                    Destroy(this);
                    return;
                }
            }
            else
            {
                Instance = this;
            }
        }

        public static void AddTimerEntry<ID>(float a_fTime, ID a_id, Action<ID> a_callback)
        {
            Timer.Instance.StartCoroutine(Wait<ID>(a_fTime, a_id, a_callback));
        }

        public static void AddTimerEntry(float a_fTime, Action a_callback)
        {
            Timer.Instance.StartCoroutine(Wait(a_fTime, a_callback));
        }

        static IEnumerator Wait<ID>(float a_fTime, ID a_id, Action<ID> a_callback)
        {
            float fStartTime = Time.realtimeSinceStartup;

            float fCurrentTime = Time.realtimeSinceStartup;
            while (fCurrentTime - fStartTime < a_fTime)
            {
                yield return new WaitForEndOfFrame();
                fCurrentTime = Time.realtimeSinceStartup;
            }
            a_callback(a_id);
        }

        static IEnumerator Wait(float a_fTime, Action a_callback)
        {
            float fStartTime = Time.realtimeSinceStartup;

            float fCurrentTime = Time.realtimeSinceStartup;
            while ((fCurrentTime - fStartTime) < a_fTime)
            {
                yield return new WaitForEndOfFrame();
                fCurrentTime = Time.realtimeSinceStartup;
            }
            a_callback();
        }

    }
}