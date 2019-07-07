using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SillyGames.TreasureHunt.Networking
{
    public class DummyServer : MonoBehaviour , IServer
    {
        #region IServer implementation

        void IServer.LaunchHunt(string a_strHuntID, Action<HuntResponse, IHuntInstance> a_callback, string a_strDisplayTest)
        {
            var newHunt = gameObject.AddComponent<DummyHuntInstance>();
            var response = new HuntResponse();
            response.HuntID = a_strHuntID;
            response.ErrorCode = HuntResponse.ErrorCodes.SUCCESS;
            m_lstHunts.Add(newHunt);
            newHunt.InstanceID = (m_lstHunts.Count + 1).ToString();
            newHunt.DisplayText = a_strDisplayTest;
            newHunt.OnCommand(Commands.LAUNCH);
            a_callback(response, newHunt);
        }

        void IServer.BeginTheHunt(string a_instanceID, Action<HuntResponse> a_responseCallback)
        {
            var hunt = Hunts.Find((instance) => { return instance.InstanceID == a_instanceID; });
            var response = new HuntResponse();
            if (hunt != null)
            {
                response.ErrorCode = HuntResponse.ErrorCodes.SUCCESS;
            }
            else
            {
                response.ErrorCode = HuntResponse.ErrorCodes.NO_INSTANCE_FOUND;
            }
            a_responseCallback(response);
        }

        #endregion

        private List<IHuntInstance> m_lstHunts = new List<IHuntInstance>();
        private List<IHuntInstance> Hunts { get { return m_lstHunts; } }
        
    }
}