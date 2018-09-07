using UnityEngine;
using System;
using System.Collections.Generic;
using SillyGames.SGBase.CardUtils;
using SillyGames.SGBase.PokerUtils;

[ExecuteInEditMode]
internal class TestPokerHands : MonoBehaviour 
{

    [Serializable]
    internal class Card_
    {
        public eCardSuit suit;
        public eCardValue value;
        public static implicit operator Card_(Card a_card)
        {
            Card_ card = new Card_();
            card.suit = a_card.CardSuit;
            card.value = a_card.CardValue;
            return card;
        }

        public static implicit operator Card(Card_ a_card)
        {
            return new Card(a_card.value,a_card.suit);
        }
    }

    private int pressedIndex = -1;

    private int iSelectedSuit = -1;
    private int iSelectedValue = -1;
    private Rect rectModifyCard = new Rect(0, 0, 100, 350);
    internal static GUIContent[] suitContent = new GUIContent[] {   new GUIContent("♣"),
                                                    new GUIContent("♦"), 
                                                    new GUIContent("♥"), 
                                                    new GUIContent("♠") };

    internal static GUIContent[] valueContent = new GUIContent[] {   new GUIContent("2"),
                                                    new GUIContent("3"),
                                                    new GUIContent("4"),
                                                    new GUIContent("5"),
                                                    new GUIContent("6"),
                                                    new GUIContent("7"),
                                                    new GUIContent("8"),
                                                    new GUIContent("9"),
                                                    new GUIContent("10"),
                                                    new GUIContent("J"),
                                                    new GUIContent("Q"),
                                                    new GUIContent("K"),
                                                    new GUIContent("A")};

    [SerializeField]
    private List<Card_> handCards = new List<Card_>();

