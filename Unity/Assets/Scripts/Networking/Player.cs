using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SillyGames.TreasureHunt.Networking
{
    public interface IPlayer
    {
        string PlayerID { get; }
        string DisplayName { get; }
    }
}