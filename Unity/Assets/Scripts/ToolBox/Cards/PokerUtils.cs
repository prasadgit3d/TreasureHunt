using SillyGames.SGBase.CardUtils;
using System;
using System.Collections.Generic;

namespace SillyGames.SGBase.PokerUtils
{
    public static class Poker
    {
        /// <summary>
        /// these are the types of hands in poker
        /// arranged in assending order
        /// </summary>
        public enum ePokerHand
        {
            None,
            HighCard,
            OnePair,
            TwoPair,
            ThreeOfAKind,
            Straight,
            Sequence = Straight,
            Flush,
            Color = Flush,
            FullHouse,
            FourOfAKind,
            StraightFlush,
            PureSequence = StraightFlush,
            RoyalFlush
        }

        /// <summary>
        /// a helper struct to denote a poker hand 
        /// it is usefull to carry as a single entity rather than array of cards,
        /// it also helps to retrieve common information regarding the hand
        /// such as kicker cards, hand type, strength etc..
        /// </summary>
        public struct PokerHand
        {
            /// <summary>
            /// the cards that are part of this hand
            /// </summary>
            public Card[] Cards { get; set; }

            /// <summary>
            /// tells what type of hand is it
            /// </summary>
            public ePokerHand HandType { get; set; }

            internal eCardValue[] GetAllCardValues()
            {
               var cardValues = new eCardValue[Cards.Length];
               for (int i = 0; i < cardValues.Length; i++)
               {
                   cardValues[i] = Cards[i].CardValue;
               }
               return cardValues;
            }

            /// <summary>
            /// returns first card of the first pair in the hand
            /// this property assumes there are enough cards in the hand and 
            /// the hand type is either OnePair or TwoPair
            /// </summary>
            public Card FirstPairCard
            {
                get
                {
                    return Cards[0];
                }
            }

            /// <summary>
            /// returns first card of the first pair in the hand
            /// this property assumes there are enough cards in the hand and 
            /// the hand type is TwoPair
            /// </summary>
            public Card SecondPairCard
            {
                get
                {
                    return Cards[2];
                }
            }

            /// <summary>
            /// returns the kicker cards of the hand
            /// this property assumes there are enough cards in the hand and 
            /// the hand type is OnePair
            /// </summary>
            public Card[] KickerCards_OnePair
            {
                get
                {
                    return new Card[] { Cards[2], Cards[3], Cards[4] };
                }
            }

            /// <summary>
            /// returns the kicker card of the hand
            /// this property assumes there are enough cards in the hand and 
            /// the hand type is TwoPair
            public Card KickerCard_TwoPair
            {
                get
                {
                    return Cards[4];
                }
            }

            /// <summary>
            /// returns the kicker card of the hand
            /// this property assumes there are enough cards in the hand and 
            /// the hand type is FourOfAKind
            public Card KickerCard_FourOfAKind
            {
                get
                {
                    return Cards[4];
                }
            }

            /// <summary>
            /// returns the kicker cards of the hand
            /// this property assumes there are enough cards in the hand and 
            /// the hand type is ThreeOfAKind
            /// </summary>
            public Card[] KickerCards_ThreeOfAKind
            {
                get 
                {
                    return new Card[] { Cards[3], Cards[4] };
                }
            }

            /// <summary>
            /// returns first card of the three of a kind cards in the hand
            /// this property assumes there are enough cards in the hand and 
            /// the hand type is either ThreeOfAKind or FullHouse
            /// </summary>
            public Card ThreeOfAKindCard
            {
                get
                {
                    return Cards[0];
                }
            }

            /// <summary>
            /// returns first card of the four of a kind in the hand
            /// this property assumes there are enough cards in the hand and 
            /// the hand type is ThreeOfAKind
            /// </summary>
            public Card FourOfAKindCard
            {
                get
                {
                    return Cards[0];
                }
            }

            /// <summary>
            /// returns first card of the pair in the hand
            /// this property assumes there are enough cards in the hand and 
            /// the hand type is FullHouse
            public Card FullHousePairCard
            {
                get
                {
                    return Cards[3];
                }
            }

