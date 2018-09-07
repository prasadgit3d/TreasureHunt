using UnityEngine;
using System.Collections;
using SillyGames.SGBase.TaskSystem;
using System.Collections.Generic;
using System;
using System.Collections.ObjectModel;

using ObjectPool = SillyGames.SGBase.Utilities.UnityObjectPooler<UnityEngine.Object,int>;

namespace SillyGames.SGBase.TaskSystem
{
    public class ObjectInstantiatingTask: Task
    {
        [Tooltip("We can put a ref to a scene object as well which will get instantiated")]
        [SerializeField]
        private UnityEngine.Object m_prefabToInstantiate = null;

        [SerializeField]
        private int m_cacheSize = 0;
        public int CacheSize
        {
            get { return m_cacheSize; }
            set { m_cacheSize = value; }
        }

        private List<UnityEngine.Object> m_lstIntatiatedObjects = new List<UnityEngine.Object>();

        public void RecycleObject(UnityEngine.Object a_object)
        {
            ObjectPool.ReturnToPool(GetInstanceID(), a_object);
            m_lstIntatiatedObjects.Remove(a_object);
            OnObjectRecycled(a_object);
        }

        protected override void OnStart()
        {
            if(ObjectPool.GetBaseObject(GetInstanceID()) == null)
            {
                ObjectPool.SetBaseObject(GetInstanceID(), m_prefabToInstantiate);
                for (int i = 0; i < CacheSize; i++)
                {
                    var instantiatedObj = ObjectPool.RetrieveFromPool(GetInstanceID());
                    ObjectPool.ReturnToPool(GetInstanceID(), instantiatedObj);
                }
            }

            var currentInstantiatedObject = ObjectPool.RetrieveFromPool(GetInstanceID());
            m_lstIntatiatedObjects.Add(currentInstantiatedObject);
            OnObjectInstatiated(currentInstantiatedObject);
            Stop();
        }

        protected virtual void OnObjectInstatiated(UnityEngine.Object a_currentInstantiatedObject)
        {
            Debug.Log("Object Instantiated by task: " + this + " ... object: " + a_currentInstantiatedObject);    
        }

        protected virtual void OnObjectRecycled(UnityEngine.Object a_object)
        {
            Debug.Log("Object recycled to the task: " + this + " ... object: " + a_object);    
        }

    }
}
