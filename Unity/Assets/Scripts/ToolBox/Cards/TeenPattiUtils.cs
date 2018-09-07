using SillyGames.SGBase.CardUtils;
using System;
namespace SillyGames.SGBase.TeenPattiUtils
{
    public static class TeenPatti
    {
        #region TeenPatti hand evaluation constants

        /*
        Three of a Kind	5	96-100
        Pure Sequence	5	91-95
        Sequence	    8	83-90
        Colour	        8	75-82
        Pair	        14	61-74
        High Card	    60	0-60
        */

        ///this addition and subtraction of 0.01f is just to make sure higher hands always have a bigger value
        ///for exmaple Highest High card is lower than lowest pair

        const float HAND_WEIGHT_HIGHCARD = 0.60f;
        const float HAND_WEIGHT_PAIR = 0.14f - 0.001f;
        const float HAND_WEIGHT_COLOR = 0.08f - 0.001f;
        const float HAND_WEIGHT_SEQUENCE = 0.08f - 0.001f;
        const float HAND_WEIGHT_PURESEQUENCE = 0.05f - 0.001f;
        const float HAND_WEIGHT_THREEOFAKIND = 0.05f - 0.001f;

        const float STRENGHT_START_HIGHCARD = 0.0f;
        const float STRENGHT_START_PAIR = STRENGHT_START_HIGHCARD + HAND_WEIGHT_HIGHCARD + 0.001f;
        const float STRENGHT_START_COLOR = STRENGHT_START_PAIR + HAND_WEIGHT_PAIR + 0.001f;
        const float STRENGHT_START_SEQUENCE = STRENGHT_START_COLOR + HAND_WEIGHT_COLOR + 0.001f;
        const float STRENGHT_START_PURESEQUENCE = STRENGHT_START_SEQUENCE + HAND_WEIGHT_SEQUENCE + 0.001f;
        const float STRENGHT_START_THREEOFAKIND = STRENGHT_START_PURESEQUENCE + HAND_WEIGHT_PURESEQUENCE + 0.001f;

        #endregion
        public enum eTeenPattiHand
        {
            None,
            Draw = None,
            HighCard,
            Pair,
            Color,
            Sequence,
            PureSequence,
            ThreeOfAKind
        }

        // Slots for players on the table
        public enum Slot
        {
            None = 0,
            A = 1,
            B = 2,
            C = 3,
            D = 4,
            E = 5,
            Maximum
        }

        /// <summary>
        /// returns the hand strength for the given hand, the value is between 0 (inclusive) to 1 (inclusive)
        /// does not check if all the three cards belongs to one logical deck, it just calculate the strength based on input
        /// returns 0 if the cards are 2,3,5 of different color
        /// returns 1 if all the cards are As irrespective of the color
        /// </summary>
        /// <param name="a_1"></param>
        /// <param name="a_2"></param>
        /// <param name="a_3"></param>
        /// <returns>normalized value for strength of a card</returns>
        public static float getHandStrength(Card a_1, Card a_2, Card a_3)
        {
            float fHandStrength = 0.0f;
            var handType = getHandType(a_1, a_2, a_3);
            switch (handType)
            {
                case eTeenPattiHand.HighCard:
                    {
                        fHandStrength = STRENGHT_START_HIGHCARD + (HAND_WEIGHT_HIGHCARD * getStrength_HighCard(a_1.CardValue, a_2.CardValue, a_3.CardValue));
                        break;
                    }
                case eTeenPattiHand.Pair:
                    {
                        fHandStrength = STRENGHT_START_PAIR + (HAND_WEIGHT_PAIR * getStrength_Pair(a_1.CardValue, a_2.CardValue, a_3.CardValue));
                        break;
                    }
                case eTeenPattiHand.Color:
                    {
                        fHandStrength = STRENGHT_START_COLOR + (HAND_WEIGHT_COLOR * getStrength_HighCard(a_1.CardValue, a_2.CardValue, a_3.CardValue));
                        break;
                    }
                case eTeenPattiHand.Sequence:
                    {
                        fHandStrength = STRENGHT_START_SEQUENCE + (HAND_WEIGHT_SEQUENCE * getStrength_Sequence(a_1.CardValue, a_2.CardValue, a_3.CardValue));
                        break;
                    }
                case eTeenPattiHand.PureSequence:
                    {
                        fHandStrength = STRENGHT_START_PURESEQUENCE + (HAND_WEIGHT_PURESEQUENCE * getStrength_Sequence(a_1.CardValue, a_2.CardValue, a_3.CardValue));
                        break;
                    }
                case eTeenPattiHand.ThreeOfAKind:
                    {
                        fHandStrength = STRENGHT_START_THREEOFAKIND + (HAND_WEIGHT_THREEOFAKIND * getStrength_ThreeOfAKind(a_1.CardValue));
                        break;
                    }
                default:
                    {
                        //we should never arrive here, since all cases for the enum value were 
                        //handled in switch case above at the time of writing this
                        throw new Exception("Unexpected Hand Type whilie getting Hand strength: " + handType);

                    }
            }
            return fHandStrength;
        }