            /// <summary>
            /// determine the normalized value of the hand for the HandType
            /// for example the hand type is FourOfAKind, the normalized relative value (between 0 and 1)
            /// will tell you how good if you hand compared against lowest and highest FourOfAKind hands
            /// </summary>
            /// <returns>0 if the hand is smallest in the HandType, 1 if the hand is biggest in the HandType, 
            /// else lerped values bwetween the two</returns>
            public double GetNormalizedRelativeStrength()
            {
                if(HandType == ePokerHand.None)
                {
                    return 0;
                }

                double relativeStrength = 0.0;
                if(Cards.Length >= POKER_HAND_CARD_COUNT)
                {
                    switch (HandType)
                    {
                        case Poker.ePokerHand.HighCard:
                            {
                                relativeStrength = Poker.GetNormalizedStrength_HighCard(GetAllCardValues());
                                break;
                            }
                        case Poker.ePokerHand.OnePair:
                            {
                                var kickerCards = KickerCards_OnePair;
                                if (kickerCards.Length >= 3)
                                {
                                    relativeStrength = Poker.GetNormalizedStrength_OnePair(FirstPairCard.CardValue,
                                                                                                    kickerCards[0].CardValue,
                                                                                                    kickerCards[1].CardValue,
                                                                                                    kickerCards[2].CardValue);
                                }
                                break;
                            }
                        case Poker.ePokerHand.TwoPair:
                            {
                                var kickerCard = KickerCard_TwoPair;
                                relativeStrength = Poker.GetNormalizedStrength_TwoPair(FirstPairCard.CardValue,
                                                                                                SecondPairCard.CardValue,
                                                                                                kickerCard.CardValue);
                                break;
                            }
                        case Poker.ePokerHand.ThreeOfAKind:
                            {
                                var kickerCards = KickerCards_ThreeOfAKind;
                                if (kickerCards.Length >= 2)
                                {
                                    relativeStrength = Poker.GetNormalizedStrength_ThreeOfAKind(ThreeOfAKindCard.CardValue,
                                                                                                        kickerCards[0].CardValue,
                                                                                                        kickerCards[1].CardValue);
                                }
                                break;
                            }
                        case Poker.ePokerHand.RoyalFlush:
                        case Poker.ePokerHand.StraightFlush:
                        case Poker.ePokerHand.Straight:
                            {
                                relativeStrength = Poker.GetNormalizedStrength_Straight(Cards[0].CardValue, Cards[1].CardValue);
                                break;
                            }
                        case Poker.ePokerHand.Flush:
                            {
                                relativeStrength = Poker.GetNormalizedStrength_Flush(GetAllCardValues());
                                break;
                            }
                        case Poker.ePokerHand.FullHouse:
                            {
                                relativeStrength = Poker.GetNormalizedStrength_FullHouse(ThreeOfAKindCard.CardValue, FullHousePairCard.CardValue);
                                break;
                            }
                        case Poker.ePokerHand.FourOfAKind:
                            {
                                relativeStrength = Poker.GetNormalizedStrength_FourOfAKind(FourOfAKindCard.CardValue, KickerCard_FourOfAKind.CardValue);
                                break;
                            }
                        default:
                            break;
                    }
                }

                return relativeStrength;                
            }

            /// <summary>
            /// determin the absolute normalized value for the hand type
            /// the value will vary from lowest high card to highest straight flush (royal flush)
            /// </summary>
            /// <returns>value linearly interpolated between 0 and 1 (both inclusive)</returns>
            public double GetNormalizedStrength()
            {
                var relativeHandStrength = GetNormalizedRelativeStrength();
                return Poker.GetNormalizedStrength(HandType, relativeHandStrength);                
            }

            /// <summary>
            /// calcualtes the hand type based on current cards,
            /// this specially usefull if you create a new instance, and assign the cards manually
            /// </summary>
            public void CalculateHandType()
            {
                var hand = Poker.RetrieveBestHandCards(Cards);
                Cards = hand.Cards;
                HandType = hand.HandType;
            }

            /// <summary>
            /// helper function to create a Hand from the cards
            /// it determine the best hand type, and trims the extra cards
            /// </summary>
            /// <param name="a_cards">cards that are suppose to be a part of the hand</param>
            /// <returns></returns>
            public static PokerHand Create(params Card[] a_cards)
            {
                return Poker.RetrieveBestHandCards(a_cards);
            }
        }

        internal static double GetDefaultHandStrengthFraction(ePokerHand a_handType)
        {
            if(a_handType == ePokerHand.RoyalFlush)
            {
                a_handType = ePokerHand.StraightFlush;
            }
            return HandStrengths[(int)a_handType];
        }
        
        private static readonly List<double> HandStrengths = new List<double>() 
        {  
            0.0,    //None
            0.36,   //HighCard
            0.28,   //OnePair
            0.1,    //TwoPair
            0.08,   //ThreeOfAKind
            0.07,   //Straight
            0.05,   //Flush
            0.03,   //FullHouse
            0.02,   //FourOfAKind
            0.01,   //StraightFlush
        };

        internal static double GetNormalizedStrength(ePokerHand a_handType, double a_relativeStrenght)
        {
            if(a_handType == ePokerHand.RoyalFlush)
            {
                a_handType = ePokerHand.StraightFlush;
            }

            double actualFraction = GetDefaultHandStrengthFraction(a_handType);
            double adjustmentFraction = a_handType != ePokerHand.HighCard ? (actualFraction * 0.1) : 0.0;
            double d = a_handType != ePokerHand.HighCard ? (actualFraction * 0.9 * a_relativeStrenght) : (actualFraction * a_relativeStrenght);

            for (var i = (a_handType-1); i >= ePokerHand.HighCard; --i)
            {
                d += GetDefaultHandStrengthFraction(i);
            }
            return d+adjustmentFraction;
        }

