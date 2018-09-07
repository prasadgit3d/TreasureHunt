using UnityEngine;
using System.Collections;
using SillyGames.SGBase;

public class THGameState : GameStateBase
{
    protected override void OnEnter()
    {
        base.OnEnter();
        Debug.Log("Entered state: " + GetType());
    }


}
