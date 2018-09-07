using UnityEngine;
using System.Collections;

namespace SillyGames.SGBase
{
    /// <summary>
    /// helper class to make sure its gameobject doesnt destroy when scene changes (level loads )at runtime
    /// </summary>
    public class DontDestroyOnLevelLoad : MonoBehaviour
    {
        void Awake()
        {
            DontDestroyOnLoad(this);
        }
    }
}