        public static PokerHand RetrieveBestHandCards(params Card[] a_cards)
        {
            PokerHand hand = new PokerHand();

            //determine most common suit in the cards, and how many are they
            var suitData = RetrieveBestSuit(a_cards);

            ///since flush has higher value than straight, we retrieve flush first
            bool bIsFlushPossible = suitData.Count >= POKER_HAND_CARD_COUNT;

            if (bIsFlushPossible)
            {
                //get the suited cards
                var suitedCards = RetrieveCardsBySuit(suitData.Suit, a_cards);

                //check if the suited cards are straight, for straight flush
                var sequenceCards = RetrieveStraight(suitedCards);

                ///if sequence cards are non null, then its a valid sequence
                bool bIsSequence = sequenceCards != null;

                //if its sequence, check additionally if its royal flush
                if (bIsSequence)
                {
                    hand.Cards = sequenceCards;

                    if (IsRoyalFlush(sequenceCards))
                    {
                        hand.HandType = ePokerHand.RoyalFlush;
                    }
                    else
                    {
                        hand.HandType = ePokerHand.StraightFlush;
                    }
                }
                else
                {
                    ///now there are suited cards, just sort and trim them
                    var trimedCards = Card.SortCardsByValueDescending(suitedCards);
                    hand.Cards = trimedCards;
                    hand.HandType = ePokerHand.Flush;
                }
            }
            else
            {
                //since its not a flush, check for other hand types 

                a_cards = Card.SortCardsByValueDescending(a_cards);

                var bestValueData = RetrieveBestValue(a_cards);

                switch (bestValueData.Count)
                {
                    case (4):   //check for a 'four of a kind' hand
                        {
                            var fourOfAKindCards = RetrieveFourOfAKind(bestValueData.Value, a_cards);

                            ///here the cards are already either 4 or 5, so no need to trim
                            hand.Cards = fourOfAKindCards;
                            hand.HandType = ePokerHand.FourOfAKind;
                            break;
                        }
                    case (3):
                        {
                            //this will fail even in the case there are less than POKER_HAND_CARD_COUNT cards 
                            //since it will not get common 2 cards other than the 'bestValueData.Value'
                            var anyOtherPairCardValue = IsThereAnyOtherPair(bestValueData.Value, a_cards);
                            if (anyOtherPairCardValue != eCardValue.None)
                            {
                                var fullHouseCards = MakeFullHouse(bestValueData.Value, anyOtherPairCardValue, a_cards);
                                hand.Cards = fullHouseCards;
                                hand.HandType = ePokerHand.FullHouse;
                            }
                            else
                            {
                                //if it landed here, means, either there is just a 3 of a kind hand, of not enough (4 or 3) cards to form a full house

                                //check if its just a straight
                                var sequenceCards = RetrieveStraight(a_cards);

                                //if these are non null, that means, its a straight hand
                                if (sequenceCards != null)
                                {
                                    //here we dont need to sort the cards, since they are already sorted due to 'RetrieveSequence'
                                    hand.Cards = sequenceCards;
                                    hand.HandType = ePokerHand.Straight;
                                }
                                else
                                {
                                    var threeOfAKindcards = MakeThreeOfAKind(bestValueData.Value, a_cards);
                                    hand.Cards = threeOfAKindcards;
                                    hand.HandType = ePokerHand.ThreeOfAKind;
                                }
                            }
                            break;
                        }
                    case (2):
                        {
                            //check if its just a straight
                            var sequenceCards = RetrieveStraight(a_cards);

                            //if these are non null, that means, its a straight hand
                            if (sequenceCards != null)
                            {
                                //here we dont need to sort the cards, since they are already sorted due to 'RetrieveSequence'
                                hand.Cards = sequenceCards;
                                hand.HandType = ePokerHand.Straight;
                            }
                            else
                            {
                                //now here there may be single pair or two pairs
                                var anyOtherPairCardValue = IsThereAnyOtherPair(bestValueData.Value, a_cards);

                                if (anyOtherPairCardValue != eCardValue.None)
                                {
                                    var twoPairCards = MakeTwoPair(bestValueData.Value, anyOtherPairCardValue, a_cards);
                                    hand.Cards = twoPairCards;
                                    hand.HandType = ePokerHand.TwoPair;
                                }
                                else
                                {
                                    var onePairCards = MakeOnePair(bestValueData.Value, a_cards);
                                    hand.Cards = onePairCards;
                                    hand.HandType = ePokerHand.OnePair;
                                }
                            }
                            break;
                        }
                    default:
                        {
                            //check if its just a straight
                            var sequenceCards = RetrieveStraight(a_cards);

                            //if these are non null, that means, its a straight hand
                            if (sequenceCards != null)
                            {
                                //here we dont need to sort the cards, since they are already sorted due to 'RetrieveSequence'
                                hand.Cards = sequenceCards;
                                hand.HandType = ePokerHand.Straight;
                            }
                            else
                            {
                                ///so ther was not even a pair, so it should be a high card

                                hand.Cards = MakeHighCard(a_cards);
                                hand.HandType = ePokerHand.HighCard;
                            }
                        }
                        break;
                }
            }

            var handCards = hand.Cards;
            Array.Resize<Card>(ref handCards, Math.Min(hand.Cards.Length, POKER_HAND_CARD_COUNT));
            hand.Cards = handCards;
            return hand;
        }

        #region Chen Formula Points Retrieval
        
        private static readonly double m_minChenPoints = GetChenFormulaStrength(new Card(eCardValue._2,eCardSuit.Clubs),
                                                                        new Card(eCardValue._7,eCardSuit.Diamonds));
        private static readonly double m_maxChenPoints = GetChenFormulaStrength(new Card(eCardValue.A, eCardSuit.Clubs),
                                                                        new Card(eCardValue.A, eCardSuit.Diamonds));

        private static readonly double m_chenPointsDiff = m_maxChenPoints - m_minChenPoints;
        
        /// <summary>
        /// returns normalized value for the Cehn formula points
        /// <seealso cref="GetChenFormulaStrength"/>
        /// </summary>
        /// <param name="a_card1"></param>
        /// <param name="a_card2"></param>
        /// <returns></returns>
        public static double GetChenFormulaStrengthNormalized(Card a_card1, Card a_card2)
        {
            var tempValue = GetChenFormulaStrength(a_card1, a_card2);

            return (tempValue - m_minChenPoints) / m_chenPointsDiff;
        }

        public static int GetWinningCardCount(PokerHand a_pokerhand1, PokerHand a_pokerhand2)
        {
            var fStrength1 = a_pokerhand1.GetNormalizedStrength();
            var fStrength2 = a_pokerhand2.GetNormalizedStrength();

            if (fStrength1 == fStrength2)
            {
                return 0;
            }

            var sign = Math.Sign(fStrength1 - fStrength2);
            var fHighHand = fStrength1 > fStrength2 ? a_pokerhand1 : a_pokerhand2;
            var fLowHand = fStrength1 < fStrength2 ? a_pokerhand1 : a_pokerhand2;

            var finalValue = GetWinnigCardCountInternal(fHighHand, fLowHand);
            return (finalValue * sign);
        }

