using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SillyGames.SGBase.CardUtils;
using SillyGames.SGBase.PokerUtils;

public class HighCardTest : MonoBehaviour 
{
    [SerializeField]
    List<eCardValue> handCards = null;
    void OnGUI()
    {
        GUILayout.Label("Hand Strength is: " + Poker.GetNormalizedStrength_HighCard(handCards.ToArray()));
    }
}
