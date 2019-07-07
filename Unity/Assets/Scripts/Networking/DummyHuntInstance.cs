using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SillyGames.TreasureHunt.Networking
{
    public class DummyHuntInstance : MonoBehaviour, IHuntInstance
    {
        public string InstanceID
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public string DisplayText { get; set; }

        public void OnCommand(Commands a_commandID, params object[] a_data)
        {
            throw new NotImplementedException();
        }
    }
}