        private static int GetWinnigCardCountInternal(PokerHand a_pokerhandHigh, PokerHand a_pokerhandLow)
        {
            int iCardsToReturn = 0;
            switch (a_pokerhandHigh.HandType)
            {
                #region RoyalFlush, StraightFlush
                case ePokerHand.RoyalFlush:
                    {
                        //no matter what the other hand type is, royal flush and stright flush requires all the cards
                        iCardsToReturn = 5;
                        break;
                    }
                #endregion
                #region FourOfAKind
                case ePokerHand.FourOfAKind:
                    {
                        // if both hands are 4 of a kind
                        if (a_pokerhandLow.HandType == ePokerHand.FourOfAKind)
                        {
                            // send kicker only if both 4 of a kind are equal
                            if (a_pokerhandHigh.Cards[0].CardValue == a_pokerhandLow.Cards[0].CardValue)
                            {
                                iCardsToReturn = 5;
                            }
                            else
                            {
                                iCardsToReturn = 4;
                            }
                        }
                        else
                        {
                            iCardsToReturn = 4;
                        }
                        break;
                    }
                #endregion
                #region FullHouse
                case ePokerHand.FullHouse:
                    {
                        ///its a 5 card hand, so returning 5 for now..
                        iCardsToReturn = 5;
                        break;
                    }
                #endregion
                #region Flush
                case ePokerHand.Flush:
                    {
                        ///its a 5 card hand, so returning 5 for now..
                        iCardsToReturn = 5;
                        break;
                    }
                #endregion
                #region Straight
                case ePokerHand.Straight:
                    {
                        ///its a 5 card hand, so returning 5 for now..
                        iCardsToReturn = 5;
                        break;
                    }
                #endregion
                #region ThreeOfAKind
                case ePokerHand.ThreeOfAKind:
                    {
                        // if both hands are three of a kind
                        if (a_pokerhandLow.HandType == ePokerHand.ThreeOfAKind)
                        {
                            // send kicker only if both 3 of a kinds are equal
                            if (a_pokerhandHigh.Cards[0].CardValue == a_pokerhandLow.Cards[0].CardValue)
                            {
                                for (int i = 3; i < 5; i++)
                                {
                                    if (a_pokerhandLow.Cards[i].CardValue != a_pokerhandHigh.Cards[i].CardValue)
                                    {
                                        // found mismatched card, get out of loop
                                        iCardsToReturn = i + 1;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                iCardsToReturn = 3;
                            }
                        }
                        else
                        {
                            iCardsToReturn = 3;
                        }
                        break;
                    }
                #endregion
                #region TwoPair
                case ePokerHand.TwoPair:
                    {
                        // if both hads are two pairs
                        if (a_pokerhandLow.HandType == ePokerHand.TwoPair)
                        {
                            // send kicker only if both pair are equal
                            if (a_pokerhandLow.Cards[2].CardValue == a_pokerhandHigh.Cards[2].CardValue && a_pokerhandLow.Cards[0].CardValue == a_pokerhandHigh.Cards[0].CardValue)
                            {
                                iCardsToReturn = 5;
                            }
                            else
                            {
                                iCardsToReturn = 4;
                            }
                        }
                        else
                        {
                            iCardsToReturn = 4;
                        }
                        break;
                    }
                #endregion
                #region OnePair
                case ePokerHand.OnePair:
                    {
                        if (a_pokerhandLow.HandType == ePokerHand.OnePair)
                        {
                            if (a_pokerhandLow.Cards[0].CardValue == a_pokerhandHigh.Cards[0].CardValue)
                            {
                                for (int i = 2; i < 5; i++)
                                {
                                    if (a_pokerhandLow.Cards[i].CardValue != a_pokerhandHigh.Cards[i].CardValue)
                                    {
                                        ///we found mismatched card, just get out of the for loop
                                        iCardsToReturn = i + 1;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                iCardsToReturn = 2;
                            }
                        }
                        else
                        {
                            iCardsToReturn = 2;
                        }
                        break;
                    }
                #endregion
                #region HighCard
                case ePokerHand.HighCard:
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            if (a_pokerhandLow.Cards[i].CardValue != a_pokerhandHigh.Cards[i].CardValue)
                            {
                                ///we found mismatched card, just get out of the for loop
                                iCardsToReturn = i + 1;
                                break;
                            }
                        }
                        break;
                    }
                #endregion
                default:
                    {

                        break;
                    }
            }

            return iCardsToReturn;
        }

        #endregion

        /// <summary>
        /// calculate Chen formula points
        /// http://www.simplyholdem.com/chen.html
        /// the last step of rounding the points is not done here
        /// <seealso cref="GetChenFormulaStrengthNormalized"/>
        /// </summary>
        /// <param name="a_card1"></param>
        /// <param name="a_card2"></param>
        /// <returns></returns>
        public static double GetChenFormulaStrength(Card a_card1, Card a_card2)
        {
            var maxCardValue = a_card1.CardValue > a_card2.CardValue ? a_card1.CardValue : a_card2.CardValue;
            var fHighCardPoints = 0.0f;
            switch (maxCardValue)
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
                    fHighCardPoints = (int)(maxCardValue + 2) / 2.0f;
                    break;
                case eCardValue.J: fHighCardPoints = 6; break;
                case eCardValue.Q: fHighCardPoints = 7; break;
                case eCardValue.K: fHighCardPoints = 8; break;
                case eCardValue.A: fHighCardPoints = 10; break;
            }

            var fPairBonus = a_card1.CardValue == a_card2.CardValue ? fHighCardPoints : 0.0f;
            if (fPairBonus > 0)
            {
                fHighCardPoints = Math.Max(fHighCardPoints + fPairBonus, 5.0f);
            }

            var fSuitBonus = a_card1.CardSuit == a_card2.CardSuit ? 2.0f : 0.0f;

            var gap = Math.Abs(a_card1.CardValue - a_card2.CardValue);
            var fGapFine = 0.0f;

            switch (gap)
            {
                case 0:
                case 1: fGapFine = 0.0f; break;
                case 2: fGapFine = -1.0f; break;
                case 3: fGapFine = -2.0f; break;
                case 4: fGapFine = -4.0f; break;
                default: fGapFine = -5.0f; break;
            }

            var finalScore = fHighCardPoints + fSuitBonus + fGapFine;

            //straight possibility bonus
            if (gap <= 2 && gap != 0 && a_card1.CardValue < eCardValue.Q && a_card2.CardValue < eCardValue.Q)
            {
                ++finalScore;
            }

            //finalScore = (float)Math.Ceiling((float)finalScore);

            return finalScore;
        }

        private struct SuitCardCount
        {
            public eCardSuit Suit;
            public int Count;
            public SuitCardCount(eCardSuit a_suit)
            {
                Suit = a_suit;
                Count = 0;
            }
        }

        private struct ValueCardCount
        {
            public eCardValue Value;
            public int Count;
        }

        private const int POKER_HAND_CARD_COUNT = 5;

        private const float HAND_FRACTION = 1.0f / 13.0f;

        internal static float GetNormalizedStrength_Straight(eCardValue a_highestCardValue, eCardValue a_secondHighestCardValue)
        {
            ///flushes can be made from 2 to 10, which are 9 and a special case
            ///which is A,2,3,4,5, so total 10 straight flushses are possible
            ///(we are ignoring the suits here, since no suit is greater than the other)

            const float MAX_STRAIGHT_FLUSH_INDEX = 9.0f;// 0-9
            switch (a_highestCardValue)
            {
                case eCardValue._6:
                case eCardValue._7:
                case eCardValue._8:
                case eCardValue._9:
                case eCardValue._10:
                case eCardValue.J:
                case eCardValue.Q:
                case eCardValue.K: return ((int)a_highestCardValue - 4) / MAX_STRAIGHT_FLUSH_INDEX;
                case eCardValue.A: return (a_secondHighestCardValue == eCardValue.K ? 9.0f : 8.0f) / MAX_STRAIGHT_FLUSH_INDEX;
                default: return 0.0f;
            }
        }

        #region FourOfAKind Retrieval

        private static readonly double m_minFourOfAKindValue = GetNormalizedStrength_HighCardInternal(eCardValue._2, eCardValue._3);

        private static readonly double m_MaxFourOfAKindValue = GetNormalizedStrength_HighCardInternal(eCardValue.A, eCardValue.K);

        private static readonly double m_fourOfAKindDiff = m_MaxFourOfAKindValue - m_minFourOfAKindValue;
        internal static double GetNormalizedStrength_FourOfAKind(eCardValue a_commonCardValue, eCardValue a_kickerCardValue)
        {
            var tempValue = GetNormalizedStrength_HighCardInternal(a_commonCardValue, a_kickerCardValue);
            return (tempValue - m_minFourOfAKindValue) / (m_fourOfAKindDiff);
        }

        #endregion

        private static float GetNormalizedStrength_SingleCard(eCardValue a_heighestCard)
        {
            //it will return 0 to _2 and 1 to A
            return (int)a_heighestCard / 12.0f;
        }

        #region FullHouse Retrieval
        private static readonly double m_minFullHouseValue = GetNormalizedStrength_HighCardInternal(eCardValue._2, eCardValue._3);

        private static readonly double m_MaxFullHouseValue = GetNormalizedStrength_HighCardInternal(eCardValue.A, eCardValue.K);

        private static readonly double m_fullHouseDiff = m_MaxFullHouseValue - m_minFullHouseValue;
        internal static double GetNormalizedStrength_FullHouse(eCardValue a_threeOfAKindCardValue, eCardValue a_pairCardValue)
        {
            var tempValue = GetNormalizedStrength_HighCardInternal(a_threeOfAKindCardValue, a_pairCardValue);
            return (tempValue - m_minFullHouseValue) / (m_fullHouseDiff);
        }
        #endregion

        #region TwoPair Retrieval

        private static readonly double m_minTwoPairValue = GetNormalizedStrength_HighCardInternal(eCardValue._3, eCardValue._2, eCardValue._4);

        private static readonly double m_MaxTwoPairValue = GetNormalizedStrength_HighCardInternal(eCardValue.A, eCardValue.K, eCardValue.Q);

        private static readonly double m_twoPairDiff = m_MaxTwoPairValue - m_minTwoPairValue;
        internal static double GetNormalizedStrength_TwoPair(eCardValue a_firstPairCardValue, eCardValue a_secondPairCardValue, eCardValue a_kickerCardValue)
        {
            var tempValue = GetNormalizedStrength_HighCardInternal(a_firstPairCardValue, a_secondPairCardValue, a_kickerCardValue);
            return (tempValue - m_minTwoPairValue) / (m_twoPairDiff);
        }
        #endregion

        #region OnePair Retrieval

        private static readonly double m_minOnePairValue = GetNormalizedStrength_HighCardInternal(eCardValue._2, eCardValue._5, eCardValue._4, eCardValue._3);

        private static readonly double m_MaxOnePairValue = GetNormalizedStrength_HighCardInternal(eCardValue.A, eCardValue.K, eCardValue.Q, eCardValue.J);

        private static readonly double m_onePairDiff = m_MaxOnePairValue - m_minOnePairValue;
        internal static double  GetNormalizedStrength_OnePair(eCardValue a_firstPairCardValue,
                                                            eCardValue a_kickerCard1,
                                                            eCardValue a_kickerCard2,
                                                            eCardValue a_kickerCard3)
        {
            var tempValue = GetNormalizedStrength_HighCardInternal(a_firstPairCardValue,a_kickerCard1,a_kickerCard2,a_kickerCard3);
            return (tempValue - m_minOnePairValue) / (m_onePairDiff);
        }
        #endregion

        #region ThreeOfAKind Retrieval
        private static readonly double m_minThreeOfAKindValue = GetNormalizedStrength_HighCardInternal(eCardValue._2, eCardValue._4, eCardValue._3);

        private static readonly double m_MaxThreeOfAKindValue = GetNormalizedStrength_HighCardInternal(eCardValue.A, eCardValue.K, eCardValue.Q);

        private static readonly double m_threeOfAKindDiff = m_MaxThreeOfAKindValue - m_minThreeOfAKindValue;

        internal static double GetNormalizedStrength_ThreeOfAKind(  eCardValue a_threeOfAKindCardValue,
                                                                    eCardValue a_kickerCard1,
                                                                    eCardValue a_kickerCard2 )
        {
            var tempValue = GetNormalizedStrength_HighCardInternal(a_threeOfAKindCardValue, a_kickerCard1, a_kickerCard2);
            return (tempValue - m_minThreeOfAKindValue)/m_threeOfAKindDiff;
        }

        #endregion
        
        private const double TRI_DECIMAL_BASE = 13.0f;
        private static double GetNormalizedStrength_HighCardInternal(params eCardValue[] a_values)
        {
            var length = a_values.Length;
            var fMaxValuePossible = Math.Pow(TRI_DECIMAL_BASE, a_values.Length)-1;
            double finalValue = 0.0f;
            for (int i = length-1; i >= 0; --i)
            {
                finalValue += Math.Pow(TRI_DECIMAL_BASE, ((length-1)-i))* (double)a_values[i];
            }
            return length > 0 ? finalValue / fMaxValuePossible : 0.0f;
        }

        #region HighCard Retrieval
        private static readonly double m_minHighCardValue = GetNormalizedStrength_HighCardInternal(eCardValue._7, eCardValue._5, eCardValue._4, eCardValue._3, eCardValue._2);

        private static readonly double m_maxHighCardValue = GetNormalizedStrength_HighCardInternal(eCardValue.A, eCardValue.K, eCardValue.Q, eCardValue.J, eCardValue._9);

        private static readonly double m_HighCardDiff = m_maxHighCardValue - m_minHighCardValue;
        internal static double GetNormalizedStrength_HighCard(params eCardValue[] a_values)
        {
            double highCardValue = GetNormalizedStrength_HighCardInternal(a_values);
            return ((highCardValue - m_minHighCardValue) / m_HighCardDiff);
        }
        #endregion

        #region Flush Retrieval

        private static readonly double m_minFlushValue = GetNormalizedStrength_HighCardInternal(eCardValue._7, eCardValue._5, eCardValue._4,
                                                                                                eCardValue._3, eCardValue._2);

        private static readonly double m_MaxFlushValue = GetNormalizedStrength_HighCardInternal(eCardValue.A, eCardValue.K, eCardValue.Q,
                                                                                                eCardValue.J, eCardValue._9);

        private static readonly double m_flushDiff = m_MaxFlushValue - m_minFlushValue;
        internal static double GetNormalizedStrength_Flush(params eCardValue[] a_values)
        {
            var tempValue = GetNormalizedStrength_HighCardInternal(a_values);
            return (tempValue - m_minFlushValue) / (m_flushDiff);
        }

        #endregion

        /// <summary>
        /// just checks if its royal flush, it assumes it is a straight flush
        /// </summary>
        /// <param name="a_cards"></param>
        /// <returns></returns>
        private static bool IsRoyalFlush(params Card[] a_cards)
        {
            return a_cards[0].CardValue == eCardValue.A && a_cards[1].CardValue == eCardValue.K;
        }

        /// <summary>
        /// checks if its a flush hand, it assumes its a valid hand
        /// </summary>
        /// <param name="a_cards"></param>
        /// <returns></returns>
        private static bool IsFlush(params Card[] a_cards)
        {
            eCardSuit eSuitToCompare = eCardSuit.None;
            for (int i = 0; i < a_cards.Length; i++)
            {
                if(i != 0)
                {
                    if(a_cards[i].CardSuit != eSuitToCompare)
                    {
                        return false;
                    }
                }
                else
                {
                    eSuitToCompare = a_cards[i].CardSuit;
                }
            }
            return true;
        }

        /// <summary>
        /// it will try to retrieve POKER_HAND_CARD_COUNT sequenctial cards, or null if fails
        /// </summary>
        /// <param name="a_cards"></param>
        /// <returns></returns>
        private static Card[] RetrieveStraight(params Card[] a_cards)
        {
            ///since in poker, the valid sequence consists of exactly 5 cards
            if(a_cards.Length < POKER_HAND_CARD_COUNT)
            {
                return null;
            }

            ///check if the sequence is A,2,3,4,5
            bool bIsAceDueceStraight = IsAceDeuceStraight(a_cards);
            
            var sortedCards = Card.SortCardsByValueDescending(a_cards);

            var firstValue = sortedCards[0];

            var iCount = sortedCards.Length;
            var iStartOfTheSequence = sortedCards[0].CardValue;
            var iSequenceLength = 0;
            var iDuplicatesFound = 0;
            var iTargetSequenceLength = (bIsAceDueceStraight? POKER_HAND_CARD_COUNT-1:POKER_HAND_CARD_COUNT);
            //figire out where in the cards the sequence starts
            //also if there no sequence, then just do not proceed
            for (int i = 1; i < iCount; i++)
            {
                if(sortedCards[i-1].CardValue - sortedCards[i].CardValue == 1)
                {
                    ++iSequenceLength;
                }
                else if (sortedCards[i].CardValue == sortedCards[i - 1].CardValue)
                {
                    ++iDuplicatesFound;
                }
                else
                {
                    ///this is the case where the cards is either in sequence nor the same as previous one
                    ///in this case, check if there are enough cards to make sequence, else return null
                    if (iCount - i < iTargetSequenceLength)
                    {
                        break;
                    }
                    iStartOfTheSequence = sortedCards[i].CardValue;
                    iSequenceLength = 0;
                }
            }

            //if the sequence length is not enough, do not proceed
            if(iSequenceLength < (iTargetSequenceLength-1))
            {
                return null;
            }

            //at this point, it is confirmed that the cards are in sequence

            ///remove deplicate cards here
            int j = 1;
            for (int i = 1; i < sortedCards.Length; i++)
            {
                if (sortedCards[i].CardValue != sortedCards[i - 1].CardValue)
                {
                    sortedCards[j] = sortedCards[i];
                    ++j;
                }
            }

            //trim the last duplicate cards 
            Array.Resize<Card>(ref sortedCards, sortedCards.Length - iDuplicatesFound);

            sortedCards[0] = firstValue;

            if(bIsAceDueceStraight)
            {
                sortedCards = Card.SortCardsByValueAscending(sortedCards);
               sortedCards[POKER_HAND_CARD_COUNT - 1] = firstValue;
            }
            else
            {
                int iIndexToStartCopy = Array.FindIndex<Card>(sortedCards,(Card a_card)=>{return a_card.CardValue == iStartOfTheSequence;});
                Array.Copy(sortedCards, iIndexToStartCopy, sortedCards, 0, sortedCards.Length - iIndexToStartCopy);
            }
            return sortedCards;
        }

        private static bool IsAceDeuceStraight(params Card[] a_cards)
        {
            for (eCardValue i = eCardValue._2 ; i <= eCardValue._5; i++)
            {
                if(!Array.Exists<Card>(a_cards, (Card a_card)=> {return a_card.CardValue == i;}))
                {
                    return false;
                }
            }
            return Array.Exists<Card>(a_cards, (Card a_card) => { return a_card.CardValue == eCardValue.A; });
        }
        
        private static SuitCardCount RetrieveBestSuit(params Card[] a_cards)
        {
            SuitCardCount clubCount = new SuitCardCount(eCardSuit.Clubs);
            SuitCardCount diamondCount = new SuitCardCount(eCardSuit.Diamonds);
            SuitCardCount heartCount = new SuitCardCount(eCardSuit.Hearts);
            SuitCardCount spadeCount = new SuitCardCount(eCardSuit.Spades);

            SuitCardCount returnVal = new SuitCardCount();

            foreach (var item in a_cards)
            {                
                switch (item.CardSuit)
                {
                    case eCardSuit.Clubs:
                        {
                            clubCount.Count++;
                            if (clubCount.Count > returnVal.Count) { returnVal = clubCount; }; break;
                        }
                    case eCardSuit.Diamonds:
                        {
                            diamondCount.Count++;
                            if (diamondCount.Count > returnVal.Count) { returnVal = diamondCount; }; break;
                        }
                    case eCardSuit.Hearts:
                        {
                            heartCount.Count++;
                            if (heartCount.Count > returnVal.Count) { returnVal = heartCount; }; break;
                        }
                    case eCardSuit.Spades:
                        {
                            spadeCount.Count++;
                            if (spadeCount.Count > returnVal.Count) { returnVal = spadeCount; }; break;
                        }
                    default: continue;
                }
            }

            return returnVal;
        }

        private static ValueCardCount RetrieveBestValue(params Card[] a_cards)
        {
            ValueCardCount bestValue = new ValueCardCount();
            bestValue.Value = eCardValue.None;
            
            foreach (var item in a_cards)
            {
                if(item.CardValue != bestValue.Value)
                {
                    var cardCount = RetrieveCardValueCount(item.CardValue,a_cards);
                    //if the new card value is more in number or the card value itself
                    if(cardCount > bestValue.Count)
                    {
                        //make this card value the best card value so far
                        bestValue.Value = item.CardValue;
                        //assign the count
                        bestValue.Count = cardCount;
                    }
                }
            }

            return bestValue;
        }

        private static Card[] RetrieveCardsBySuit(eCardSuit a_suit, params Card[] a_cards)
        {
            return Array.FindAll(a_cards, (Card a_card) => { return a_card.CardSuit == a_suit; });
        }

        private static Card[] RetrieveCardsByValue(eCardValue a_cardValue, params Card[] a_cards)
        {
            return Array.FindAll(a_cards, (Card a_card) => { return a_card.CardValue == a_cardValue; });
        }

        private static int RetrieveCardValueCount(eCardValue a_value, params Card[] a_cards)
        {
            int iCount = 0;
            foreach (var item in a_cards)
            {
                if(item.CardValue == a_value)
                {
                    iCount++;
                }
            }
            return iCount;
        }

        /// <summary>
        /// here assuming the number of cards is 4 at the least
        /// </summary>
        /// <param name="a_commonValue"></param>
        /// <param name="a_cards"></param>
        /// <returns></returns>
        private static Card[] RetrieveFourOfAKind(eCardValue a_commonValue, params Card[] a_cards)
        {
            Card[] cards = new Card[Math.Min(POKER_HAND_CARD_COUNT, a_cards.Length)];

            bool bIncludeKickerCard = a_cards.Length >= POKER_HAND_CARD_COUNT;
            if (bIncludeKickerCard)
            {
                ///making sure kicker card is the smallest
                cards[POKER_HAND_CARD_COUNT - 1].CardValue = eCardValue.None;
            }            
            
            int iIndex = 0;

            foreach (var item in a_cards)
            {
                if(item.CardValue == a_commonValue)
                {
                    cards[iIndex] = item;
                    ++iIndex;
                }
                else
                {
                    if (bIncludeKickerCard && item.CardValue > cards[POKER_HAND_CARD_COUNT - 1].CardValue)
                    {
                        cards[POKER_HAND_CARD_COUNT - 1] = item;
                    }
                }
            }

            return cards;
        }

        private static eCardValue IsThereAnyOtherPair(eCardValue a_valueToIgnore, params Card[] a_cards)
        {
            eCardValue OtherHighPairCardValue = eCardValue.None;
            foreach (var item in a_cards)
            {
                if(item.CardValue != a_valueToIgnore && item.CardValue > OtherHighPairCardValue)
                {
                    if(RetrieveCardValueCount(item.CardValue, a_cards) >= 2)
                    {
                        OtherHighPairCardValue = item.CardValue;
                    }
                }
            }
            return OtherHighPairCardValue;
        }

        /// <summary>
        /// assuming atleast POKER_HAND_CARD_COUNT cards are provided and sorted descending
        /// </summary>
        /// <param name="a_threeOfAKindValue"></param>
        /// <param name="a_cards"></param>
        /// <returns></returns>
        private static Card[] MakeFullHouse(eCardValue a_threeOfAKindValue, eCardValue a_pairCardValue, params Card[] a_cards)
        {
            var threeOfAKindCards = Array.FindAll<Card>(a_cards, (Card a_card) => { return a_card.CardValue == a_threeOfAKindValue; });
            var pairCards = Array.FindAll<Card>(a_cards, (Card a_card) => { return a_card.CardValue == a_pairCardValue; });

            Array.Copy(threeOfAKindCards, a_cards, threeOfAKindCards.Length);
            Array.Copy(pairCards,0, a_cards, threeOfAKindCards.Length,pairCards.Length);
            
            return a_cards;
        }

        /// <summary>
        /// assuming atleast 3 cards are provided and sorted descending
        /// </summary>
        /// <param name="a_threeOfAKindValue"></param>
        /// <param name="a_cards"></param>
        /// <returns></returns>
        private static Card[] MakeThreeOfAKind(eCardValue a_threeOfAKindValue, params Card[] a_cards)
        {
            var threeOfAKindCards = Array.FindAll<Card>(a_cards, (Card a_card) => { return a_card.CardValue == a_threeOfAKindValue; });
            var kickerCards = Array.FindAll<Card>(a_cards, (Card a_card) => { return a_card.CardValue != a_threeOfAKindValue; });

            if(kickerCards.Length > 0)
            {
                Array.Copy(threeOfAKindCards, a_cards, threeOfAKindCards.Length);
                Array.Copy(kickerCards, 0, a_cards, threeOfAKindCards.Length, kickerCards.Length);
            }
            return a_cards;
        }

        private static Card[] MakeTwoPair(eCardValue a_firstPair, eCardValue a_secondPair, params Card[] a_cards)
        {
            var firstPairCards = Array.FindAll<Card>(a_cards, (Card a_card) => { return a_card.CardValue == a_firstPair; });
            var secondPairCards = Array.FindAll<Card>(a_cards, (Card a_card) => { return a_card.CardValue == a_secondPair; });

            var kickerCards = Array.FindAll<Card>(a_cards, (Card a_card) => { return a_card.CardValue != a_firstPair && a_card.CardValue != a_secondPair; });
            kickerCards = Card.SortCardsByValueDescending(kickerCards);

            Array.Copy(firstPairCards, a_cards, firstPairCards.Length);
            Array.Copy(secondPairCards,0, a_cards, firstPairCards.Length,secondPairCards.Length);
            Array.Copy(kickerCards, 0, a_cards, firstPairCards.Length + secondPairCards.Length, kickerCards.Length);
            return a_cards;
        }

        private static Card[] MakeOnePair(eCardValue a_pairCardValue, params Card[] a_cards)
        {
            var firstPairCards = Array.FindAll<Card>(a_cards, (Card a_card) => { return a_card.CardValue == a_pairCardValue; });
            var kickerCards = Array.FindAll<Card>(a_cards, (Card a_card) => { return a_card.CardValue != a_pairCardValue; });
            kickerCards = Card.SortCardsByValueDescending(kickerCards);
            
            Array.Copy(firstPairCards, a_cards, firstPairCards.Length);
            Array.Copy(kickerCards, 0, a_cards, firstPairCards.Length, kickerCards.Length);
            
            return a_cards;
        }

        private static Card[] MakeHighCard(params Card[] a_cards)
        {
            return Card.SortCardsByValueDescending(a_cards);
        }
    }
}