        /// <summary>
        /// Determines the Hand type based on the input
        /// Expecting valid cards (its cardsuit and cardvalue should have a valid values)
        /// this method does not check if these three cards logically belongs to a common deck
        /// </summary>
        /// <param name="a_1"></param>
        /// <param name="a_2"></param>
        /// <param name="a_3"></param>
        /// <returns></returns>
        public static eTeenPattiHand getHandType(Card a_1, Card a_2, Card a_3)
        {
            if (isItThreeOfAKind(a_1.CardValue, a_2.CardValue, a_3.CardValue))
            {
                return eTeenPattiHand.ThreeOfAKind;
            }
            if (Card.isItPureSequence(a_1, a_2, a_3))
            {
                return eTeenPattiHand.PureSequence;
            }
            if (Card.isItSequence(a_1.CardValue, a_2.CardValue, a_3.CardValue))
            {
                return eTeenPattiHand.Sequence;
            }

            if (Card.isItSameColor(a_1.CardSuit, a_2.CardSuit, a_3.CardSuit))
            {
                return eTeenPattiHand.Color;
            }
            if (isItPair(a_1.CardValue, a_2.CardValue, a_3.CardValue))
            {
                return eTeenPattiHand.Pair;
            }

            return eTeenPattiHand.HighCard;
        }

        /// <summary>
        /// checks if the values are three of a kind
        /// expects card values to be valid
        /// </summary>
        /// <param name="a_eValue1"></param>
        /// <param name="a_eValue2"></param>
        /// <param name="a_eValue3"></param>
        /// <returns>true if all the values are same, false otherwise</returns>
        public static bool isItThreeOfAKind(eCardValue a_eValue1, eCardValue a_eValue2, eCardValue a_eValue3)
        {
            return a_eValue1 == a_eValue2 && a_eValue1 == a_eValue3;
        }

        /// <summary>
        /// check if the card values provided contains a pair
        /// internally calls <see cref="getPairIfExists(eCardValue,eCardValue,eCardValue)"/>
        /// </summary>
        /// <param name="a_eValue1"></param>
        /// <param name="a_eValue2"></param>
        /// <param name="a_eValue3"></param>
        /// <returns>true if there exists a pair of cards, false otherwise</returns>
        public static bool isItPair(eCardValue a_eValue1, eCardValue a_eValue2, eCardValue a_eValue3)
        {
            return getPairIfExists(a_eValue1, a_eValue2, a_eValue3) != eCardValue.None;
        }

