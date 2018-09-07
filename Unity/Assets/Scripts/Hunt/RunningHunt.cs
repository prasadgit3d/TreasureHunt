using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SillyGames.TreasureHunt
{

    public class RunningHunt : MonoBehaviour
    {

       public enum State
        {
            WaitingToStart,
            Started,
            Ended,
        }

        public State CurrentState { get; private set; }

        public float ElapsedTime { get; private set; }


    }
}