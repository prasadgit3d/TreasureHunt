using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utilities : MonoBehaviour
{

    private static Utilities s_instnace = null;
    public static Utilities Instance
    {
        get
        {
            if (s_instnace == null)
            {
                var go = new GameObject("Utilities", typeof(Utilities));
                s_instnace = go.GetComponent<Utilities>();
            }
            return s_instnace;
        }
    }

    public static void WaitAndCall<T>(float a_fWaitDuration, Action<T> a_callback, T a_value)
    {
        Instance.StartCoroutine(WaitAndCallInternal<T>(a_fWaitDuration,a_callback,a_value));
    }

    private static IEnumerator WaitAndCallInternal<T>(float a_fWaitDuration, Action<T> a_callback, T a_value)
    {
        yield return new WaitForSeconds(a_fWaitDuration);
        a_callback(a_value);
    }

}