        /// <summary>
        /// lets us determine whther the cards proded contains a pair
        /// </summary>
        /// <param name="a_eValue1"></param>
        /// <param name="a_eValue2"></param>
        /// <param name="a_eValue3"></param>
        /// <returns>valid card value if the cards provided contined pair, none otherwise</returns>
        public static eCardValue getPairIfExists(eCardValue a_eValue1, eCardValue a_eValue2, eCardValue a_eValue3)
        {
            if (a_eValue1 == a_eValue2)
            {
                return a_eValue1;
            }
            if (a_eValue1 == a_eValue3)
            {
                return a_eValue1;
            }
            if (a_eValue2 == a_eValue3)
            {
                return a_eValue2;
            }
            return eCardValue.None;
        }

        public static float getStrength_ThreeOfAKind(eCardValue a_eCardValue)
        {
            return (float)(a_eCardValue) / 12.0f;
        }

        public static float getStrength_Sequence(eCardValue a_eValue1, eCardValue a_eValue2, eCardValue a_eValue3)
        {
            var sortedValues = Card.GetCardsSortedByValue(a_eValue1, a_eValue2, a_eValue3);
            var finalValue = sortedValues[1];
            if (sortedValues[0] == eCardValue._2 && sortedValues[2] == eCardValue.A)
            {
                finalValue = eCardValue._2;
            }

            if (finalValue != eCardValue.A)
            {
                switch (finalValue)
                {

                    case eCardValue.K:
                        {
                            return 12.0f / 12.0f;
                        }
                    case eCardValue._2:
                        {
                            return 11.0f / 12.0f;
                        }
                    default:
                        {
                            return ((int)finalValue - 1) / 12.0f;
                        }
                }
            }
            return 0.0f;
        }

        /// <summary>
        /// gets the strength of a 3 card hand which has a pair in it
        /// since there are 13 values in a suit and pair hand must accompanied by an odd card, there are 13 possibilites for the a each pair (ignoring suits)
        /// for example AA2, AA3, AA4 .... AAK, 
        /// this way total pairs in a deck (of 52 cards) are 13 * 13 = 169
        /// hence we need to make sure, the AAK should evaluate to 156, and 223 to 0 
        /// </summary>
        /// <param name="a_ePairValue"></param>
        /// <param name="a_eOddValue"></param>
        /// <returns></returns>
        public static float getStrength_Pair(eCardValue a_ePairValue, eCardValue a_eOddValue)
        {
            //int iMultiplier = (int)a_ePairValue;

            //int iAdder = (int)a_eOddValue - (a_eOddValue < a_ePairValue ? 0 : 1);
            //return ((iMultiplier * 12) + iAdder) / 156.0f;

            ///internally cardvalue 2 is treated as 0, 3 as 1, 4 as 2 and so on, till K as 11 and A as 12
            ///multiplying by 13 is because its base 13 number system here

            //
            const float iLOWEST_PAIR_VALUE = ((int)eCardValue._2 * 13) + ((int)eCardValue._3); //2 
            const float iHighest_PAIR_VALUE = ((int)eCardValue.A * 13) + ((int)eCardValue.K);//168


            ///here the interpolation will be done from 0 to iHighest_HIGHCARD_VALUE_ADJUSTED
            const float iHighest_PAIR_VALUE_ADJUSTED = iHighest_PAIR_VALUE - iLOWEST_PAIR_VALUE;

            int iCardValue = ((int)a_ePairValue * 13) + ((int)a_eOddValue);
            return (iCardValue - iLOWEST_PAIR_VALUE) / iHighest_PAIR_VALUE_ADJUSTED;
        }

        public static float getStrength_Pair(eCardValue a_eValue1, eCardValue a_eValue2, eCardValue a_eValue3)
        {
            var pairCardValue = getPairIfExists(a_eValue1, a_eValue2, a_eValue3);
            if (pairCardValue != eCardValue.None)
            {
                //var oddCardValue = pairCardValue != a_eValue1 ? a_eValue1 : (pairCardValue != a_eValue2 ? a_eValue2 : (pairCardValue != a_eValue3 ? a_eValue3 : ECardValue.None));
                var oddCardValue = pairCardValue != a_eValue1 ? a_eValue1 : (pairCardValue != a_eValue2 ? a_eValue2 : a_eValue3);
                return getStrength_Pair(pairCardValue, oddCardValue);
            }
            throw new Exception("GetStrength_Pair returning unexpectedly, input: 1:" + a_eValue1 + ", 2:" + a_eValue2 + ", 3: " + a_eValue3);
        }

