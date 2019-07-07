using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SillyGames.TreasureHunt.Networking
{
    public interface IServer
    {
        void LaunchHunt(string a_strHuntID, Action<HuntResponse, IHuntInstance> a_callback, string a_strDisplayTest = "");
        void BeginTheHunt(string a_strInstanceID, Action<HuntResponse> a_responseCallback);
    }

    public interface IHuntInstance
    {
        string InstanceID { get; set; }
        string DisplayText { get; }
        void OnCommand(Commands a_commandID, params object[] a_data);
    }

    public enum Commands
    {
        NONE = 0,
        LAUNCH = 1,
    }

    
    public struct HuntResponse
    {
        public enum ErrorCodes
        {
            SUCCESS = 100,
            NO_INSTANCE_FOUND = 101
        }
        public bool IsSuccess { get { return ErrorCode == ErrorCodes.SUCCESS; } }
        public string HuntID { get; set; }
        public string HuntName { get; set; }
        public ErrorCodes ErrorCode { get; set; }
    }
}


