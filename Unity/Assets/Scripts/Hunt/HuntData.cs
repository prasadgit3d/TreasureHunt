using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class HuntData
{
    public string DisplayName = string.Empty;
    public string ID = string.Empty;
    public string Description = string.Empty;
    public List<string> admins = new List<string>();
    public List<string> participants = new List<string>();

    public string TemplateID = string.Empty;

    public bool IsOwned
    {
        get
        {
            return admins.Contains(PlayerProfile.LocalPlayer.ID);
        }
    }

    [Flags]
    public enum Status
    {
        None = 0,
        AcceptingInvitations = 1<<0,
        Started = 1<<1,
    }

}