        /// <summary>
        /// gets the strength of a 3 card hand assuming its a high card hand
        /// Highest card assumed is AKJ, while the lowest card assumed is 532
        /// internally this function uses base 13 number system to assign each hand a unique number
        /// </summary>
        /// <param name="a_eValue1"></param>
        /// <param name="a_eValue2"></param>
        /// <param name="a_eValue3"></param>
        /// <returns></returns>
        public static float getStrength_HighCard(eCardValue a_eValue1, eCardValue a_eValue2, eCardValue a_eValue3)
        {
            ///internally cardvalue 2 is treated as 0, 3 as 1, 4 as 2 and so on, till K as 11 and A as 12
            ///multiplying by 13 is because its base 13 number system
            const float iLOWEST_HIGHCARD_VALUE = ((int)eCardValue._5 * 13 * 13) + ((int)eCardValue._3 * 13) + ((int)eCardValue._2); //520
            const float iHighest_HIGHCARD_VALUE = ((int)eCardValue.A * 13 * 13) + ((int)eCardValue.K * 13) + ((int)eCardValue.J);//2180
            ///here the interpolation will be done from 0 to iHighest_HIGHCARD_VALUE_ADJUSTED
            const float iHighest_HIGHCARD_VALUE_ADJUSTED = iHighest_HIGHCARD_VALUE - iLOWEST_HIGHCARD_VALUE;

            var sortedCards = Card.GetCardsSortedByValue(a_eValue1, a_eValue2, a_eValue3);

            int iCardValue = 0;
            var iNumberOfCards = sortedCards.Length;
            for (int i = 0; i < iNumberOfCards; i++)
            {
                int iConverted = (int)sortedCards[i];
                iCardValue += (iConverted) * (int)Math.Pow(13, i);
            }

            return (iCardValue - iLOWEST_HIGHCARD_VALUE) / iHighest_HIGHCARD_VALUE_ADJUSTED;
        }

        public static Card GetThirdBestCard(Card a_card1, Card a_card2)
        {
            ///initial i was thinking of an algorithm where these two cards are checked agains all the possible hands,
            ///checking from the strongest hand to weakest hand, but then i found it would be very long and unnecessarily 
            ///complecated, so then i though its rather better idea to go good old brute force method, which anyways should not affect the
            ///server performance considerably
            
            Card thirdCard = new Card();
            float fHandStrength = 0.0f;
            for (eCardSuit suit = eCardSuit.Clubs; suit <= eCardSuit.Spades; suit++)
            {
                for (eCardValue value = eCardValue._2; value <= eCardValue.A; value++)
                {
                    Card newCard= new Card(value,suit);
                    float fNewHandStrength = getHandStrength(a_card1,a_card2,newCard);
                    if(fNewHandStrength > fHandStrength)
                    {
                        fHandStrength = fNewHandStrength;
                        thirdCard = newCard;
                    }
                }
            }
            return thirdCard;
        }

        public static Card[] GetBestThreeCards(params Card[] a_cards)
        {
            Card[] finalCards = new Card[3];
            float fHandStrength = 0.0f;

            ///need to make sure there are atleast 3 cards provided
            System.Diagnostics.Debug.Assert(a_cards.Length >= 3);
            
            ///combination of n Cards, choosing r (in our case 3) at a time, where order is not important
            ///  n! / ((r!)*((n-r)!) )
            ///once a combination is retrieved, check for its strength and store if greater

            int n = a_cards.Length;
            for (int i = 0; i < n; i++)
            {
                for (int j = i+1 ; j < n; j++)
                {
                    for (int k = j+1; k < n; k++)
                    {
                        Card card1 = a_cards[i];
                        Card card2 = a_cards[j];
                        Card card3 = a_cards[k];
                        float fStrength = getHandStrength(card1,card2,card3);
                        if(fStrength > fHandStrength)
                        {
                            fHandStrength = fStrength;
                            finalCards[0] = card1;
                            finalCards[1] = card2;
                            finalCards[2] = card3;
                        }
                    }
                }
            }
            return finalCards;
        }

