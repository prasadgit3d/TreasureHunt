using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SillyGames.SGBase.UI
{
    public class UISlider : Slider
    {        
        [SerializeField]
        private float m_minValue = 0.0f;
        
        [SerializeField]
        private float m_maxValue = 1.0f;

        [Range(0.0f, 1.0f)]
        [SerializeField]
        private float m_thumbPosition = 0.0f;

        [Range(0.0f, 1.0f)]
        [SerializeField]
        private float m_thumbSize;
        
        public float MinValue
        {
            get
            {
                return m_minValue;
            }
            set
            {
                if (m_minValue != value)
                {
                    m_minValue = value;
                    OnMinValueChanged();
                    RaiseEditorCallbacks(SliderEvents.OnMinValueChanged);
                }                
            }
        }
        
        public float MaxValue
        {
            get
            {
                return m_maxValue;
            }
            set
            {
                if (m_maxValue != value)
                {
                    m_maxValue = value;
                    OnMaxValueChanged();
                    RaiseEditorCallbacks(SliderEvents.OnMaxValueChanged);
                }
            }
        }
        
        public float ThumbPosition
        {
            get { return m_thumbPosition; }
            set 
            {
                if (m_thumbPosition != value)
                {
                    m_thumbPosition = value;
                    OnThumbPositionChanged();
                    RaiseEditorCallbacks(SliderEvents.OnThumbPositionChanged);
                }
            }
        }

        public float ThumbSize
        {
            get { return m_thumbSize; }
            set 
            {
                if (m_thumbSize != value)
                {
                    m_thumbSize = value;
                    OnThumbSizeChanged();
                    RaiseEditorCallbacks(SliderEvents.OnThumbSizeChanged);
                }
            }
        }
        
        private void RaiseEditorCallbacks(SliderEvents a_event)
        {
            UnityEvent unityEvent;
            if(m_dicEditorCallbacks.TryGetValue(a_event, out unityEvent))
            {
                unityEvent.Invoke();
            }
        }

        [SerializeField]
        private EditorCallbacks[] m_sliderCallbacks = null;
        private Dictionary<SliderEvents, UnityEvent> m_dicEditorCallbacks = new Dictionary<SliderEvents, UnityEvent>();

        protected override void Start()
        {
            base.Start();
            if(m_sliderCallbacks != null)
            {
                foreach (var item in m_sliderCallbacks)
                {
                    m_dicEditorCallbacks[item.m_eventType] = item.m_eventCalbacks;
                }
            }
        }

        protected virtual void OnMinValueChanged() { }
        protected virtual void OnMaxValueChanged() { }
        protected virtual void OnThumbPositionChanged() { }
        protected virtual void OnThumbSizeChanged() { }
    }

    internal enum SliderEvents
    {
        None,
        OnMinValueChanged,
        OnMaxValueChanged,
        OnThumbPositionChanged,
        OnThumbSizeChanged
    }

    [Serializable]
    internal class EditorCallbacks
    {
        public SliderEvents m_eventType = SliderEvents.None;
        public UnityEvent m_eventCalbacks = null;
    }
    
}