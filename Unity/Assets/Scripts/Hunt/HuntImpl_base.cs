using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HuntImpl_base : MonoBehaviour, IHunt
{
    [SerializeField]
    private Canvas m_mainCanvas = null;

    protected Canvas MainCanvas { get; set; }
    public virtual bool Visible
    {
        get
        {
            return MainCanvas.enabled;
        }

        set
        {
            MainCanvas.enabled = value;
        }
    }

    public virtual void DeInit()
    {
        
    }

    public virtual void Init(Action<float> a_progress)
    {
        a_progress(1.0f);
    }
}
