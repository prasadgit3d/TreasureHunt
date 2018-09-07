using UnityEngine;
using System.Collections.Generic;
using System;
using SillyGames.SGBase.Utilities;
using UnityEngine.EventSystems;
using System.Collections.ObjectModel;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SillyGames.SGBase.UI
{
    //[RequireComponent(typeof(EventTrigger))]
    [DisallowMultipleComponent]
    public class UIElement : MonoBehaviour
    {
        [HideInInspector]
        [SerializeField]
        private UIElement m_parent;

        private List<UIElement> m_lstChildren = new List<UIElement>();

        protected ReadOnlyCollection<UIElement> Children
        {
            get
            {
                return m_lstChildren.AsReadOnly();
            }
        }

        public UIElement Parent
        {
            get
            {
                return m_parent;
            }

            private set
            {
                if (m_parent != null)
                {
                    m_parent.RemoveChildren(this);
                }

                if (value != null)
                {
                    value.AddChild(this);
                }
                //transform.SetParent(value != null ? value.transform : null);
                m_parent = value;
            }
        }

        protected virtual void OnCustomInspector() { }

        public void OnCustomInspectorPrivate() {
            OnCustomInspector();
        }

        void Awake()
        {
            var parents = GetComponentsInParent<UIElement>();
            //looking for immidiate parent
            if (parents.Length > 1)
            {
                var parent = parents[1];
                this.Parent = parent;
            }
        }

        protected virtual void Start()
        {
            //var parents = GetComponentsInParent<UIElement>();
            //if (parents.Length == 1)
            //{
            //    //Debug.Log("Started Calculating Children with: " + name);
            //    UpdateChildrenReferences();
            //}
            
            //OnDeactivatedStatusChanged(this);
        }

        //protected virtual void OnUnityEvent(BaseEventData a_data, EventTriggerType a_eTriggerType)
        //{
        //    //Debug.Log("got a unity Event: " + a_eTriggerType);
        //}

        protected virtual void OnEnable() 
        {
            //UnityEventsDisabled = m_bUnityEventsDisabled;
        }

        private void AddChild(UIElement a_childElement)
        {
            if (!m_lstChildren.Contains(a_childElement))
            {
                m_lstChildren.Add(a_childElement);
            }
        }

        private void RemoveChildren(UIElement a_childElement)
        {
            m_lstChildren.Remove(a_childElement);
        }

        //public void UpdateChildrenReferences()
        //{
        //    m_lstChildren.Clear();
        //    AssignChildren(transform);
        //}

        //private void AssignChildren(Transform a_tran)
        //{
        //    var childCount = a_tran.childCount;
        //    //Debug.Log(a_tran.name + " has ChildCount: " + childCount);
        //    for (int i = 0; i < childCount; i++)
        //    {
        //        //Debug.Log(a_tran.name + " is getting child with index: " + i);
        //        var child = a_tran.GetChild(i);
        //        var element = child.GetComponent<UIElement>();
        //        if (element != null)
        //        {
        //            //Debug.Log(name + " got a child called: " + child.name);
        //            element.Parent = this;
        //            element.UpdateChildrenReferences();
        //        }
        //        else
        //        {
        //            //Debug.Log(name + " will skip through Tranform  child: " + child.name);
        //            AssignChildren(child);
        //        }
        //    }
        //}

        //protected virtual Type GetCustomEditorType() { return typeof(UIElementEditor); }

        private void ClearExclusiveness()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) { return; }
#endif
            if (!string.IsNullOrEmpty(ExclusivenessContext))
            {
                List<UIElement> lstPrevContextUIs;
                if (s_dictExclusiveUI.TryGetValue(ExclusivenessContext, out lstPrevContextUIs))
                {
                    lstPrevContextUIs.Remove(this);
                }
                if(lstPrevContextUIs.Count > 0)
                {
                    var topMostUI = lstPrevContextUIs[lstPrevContextUIs.Count - 1];
                    if(!topMostUI.gameObject.activeSelf)
                    {
                        lstPrevContextUIs[lstPrevContextUIs.Count - 1].gameObject.SetActive(true);
                    }
                    
                }
                ExclusivenessContext = string.Empty;
            }
        }

        private void ActivateLastExlusiveObject()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying){return;}
#endif
            if(string.IsNullOrEmpty(ExclusivenessContext)){return;}

            List<UIElement> lstCurrentContextUIs;

            if (s_dictExclusiveUI.TryGetValue(ExclusivenessContext, out lstCurrentContextUIs))
            {
                if (lstCurrentContextUIs.Count > 0)
                {
                    lstCurrentContextUIs[0].gameObject.SetActive(true);
                }
            }
        }

        public virtual void Show()
        {
            ClearExclusiveness();
            OnShow();
        }

        protected virtual void OnShow()
        {
            gameObject.SetActive(true);
            IsShown = true;
        }

        public virtual void Hide()
        {
            ActivateLastExlusiveObject();
            ClearExclusiveness();
            OnHide();
        }

        protected virtual void OnHide()
        {
            gameObject.SetActive(false);
            IsShown = false;
        }

        public bool IsShown
        {
            get;
            protected set;
        }

        public void ShowExclusive(string a_strContext = DEFAULT_CONTEXT)
        {
            ClearExclusiveness();

            ExclusivenessContext = string.IsNullOrEmpty(a_strContext) ? DEFAULT_CONTEXT : a_strContext;

            List<UIElement> lstCurrentContextUIs;

            if (!s_dictExclusiveUI.TryGetValue(ExclusivenessContext, out lstCurrentContextUIs))
            {
                lstCurrentContextUIs = new List<UIElement>();
                s_dictExclusiveUI[ExclusivenessContext] = lstCurrentContextUIs;
            }
            for (int i = 0; i < lstCurrentContextUIs.Count; i++)
            {
                lstCurrentContextUIs[i].gameObject.SetActive(false);
            }
            lstCurrentContextUIs.Add(this);
            
            OnShow();
            
        }

        private string ExclusivenessContext { get; set; }

        private const string DEFAULT_CONTEXT = "global";
        private static Dictionary<string, List<UIElement>> s_dictExclusiveUI = new Dictionary<string, List<UIElement>>();
    }
}