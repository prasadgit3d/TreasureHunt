using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace SillyGames.SGBase.CardUtils
{
    public enum eCardValue
    {
        None = -1,
        _2 = 0,
        _3 = 1,
        _4 = 2,
        _5 = 3,
        _6 = 4,
        _7 = 5,
        _8 = 6,
        _9 = 7,
        _10 = 8,
        _11 = 9,
        _12 = 10,
        _13 = 11,
        _14 = 12,
        J = _11, Q = _12, K = _13, A = _14
    }
    public enum eCardSuit
    {
        None,
        Clubs,
        Diamonds,
        Hearts,
        Spades,
    }

    /// <summary>
    /// a struct to hold data for a playing card
    /// it also contains common card specific static helper functions
    /// </summary>
    [Serializable]
    [DebuggerDisplay("{SUIT_ICONS[CardSuit]}:{SUIT_VALUES[CardValue]}")]
    public struct Card
    {

        private static Dictionary<eCardSuit, string> SUIT_ICONS = new Dictionary<eCardSuit, string>()
        {
            {eCardSuit.None,"?"},
            {eCardSuit.Clubs,"♣"},
            {eCardSuit.Diamonds,"♦"},
            {eCardSuit.Hearts,"♥"},
            {eCardSuit.Spades,"♠"},
        };
        private static Dictionary<eCardValue, string> SUIT_VALUES = new Dictionary<eCardValue, string>()
        {
            {eCardValue.None    ,"?"},
            {eCardValue._2    ,"2"},
            {eCardValue._3     ,"3"},
            {eCardValue._4     ,"4"},
            {eCardValue._5     ,"5"},
            {eCardValue._6     ,"6"},
            {eCardValue._7     ,"7"},
            {eCardValue._8     ,"8"},
            {eCardValue._9     ,"9"},
            {eCardValue._10    ,"10"},
                {eCardValue.J  ,"J"},
                {eCardValue.Q  ,"Q"},
                {eCardValue.K  ,"K"},
                {eCardValue.A  ,"A"}
        };

        private eCardValue m_eCardValue;
        public eCardValue CardValue
        {
            get { return m_eCardValue; }
            set { m_eCardValue = value; }
        }

        private eCardSuit m_eCardSuit;
        public eCardSuit CardSuit
        {
            get { return m_eCardSuit; }
            set { m_eCardSuit = value; }
        }

        public Card(eCardValue a_eCardValue, eCardSuit a_eCardSuit)
        {
            m_eCardValue = a_eCardValue;
            m_eCardSuit = a_eCardSuit;
        }

        #region static helper functions

        /// <summary>
        /// checks if all the card suits are same
        /// expects all eCardSuits values to be valid
        /// internally compares all the values with the fisrt one
        /// </summary>
        /// <param name="a_cardSuits"></param>
        /// <returns>true if all the suits have same value, false othewise</returns>
        public static bool isItSameColor(params eCardSuit[] a_cardSuits)
        {
            for (int i = 1; i < a_cardSuits.Length; i++)
            {
                if (a_cardSuits[i] != a_cardSuits[0])
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// checks if the values are in a sequence
        /// does not expects the values to be sorted
        /// expects all cards values to be valid
        /// A,2,3... is considered a valid sequence
        /// </summary>
        /// <param name="a_eCardValues">values to be checked a sequence for</param>
        /// <returns>true if the values are in sequence, false otherwise</returns>
        public static bool isItSequence(params eCardValue[] a_eCardValues)
        {
            var sortedCards = GetCardsSortedByValue(a_eCardValues);
            for (int i = 0; i < (sortedCards.Length - 1); i++)
            {
                if ((sortedCards[i + 1] - sortedCards[i]) != 1)
                {
                    ///if the code lands up here only if the cards are not in a sequence
                    ///but we also need to check if it is A,2,3... so just perform an additional check

                    ///if this is a last comparison that failed && First cards is 2 && Last cards is A then its A,2,3.. sequence
                    if (i == (sortedCards.Length - 2) && sortedCards[0] == eCardValue._2 && sortedCards[sortedCards.Length - 1] == eCardValue.A)
                    {
                        return true;
                    }
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// checks if the cards are in a pure sequence
        /// does not expects cards to be sorted
        /// expects all cards to be valid
        /// does not check if all cards belongs to common logical deck 
        /// internally calls  <see cref="isItSameColor(params eCardValue[])"/> and <see cref="isItSequence(params eCardValue[])"/>
        /// </summary>
        /// <param name="a_cards"></param>
        /// <returns>true if the cards are in pure sequence, false otherwise</returns>
        public static bool isItPureSequence(params Card[] a_cards)
        {
            var cardSuits = new eCardSuit[a_cards.Length];
            var cardValues = new eCardValue[a_cards.Length];
            for (int i = 0; i < a_cards.Length; i++)
            {
                cardSuits[i] = a_cards[i].CardSuit;
                cardValues[i] = a_cards[i].CardValue;
            }
            return isItSameColor(cardSuits) && isItSequence(cardValues);
        }

        /// <summary>
        /// sorts the array of card values in ascending order
        /// </summary>
        /// <param name="a_cardValues">card values that needs to be sorted</param>
        /// <returns>sorted array of card values</returns>
        public static eCardValue[] GetCardsSortedByValue(params eCardValue[] a_cardValues)
        {
            return GetCardsSortedByValue(true, a_cardValues);
        }

        /// <summary>
        /// sorts the array of card values in specified order
        /// </summary>
        /// <param name="a_bAssending">if true, sorts in ascending order, descending otherwise</param>
        /// <param name="a_cardValues">card values that needs to be sorted</param>
        /// <returns>sorted array of card values</returns>
        public static eCardValue[] GetCardsSortedByValue(bool a_bAssending, params eCardValue[] a_cardValues)
        {
            Array.Sort(a_cardValues, (eCardValue a_val1, eCardValue a_val2) => { return a_bAssending ? (a_val1 - a_val2) : (a_val2 - a_val1); });
            return a_cardValues;
        }

		public static readonly Card kNullCard = new Card(eCardValue.None, eCardSuit.None);

        public byte toByte()
        {
            return toByte(this);
        }
        public static byte toByte(Card a_card)
        {
            return (byte)a_card.GetHashCode();
        }

        public static Card fromByte(byte a_byte)
        {
            return new Card((eCardValue)(a_byte>>3), (eCardSuit)(((byte)(a_byte<<5))>>5));
        }

        public override string ToString()
        {
            char strSuit;
            
            if (CardSuit == eCardSuit.Clubs) strSuit =              'C';
            else if (CardSuit == eCardSuit.Spades) strSuit =        'S';
            else if (CardSuit == eCardSuit.Hearts) strSuit =        'H';
            else if (CardSuit == eCardSuit.Diamonds) strSuit =      'D';
            else strSuit =                                          'X';

            char strVal;

            if (CardValue == eCardValue._2) strVal =        '2';
            else if (CardValue == eCardValue._3) strVal =   '3';
            else if (CardValue == eCardValue._4) strVal =   '4';
            else if (CardValue == eCardValue._5) strVal =   '5';
            else if (CardValue == eCardValue._6) strVal =   '6';
            else if (CardValue == eCardValue._7) strVal =   '7';
            else if (CardValue == eCardValue._8) strVal =   '8';
            else if (CardValue == eCardValue._9) strVal =   '9';
            else if (CardValue == eCardValue._10) strVal =  'T';
            else if (CardValue == eCardValue._11) strVal =  'J';
            else if (CardValue == eCardValue._12) strVal =  'Q';
            else if (CardValue == eCardValue._13) strVal =  'K';
            else if (CardValue == eCardValue._14) strVal =  'A';
            else strVal =                                   'X';

            return strSuit.ToString() +  strVal.ToString();
        }

        public static string ToString(Card a_card)
        {
            return a_card.ToString();
        }
        public static Card FromString(string a_cardString)
        {
            Card card = new Card();
            
            char suit = a_cardString[0];

            if ('C' == suit) card.CardSuit = eCardSuit.Clubs;
            else if ('S' == suit) card.CardSuit = eCardSuit.Spades;
            else if ('H' == suit) card.CardSuit = eCardSuit.Hearts;
            else if ('D' == suit) card.CardSuit = eCardSuit.Diamonds;
            else card.CardSuit = eCardSuit.None;

            char val = a_cardString[1];

            if ('2' == val) card.CardValue = eCardValue._2;
            else if('3' == val)card.CardValue = eCardValue._3;
			else if ('4' == val) card.CardValue = eCardValue._4;
			else if ('5' == val) card.CardValue = eCardValue._5;
			else if ('6' == val) card.CardValue = eCardValue._6;
			else if ('7' == val) card.CardValue = eCardValue._7;
			else if ('8' == val) card.CardValue = eCardValue._8;
			else if ('9' == val) card.CardValue = eCardValue._9;
			else if ('T' == val) card.CardValue = eCardValue._10;
			else if ('J' == val) card.CardValue = eCardValue.J;
			else if ('Q' == val) card.CardValue = eCardValue.Q;
			else if ('K' == val) card.CardValue = eCardValue.K;
			else if ('A' == val) card.CardValue = eCardValue.A;
            else card.CardValue = eCardValue.None;

            return card;            
        }

        public string ToStringPretty()
        {
            return "" + SUIT_ICONS[CardSuit]+ ":" + SUIT_VALUES[CardValue];
        }
        //public static byte[] toByteArray(Card a_card)
        //{
        //    var suitByteData = BitConverter.GetBytes((char)a_card.CardSuit);
        //    var valueByteData = BitConverter.GetBytes((char)a_card.CardValue);
        //    var finalBytes = new byte[suitByteData.Length + valueByteData.Length];
        //    suitByteData.CopyTo(finalBytes, 0);
        //    valueByteData.CopyTo(finalBytes, suitByteData.Length);
        //    return finalBytes;
        //}

        //public static Card fromByteArray(byte[] a_cardData)
        //{
        //    eCardSuit suite = (eCardSuit)BitConverter.ToChar(a_cardData, 0);
        //    eCardValue value = (eCardValue)BitConverter.ToChar(a_cardData,sizeof(char));
        //    return new Card(value, suite);
        //}

        #endregion

        public static bool operator ==(Card a_card1, Card a_card2)
        {
            return (a_card1.CardValue == a_card2.CardValue) && (a_card1.CardSuit == a_card2.CardSuit);
        }

        public static bool operator !=(Card a_card1, Card a_card2)
        {
            return (a_card1.CardValue != a_card2.CardValue) || (a_card1.CardSuit != a_card2.CardSuit);
        }

        public override bool Equals(object obj)
        {
            Card otherCard = (Card)obj;
            
            return otherCard == this;
        }

        public override int GetHashCode()
        {
            byte iValue = (byte)CardValue;
            byte iSuit = (byte)CardSuit;
            return (iValue << 3) | iSuit;
        }

        /// <summary>
        /// gives the sorted cards based on its value, in descending order, highest card comes first
        /// </summary>
        /// <param name="a_cards"></param>
        internal static Card[] SortCardsByValueDescending(params Card[] a_cards)
        {
            ///the minus sign in predicate is to make it in descending order
            Array.Sort<Card>(a_cards, (Card a_cardA, Card a_cardB) => { return -a_cardA.CardValue.CompareTo(a_cardB.CardValue); });
            return a_cards;
        }

        internal static Card[] SortCardsByValueAscending(params Card[] a_cards)
        {
            ///the minus sign in predicate is to make it in descending order
            Array.Sort<Card>(a_cards, (Card a_cardA, Card a_cardB) => { return a_cardA.CardValue.CompareTo(a_cardB.CardValue); });
            return a_cards;
        }
    }

    public class CardDeck
    {
        private List<Card> m_lstCards = new List<Card>(52);
        private List<Card> m_lstDrawnCards = new List<Card>();
        private System.Random m_random = null;

        public CardDeck()
        {
            FillCards();
        }

        public CardDeck(int a_iSeed, bool a_bShuffle = false)
        {
            SetSeed(a_iSeed);
            FillCards();
            if(a_bShuffle)
            {
                Shuffle();
            }
        }
        
        public void SetSeed(int a_iSeed)
        {
            m_random = new Random(a_iSeed);            
        }

        private void FillCards()
        {
            m_lstCards.Clear();
            for (eCardSuit suit = eCardSuit.Clubs; suit <= eCardSuit.Spades; suit++)
            {
                for (eCardValue cardVal = eCardValue._2; cardVal <= eCardValue.A; cardVal++)
                {
                    m_lstCards.Add(new Card(cardVal, suit));
                }
            }
        }

		public int CardsRemaining()
		{
			return(m_lstCards.Count);
		}

		public int CardsDrawn()
		{
			return (m_lstDrawnCards.Count);
		}
        
        public Card[] AllCards
        {
            get
            {
                return m_lstCards.ToArray();
            }
        }

        public void Shuffle()
        {
            if (m_random == null)
            {
                ///to make sure the seed is unigue for subsequent calls to multiple shuffles, when the default 
                ///time dependent seed will be common
                m_random = new Random(System.Guid.NewGuid().GetHashCode());
            }
            ShufflePrivate();
        }

        public void Shuffle(int a_iSeed)
        {
            m_random = new Random(a_iSeed);
            ShufflePrivate();
        }
        /// <summary>
        /// This is an inplace shuffle, algorithm from
        /// Fisher–Yates shuffle, The modern algorithm
        /// https://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle
        /// 
        ///  To shuffle an array a of n elements (indices 0..n-1):
        ///  for i from n − 1 downto 1 do
        ///     j ← random integer such that 0 ≤ j ≤ i
        ///     exchange a[j] and a[i]
        /// </summary>
        private void ShufflePrivate()
        {
            for (int i = 0; i < m_lstCards.Count; i++)
            {
				int j = m_random.Next(i, m_lstCards.Count);
                m_lstCards.Swap(i, j);
            }
        }

        /// <summary>
        /// collects all the cards, drawn so far
        /// after this call, deck has all 52 cards
        /// </summary>
        public void Collect()
        {
            m_lstCards.AddRange(m_lstDrawnCards);
            m_lstDrawnCards.Clear();
        }

        /// <summary>
        /// pops a card from top of the deck
        /// does not use random, make sure you shuffle the deck before drawing one if required
        /// this card is temporarily removed from the deck, untill you peform <see cref="Collect()"/> again
        /// does not perform any range check
        /// </summary>
        /// <returns>a card from top of the deck</returns>
        public Card Draw()
        {
            Card card = m_lstCards[0];
            m_lstCards.RemoveAt(0);
            m_lstDrawnCards.Add(card);
            return card;
        }

        /// <summary>
        /// pops the cards from the deck
        /// does not use random, make sure you shuffle the deck before drawing if required
        /// these cards are temporarily removed from the deck, untill you perform <see cref="Collect()"/> again
        /// does not perform any range check
        /// </summary>
        /// <param name="a_iNumberOfCards"></param>
        /// <returns></returns>
        public Card[] Draw(int a_iNumberOfCards)
        {
            Card[] cards = new Card[a_iNumberOfCards];
            m_lstCards.CopyTo(0, cards, 0, a_iNumberOfCards);
            m_lstCards.RemoveRange(0, a_iNumberOfCards);
            m_lstDrawnCards.AddRange(cards);
            return cards;
        }
        

    }
}