    private List<Card> tempList = new List<Card>();
    void OnGUI()
    {
        tempList.Clear();
        foreach (var item in handCards)
        {
            tempList.Add(item);
        }
        var hand = Poker.RetrieveBestHandCards(tempList.ToArray());

        GUILayout.Label("Calculated Best Hand: " + (hand.Cards.Length > 0? hand.HandType.ToString() : "---"));
        if(tempList.Count >= 2)
        {
            GUILayout.Label("Chen Formula Points for fist two cards: " + Poker.GetChenFormulaStrength(tempList[0],tempList[1]) +
                " (normalized value: " + Poker.GetChenFormulaStrengthNormalized(tempList[0], tempList[1]) + ")");
        }
        else
        {
            GUILayout.Label("Chen Formula is only applied on first Two Cards");
        }
        
        string handStrengthDescriptionRelative = "Relative Hand strength is: ";
        string handStrengthDescriptionAbsolute = "Absolute Hand Strength is: ";

        bool bAddNotEnoughCardsError = hand.Cards.Length < 5;
        if (!bAddNotEnoughCardsError)
        {
            if(hand.HandType == Poker.ePokerHand.None)
            {
                handStrengthDescriptionRelative += "--";
                handStrengthDescriptionAbsolute += "--";
            }
            else
            {
                handStrengthDescriptionRelative += hand.GetNormalizedRelativeStrength();
                handStrengthDescriptionAbsolute += hand.GetNormalizedStrength();
            }
        }
        
        if(bAddNotEnoughCardsError)
        {
            handStrengthDescriptionRelative += "(not enough cards!!)";
            handStrengthDescriptionAbsolute += "(not enough cards!!)";
        }
        GUILayout.Label(handStrengthDescriptionRelative);
        GUILayout.Label(handStrengthDescriptionAbsolute);
        GUILayout.BeginHorizontal();
        if (hand.Cards.Length > 0)
        {
            for (int i = 0; i < hand.Cards.Length; i++)
            {
                RenderCard(hand.Cards[i]);
            }
        }
        else
        {
            GUILayout.Label("Please provide some cards for Hand Evaluation");
        }
        GUILayout.EndHorizontal();
        GUILayout.Space(20);
        GUILayout.Label("Input Cards( " + handCards.Count+ ").. ");
        GUILayout.BeginHorizontal(GUI.skin.box);
        if (handCards.Count > 0)
        {
            for (int i = 0; i < handCards.Count; i++)
            {
                RenderCard(handCards[i], i);
            }
        }
        else
        {
            GUILayout.Label("---");
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(20);

        if (pressedIndex == -1)
        {
            DrawDeckButtons();
        }
        else
        {
            DrawPressedIndex();
        }

    }

    void RenderCard(Card_ a_card)
    {
        GUILayout.BeginVertical();
        GUILayout.Label(GetCardSuitString(a_card.suit) + ": " + GetCardValString(a_card.value), GUI.skin.box);
        GUILayout.EndVertical();
    }
    void RenderCard(Card_ a_card, int a_iIndex)
    {
        GUILayout.BeginVertical();
        var toggleVal = GUILayout.Toggle( a_iIndex == pressedIndex, GetCardSuitString(a_card.suit) + ": " + GetCardValString(a_card.value), GUI.skin.button,GUILayout.MinWidth(50));
        if(toggleVal)
        {
            if (pressedIndex != a_iIndex)
            {
                pressedIndex = a_iIndex;
                iSelectedSuit = -1;
                iSelectedValue = -1;
                
            }
        }
        else
        {
            if(pressedIndex == a_iIndex)
            {
                pressedIndex = -1;
                
            }
        }

        if (a_iIndex == pressedIndex)
        {
            var LastRect = GUILayoutUtility.GetLastRect();
            //Debug.Log(Event.current.type.ToString() + ", " + LastRect);
            if (Event.current.type == EventType.Repaint)
            {
                rectModifyCard.x = LastRect.x;
                rectModifyCard.y = LastRect.y + LastRect.height;
            }
        }
        GUILayout.EndVertical();
    }

    void DrawPressedIndex()
    {
        GUILayout.BeginArea(rectModifyCard);
        GUILayout.BeginHorizontal();
        iSelectedSuit = GUILayout.SelectionGrid(iSelectedSuit, suitContent, 1, GUILayout.Width(22));
        iSelectedValue = GUILayout.SelectionGrid(iSelectedValue, valueContent, 1, GUILayout.Width(22));
        GUILayout.EndHorizontal();
        GUILayout.EndArea();

        if (pressedIndex < handCards.Count)
        {
            if(iSelectedSuit != -1)
            {
                handCards[pressedIndex].suit = (eCardSuit)(iSelectedSuit+1);
            }
            if (iSelectedValue != -1)
            {
                handCards[pressedIndex].value = (eCardValue)(iSelectedValue);
            }
        }
        if(Event.current.type == EventType.MouseDown)
        {
            pressedIndex = -1;
        }
    }

    internal static string GetCardSuitString(eCardSuit suit)
    {
        string strSuitIcon = "?";
        switch (suit)
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
        return strSuitIcon;
    }
    
    string GetCardValString(eCardValue value)
    {
        string val = "?";
        switch (value)
        {
            case eCardValue._2:
            case eCardValue._3:
            case eCardValue._4:
            case eCardValue._5:
            case eCardValue._6:
            case eCardValue._7:
            case eCardValue._8:
            case eCardValue._9:
            case eCardValue._10:
                val = value.ToString().Trim('_');
                break;
            case eCardValue.J: val = "J"; break;
            case eCardValue.Q: val = "Q"; break;
            case eCardValue.K: val = "K"; break;
            case eCardValue.A: val = "A"; break;
        }
        
        return val;

    }

    CardDeck deck = new CardDeck((int)System.DateTime.Now.Ticks,true);
    private void DrawDeckButtons()
    {
        GUILayout.Label("Draw cards directly from the deck (Max 7,): " + handCards.Count + " out of 7");
        GUILayout.BeginHorizontal();
        if(GUILayout.Button("Reset Deck(" + deck.CardsRemaining() + ")"))
        {
            ResetDeck();
            
        }
        //if (GUILayout.Button("Shuffle Deck"))
        //{
        //    deck.Shuffle();
        //}
        if (GUILayout.Button("Clear All Cards"))
        {
            handCards.Clear();
        }

        if (GUILayout.Button("Randomize 7"))
        {
            handCards.Clear();
            if(deck.CardsRemaining() < 7)
            {
                ResetDeck();
            }
            DrawCardsFromDeckToHand(7);
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Draw 1"))
        {
            DrawCardsFromDeckToHand(1);
        }

        if (GUILayout.Button("Draw 2"))
        {
            DrawCardsFromDeckToHand(2);
        }
        if (GUILayout.Button("Draw 3"))
        {
            DrawCardsFromDeckToHand(3);
        }
        GUILayout.EndHorizontal();
    }

    private void ResetDeck()
    {
        deck.Collect();
        deck.Shuffle();
    }

    void DrawCardsFromDeckToHand(int iCount)
    {
        if (deck.CardsRemaining() < iCount)
        {
            ResetDeck();
        }

        AddCardsToHand(deck.Draw(Math.Min(iCount, 7 - handCards.Count)));
    }

    void AddCardsToHand(params Card[] a_cards)
    {
        foreach (var item in a_cards)
        {
            handCards.Add(item);
        }
    }
}
