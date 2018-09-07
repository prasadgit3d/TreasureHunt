using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHunt
{
    bool Visible { get; set; }
    void Init(Action<float> a_progress);
    void DeInit();
}