        /// <summary>
        /// given a hand and a trump card, it calculates and returns the biggest possible hand from input
        /// </summary>
        /// <param name="a_hand">input hand from which best possible hand needs to be deduced</param>
        /// <param name="a_trumpCardValue">card value that will act as a trump value in the hand</param>
        /// <param name="a_returnedHand">output will be stored here, the array will always have the result</param>
        /// <returns>true if the hand was improved using a trump card value, false alstherwise</returns>
        public static bool GetBestHandForTrumpCardValue(Card[] a_hand, eCardValue a_trumpCardValue, ref Card[] a_returnedHand  )
        {
            ///since a teenpatti hand is expected here, asertaining the same
            System.Diagnostics.Debug.Assert(a_hand.Length == 3, "Expecting a hand with exactly 3 cards, got: " + a_hand.Length);
            System.Diagnostics.Debug.Assert(a_returnedHand.Length >= 3, "Expecting output array to be atleast 3");

            int iTotalTrumCardsInHand = 0;
            eCardValue oddCardValue = eCardValue.None;
            //int iTrumpCardIndex = -1;
            for (int i = 0; i < a_hand.Length; i++)
            {
                var card = a_hand[i];
                if(card.CardValue == a_trumpCardValue)
                {
                    iTotalTrumCardsInHand++;
                    //iTrumpCardIndex = i;
                }
                else
                {
                    oddCardValue = card.CardValue;
                }
            }
            
            switch(iTotalTrumCardsInHand)
            {
                case 3:
                    {
                        ///since all cards are trump cards, return best hand, that is 3 of a kind of Ace
                        for (int i = 0; i < 3; i++)
                        {
                            a_returnedHand[i] = new Card(eCardValue.A, a_returnedHand[i].CardSuit);
                        }
                        return true;
                    }
                case 2:
                    {
                        ///since all but one cards are trump, we create best hand with a odd card in it
                        ///so it should be 3 of a kind of the odd card value
                        
                        for (int i = 0; i < 3; i++)
                        {
                            a_returnedHand[i] = new Card(oddCardValue, a_returnedHand[i].CardSuit);
                        }
                        return true;
                    }
                case 1:
                    {
                        int iThirdCardIndex = -1;
                        bool bFirstOddCardAssigned = false;
                        var firstCard = new Card();
                        var secondCard = new Card();
                        for (int i = 0; i < 3; i++)
                        {
                            if(a_hand[i].CardValue != a_trumpCardValue)
                            {
                                if(!bFirstOddCardAssigned)
                                {
                                    firstCard = a_hand[i];
                                    bFirstOddCardAssigned = true;
                                }
                                else
                                {
                                    secondCard = a_hand[i];
                                }
                            }
                            else
                            {
                                iThirdCardIndex = i;
                            }
                        }
                        ///since there is just one trump card, find the best hand possible woth rest 2 cards
                        var thirdBestCard = GetThirdBestCard(firstCard,secondCard);
                        
                        for (int i = 0; i < 3; i++)
                        {
                            if (iThirdCardIndex != i)
                            {
                                a_returnedHand[i] = a_hand[i];
                            }
                        }
                        a_returnedHand[iThirdCardIndex] = thirdBestCard;

                        return true;
                    }
                default:
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            a_returnedHand[i] = a_hand[i];
                        }
                        return false;
                    }
            }
        }
    }
}