using UnityEngine;
using System.Collections;
using SillyGames.SGBase.TaskSystem;
using System.Collections.Generic;
using System;
using System.Collections.ObjectModel;

namespace SillyGames.SGBase.TaskSystem
{
    public class ObjectEnableDisableTask : Task
    {
        [SerializeField]
        public List<ObjectEnableDisable> m_Objects = null;

        protected override void OnStart()
        {
            foreach (var obj in m_Objects)
            {
                var component = obj.UnityObject as MonoBehaviour;
                
                if (component != null)
                { 
                    if (obj.IncludeInTask)
                    {
                        component.enabled = obj.Enable;
                    }
                }
                else
                {
                    var testObj = obj.UnityObject as GameObject;
                    if ( testObj != null )
                    {
                        if (obj.IncludeInTask)
                        {
                            testObj.SetActive(obj.Enable);
                        }
                    }
                }
            }
            Stop();
        }
    }

    [Serializable]
    public class ObjectEnableDisable
    {
        [SerializeField]
        private UnityEngine.Object m_UnityObject = null;

        public UnityEngine.Object UnityObject
        {
            get
            {
                return m_UnityObject;
            }
            set
            {
                m_UnityObject = value;
            }
        }

        [SerializeField]
        private bool m_ignoreFromTask = true;

        public bool IncludeInTask
        {
            get
            {
                return !m_ignoreFromTask;
            }
            set
            {
                m_ignoreFromTask = !value;
            }
        }

        [SerializeField]
        private bool m_enable = false;

        public bool Enable
        {
            get
            {
                return m_enable;
            }
            set
            {
                m_enable = value;
            }
        }
    }
}
