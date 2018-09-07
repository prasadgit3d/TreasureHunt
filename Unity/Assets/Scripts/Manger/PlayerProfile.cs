using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerProfile
{
    private static PlayerProfile s_localPlayer = null;
	public static PlayerProfile LocalPlayer
    {
        get
        {
            return s_localPlayer;
        }
        set
        {
            s_localPlayer = value;
        }
    }

    [SerializeField]
    private string m_name = string.Empty;

    public string Name
    {
        get { return m_name; }
        set { m_name = value; }
    }

    [SerializeField]
    private string m_id = string.Empty;

    public string ID
    {
        get { return m_id; }
        set { m_id = value; }
    }

}
