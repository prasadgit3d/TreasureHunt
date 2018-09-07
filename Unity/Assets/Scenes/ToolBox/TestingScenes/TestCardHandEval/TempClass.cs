using UnityEngine;
using System.Collections;
using System;
using SillyGames.SGBase.CardUtils;
using SillyGames.SGBase.TeenPattiUtils;

public class TempClass : MonoBehaviour 
{
    
    void Start () 
    {
        m_card1.CardSuit = eCardSuit.Hearts;
        m_card1.CardValue = eCardValue._8;

        m_card2.CardSuit = eCardSuit.Clubs;
        m_card2.CardValue = eCardValue._14;

        m_card3.CardSuit = eCardSuit.Diamonds;
        m_card3.CardValue = eCardValue._7;
	}
	
	// Update is called once per frame
	void Update () {
        System.Diagnostics.Debug.Assert(false);
	}

    void OnGUI()
    {
        //GUILayout.Label("Card value A: " + GetStrength_ThreeOfAKind(ECardValue._8));
        GUILayout.BeginHorizontal();
        RenderCard(ref m_card1, "Card1");
        RenderCard(ref m_card2, "Card2");
        RenderCard(ref m_card3, "Card3");
        GUILayout.EndHorizontal();

        GUILayout.Space(10);
        
        bool bIsItThreeOfAKind = TeenPatti.isItThreeOfAKind(m_card1.CardValue,m_card2.CardValue,m_card3.CardValue);
        GUILayout.Label("Is it 'Three of a kind'?: " + bIsItThreeOfAKind + (bIsItThreeOfAKind ? " [Strength: " + TeenPatti.getStrength_ThreeOfAKind(m_card1.CardValue) + "]": ""));
        
        GUILayout.Label("Is it 'Same Color'?: " + Card.isItSameColor(m_card1.CardSuit, m_card2.CardSuit, m_card3.CardSuit));
        
        bool bIsItSequence = Card.isItSequence(m_card1.CardValue, m_card2.CardValue, m_card3.CardValue);
        GUILayout.Label("Is it 'Sequence'?: " + bIsItSequence + (bIsItSequence? "[Strength: " + TeenPatti.getStrength_Sequence(m_card1.CardValue, m_card2.CardValue, m_card3.CardValue) + "]":""));

        //GUILayout.Label("Is it 'A23'?: " + TeenPatti.IsIt_A23(m_card1.CardValue, m_card2.CardValue, m_card3.CardValue));
        GUILayout.Label("Is it 'Pure Sequence'?: " + Card.isItPureSequence(m_card1, m_card2, m_card3));
        
        bool bIsItPair = TeenPatti.isItPair(m_card1.CardValue, m_card2.CardValue, m_card3.CardValue);
        GUILayout.Label("Is it 'Pair'?: " + bIsItPair + 
            (bIsItPair?" [Pair of: " + TeenPatti.getPairIfExists(m_card1.CardValue, m_card2.CardValue, m_card3.CardValue) + "]" +
            " [Strength: " + TeenPatti.getStrength_Pair(m_card1.CardValue, m_card2.CardValue, m_card3.CardValue) + "]": ""));

        GUILayout.Label("High Card Strength: " + TeenPatti.getStrength_HighCard(m_card1.CardValue, m_card2.CardValue, m_card3.CardValue));

        GUILayout.Label("What hand is it: " + TeenPatti.getHandType(m_card1, m_card2, m_card3) + " [Strength: " + (TeenPatti.getHandStrength(m_card1,m_card2,m_card3)).ToString("00.000") + "]");
        //GUILayout.Label("" + TeenPattiUtils.iHighest_PAIR_VALUE);
    }
    [SerializeField]
    Card m_card1;
    [SerializeField]
    Card m_card2;
    [SerializeField]
    Card m_card3;

    void RenderCard(ref Card a_card, string a_strTittle)
    {
        GUILayout.BeginVertical(GUI.skin.box);
        {
            GUILayout.Label(a_strTittle);
            GUILayout.BeginHorizontal();
            {
                string strSuitIcon = string.Empty;
                switch (a_card.CardSuit)
                {
                    case eCardSuit.Clubs:
                        {
                            strSuitIcon = "♣";
                            break;
                        }
                    case eCardSuit.Diamonds:
                        {
                            strSuitIcon = "♦";
                            break;
                        }
                    case eCardSuit.Hearts:
                        {
                            strSuitIcon = "♥";
                            break;
                        }
                    case eCardSuit.Spades:
                        {
                            strSuitIcon = "♠";
                            break;
                        }
                }
                GUILayout.Label("Suit: [" + a_card.CardSuit + "] " + strSuitIcon, GUILayout.Width(120));
                a_card.CardSuit = (eCardSuit)GUILayout.HorizontalSlider((float)a_card.CardSuit, (float)eCardSuit.Clubs, (float)eCardSuit.Spades, GUILayout.Width(50));
                
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("Value: [" + a_card.CardValue + "]", GUILayout.Width(80));
                a_card.CardValue = (eCardValue)GUILayout.HorizontalSlider((float)a_card.CardValue, (float)eCardValue._2, (float)eCardValue.A, GUILayout.Width(90));
            }
            GUILayout.EndHorizontal();

        }
        GUILayout.EndVertical();
        //♣♠♦♥
    }

    
}
