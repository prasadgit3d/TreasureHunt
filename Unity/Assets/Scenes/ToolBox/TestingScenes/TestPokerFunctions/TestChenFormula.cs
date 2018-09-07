using UnityEngine;
using System.Collections;
using SillyGames.SGBase.PokerUtils;

internal class TestChenFormula : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    [SerializeField]
    TestPokerHands.Card_ m_card1 = null;

    [SerializeField]
    TestPokerHands.Card_ m_card2 = null;

    void OnGUI()
    {
        GUILayout.Label("Calculated Chen Formula Value: " + Poker.GetChenFormulaStrength(m_card1,m_card2) + ", Normalized: " + Poker.GetChenFormulaStrengthNormalized(m_card1, m_card2));
    }
}
