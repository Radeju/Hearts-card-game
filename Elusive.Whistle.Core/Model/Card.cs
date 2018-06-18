using System;

namespace Hearts.Core.Model
{
    public class Card : IComparable<Card>
    {
        #region Rules

        public static bool IsAceBiggest = true;

        #endregion

        #region Properties

        private CardRank rank;
        public CardRank Rank
        {
            get
            {
                return rank;
            }
        }

        private CardSuit suit;
        public CardSuit Suit
        {
            get
            {
                return suit;
            }
            set
            {
                suit = value;
            }
        }

        public CardColor Color
        {
            get
            {
                if ((Suit == CardSuit.Spades) || (Suit == CardSuit.Clubs))
                    return CardColor.Black;
                else
                    return CardColor.Red;
            }
        }

        public int Number
        {
            get
            {
                return (int)rank;
            }
        }

        public string NumberString
        {
            get
            {
                switch (rank)
                {
                    case CardRank.Ace:
                        return "A";
                    case CardRank.Jack:
                        return "J";
                    case CardRank.Queen:
                        return "Q";
                    case CardRank.King:
                        return "K";
                    default:
                        return Number.ToString();
                }
            }
        }

        public string SuitString
        {
            get
            {
                switch (suit)
                {
                    case CardSuit.Spades: return "♠";
                    case CardSuit.Hearts: return "♥";
                    case CardSuit.Clubs: return "♣";
                    case CardSuit.Diamonds: return "♦";
                    default: return Suit.ToString();
                }
            }
        }

        private Deck deck;
        public Deck Deck
        {
            get
            {
                return deck;
            }
            set
            {
                if (deck.Game != value.Game)
                    throw new InvalidOperationException("The new deck must be in the same game like the old deck of the card.");

                if (deck != value)
                {
                    deck.Cards.Remove(this);
                    deck = value;
                    deck.Cards.Add(this);

                    if (DeckChanged != null)
                        DeckChanged(this, null);
                }
            }
        }

        private bool visible = true;
        public bool Visible
        {
            get
            {
                return visible;
            }
            set
            {
                if (visible != value)
                {
                    visible = value;

                    if (VisibleChanged != null)
                        VisibleChanged(this, null);
                }
            }
        }

        private bool enabled = true;
        public bool Enabled
        {
            get
            {
                return enabled;
            }
            set
            {
                enabled = value;
            }
        }

        private bool isDragable = true;

        public bool IsDragable
        {
            get { return isDragable; }
            set { isDragable = value; }
        }

        public CardGrade cardLevel
        {
            get 
            { 
                if(Number >=2 && Number <= 5) return CardGrade.Low;     // 2 3 4 5
                if(Number >=6 && Number <= 10) return CardGrade.Medium; // 6 7 8 9 10
                else return CardGrade.High; //J K Q A
            }
        }

        #endregion

        #region Events

        public event EventHandler VisibleChanged;
        public event EventHandler DeckChanged;

        #endregion

        #region Constructors

        public Card(CardRank rank, CardSuit suit, Deck deck)
        {
            this.rank = rank;
            this.suit = suit;
            this.deck = deck;
            this.deck.Game.Cards.Add(this);
        }

        public Card(int number, CardSuit suit, Deck deck)
        {
            this.rank = (CardRank)number;
            this.suit = suit;
            this.deck = deck;
            this.deck.Game.Cards.Add(this);
        }

        //konstruktor kopiujacy
        public Card(Card copyCard)
        {
            this.rank = copyCard.Rank;
            this.suit = copyCard.Suit;
            this.deck = copyCard.Deck;
            this.enabled = copyCard.Enabled;
            this.isDragable = copyCard.IsDragable;
            this.visible = copyCard.Visible;
            this.deck.Game.Cards.Add(this);
        }

        /// <summary>
        /// Ostroznie z tym konstruktorem - nie ma przypisanego deck'a ani game.
        /// </summary>
        /// <param name="rank">Ranga karty.</param>
        /// <param name="suit">Kolor karty.</param>
        public Card(CardRank rank, CardSuit suit)
        {
            this.rank = rank;
            this.suit = suit;
        }

        #endregion

        #region IComparable<Card> Members

        public int CompareTo(Card other)
        {
            int value1 = this.Number;
            int value2 = other.Number;

            if (Card.IsAceBiggest)
            {
                if (value1 == 1)
                    value1 = 14;

                if (value2 == 1)
                    value2 = 14;
            }

            if (value1 > value2)
                return 1;
            else if (value1 < value2)
                return -1;
            else
                return 0;
        }

        public bool CompareToBool(Card other)
        {
            int value1 = this.Number;
            int value2 = other.Number;

            if (Card.IsAceBiggest)
            {
                if (value1 == 1)
                    value1 = 14;

                if (value2 == 1)
                    value2 = 14;
            }

            if (value1 > value2)
                return true;
            else
                return false;
        }

        #endregion

        #region Move Methods

        /// <summary>
        /// Przesuwa karte na szczyt talii.
        /// </summary>
        public void MoveToFirst()
        {
            MoveToIndex(0);
        }

        /// <summary>
        /// Przesuwa karte na koniec talii.
        /// </summary>
        public void MoveToLast()
        {
            Deck.Cards.Remove(this);
            Deck.Cards.Add(this);
        }

        /// <summary>
        /// Przesuwa karte na losowe miejsce w talii.
        /// </summary>
        public void Shuffle()
        {
            MoveToIndex(Deck.Game.random.Next(0, Deck.Cards.Count));
        }

        /// <summary>
        /// Przesuwa karte na wskazany indeks.
        /// </summary>
        /// <param name="index">Miejsce w talii gdzie karta ma zostac przesunieta.</param>
        public void MoveToIndex(int index)
        {
            Deck.Cards.Remove(this);
            Deck.Cards.Insert(index, this);
        }

        #endregion

        #region Generic Methods

        public override string ToString()
        {
            return this.NumberString + "" + this.SuitString;
        }

        #endregion

        #region operators
        
        public static bool operator ==(Card a, Card b)
        {
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }

            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }

            return a.Suit == b.Suit && a.Rank == b.Rank;
        }

        public static bool operator !=(Card a, Card b)
        {
            return !(a == b);
        }

        #endregion
    }
}
