// -----------------------------------------------------------------------
// <copyright file="Deck.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hearts.Core.Model
{
    /// <summary>
    /// Class to represent a deck of cards, or a hand of cards, or any
    /// other collection of cards used in the game.  I.e. the dealer's
    /// deck, or a stack of cards in a solitairre game, or a discard pile.
    /// </summary>
    public class Deck
    {
        #region Properties

        /// <summary>
        /// Gets the cards collection for this deck.
        /// </summary>
        public List<Card> Cards
        {
            get { return _cards; }
        }
        /// <summary>
        /// Ustawia karty talii - ostroznie z ta funkcja.
        /// </summary>
        public List<Card> CardsSet
        {
            set { _cards = value; }
        }
        private List<Card> _cards = new List<Card>();

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Deck"/> is enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if enabled; otherwise, <c>false</c>.
        /// </value>
        public bool Enabled
        {
            get { return _enabled; }
            set { _enabled = value; }
        }
        private bool _enabled = true;

        /// <summary>
        /// Gets the game instance to which this deck is associated.
        /// </summary>
        public Game Game
        {
            get { return _game; }
        }
        private Game _game;

        /// <summary>
        /// Gets the top card.
        /// </summary>
        public Card TopCard
        {
            get { return Cards.Count > 0 ? Cards[Cards.Count - 1] : null; }
        }

        public Card RandomCard
        {
            get { return Cards.Count > 0 ? Cards[_game.random.Next(0, Cards.Count)] : null; }
        }

        /// <summary>
        /// Gets the bottom card.
        /// </summary>
        public Card BottomCard
        {
            get { return Cards.Count > 0 ? Cards[0] : null; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has cards.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has cards; otherwise, <c>false</c>.
        /// </value>
        public bool HasCards
        {
            get { return Cards.Count > 0; }
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when [sort changed].
        /// </summary>
        public event EventHandler SortChanged;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Deck"/> class.
        /// </summary>
        /// <param name="game">The game.</param>
        public Deck(Game game)
        {
            this._game = game;
            this._game.Decks.Add(this);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Deck"/> class.
        /// </summary>
        /// <param name="numberOfDecks">The number of decks.</param>
        /// <param name="uptoNumber">The upto number.</param>
        /// <param name="game">The game.</param>
        public Deck(int numberOfDecks, int uptoNumber, Game game)
            : this(game)
        {
            for (int deck = 0; deck < numberOfDecks; deck++)
            {
                for (int suit = 1; suit <= 4; suit++)
                {
                    for (int number = 1; number <= uptoNumber; number++)
                    {
                        Cards.Add(new Card(number, (CardSuit)suit, this));
                    }
                }
            }

            Shuffle();
        }

        /// <summary>
        /// Pusty konstruktor - ostroznie z nim poniewaz wiele z jego 
        /// pol jest nullami.
        /// </summary>
        public Deck(){}

        /// <summary>
        /// Konstruktor kopiujacy.
        /// </summary>
        /// <param name="game">Talia z ktorej kopiujemy.</param>
        public Deck(Deck copyDeck, bool @addGame = true)
        {
            this._cards = copyDeck.Cards;
            this._enabled = copyDeck._enabled;

            if (addGame)
            {
                this._game = copyDeck._game;
                this._game.Decks.Add(this);
            }
            else
                this._game = null;
        }

        #endregion

        #region Get Methods

        /// <summary>
        /// Determines whether [has] [the specified number].
        /// </summary>
        /// <param name="number">The number.</param>
        /// <param name="suit">The suit.</param>
        /// <returns>
        ///   <c>true</c> if [has] [the specified number]; otherwise, <c>false</c>.
        /// </returns>
        public bool Has(int number, CardSuit suit)
        {
            return Has((CardRank)number, suit);
        }

        /// <summary>
        /// Determines whether [has] [the specified rank].
        /// </summary>
        /// <param name="rank">The rank.</param>
        /// <param name="suit">The suit.</param>
        /// <returns>
        ///   <c>true</c> if [has] [the specified rank]; otherwise, <c>false</c>.
        /// </returns>
        public bool Has(CardRank rank, CardSuit suit)
        {
            if (GetCard(rank, suit) != null)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Sprawdza czy ma karte o podanych parametrach i ja zwraca.
        /// </summary>
        /// <param name="rank">Wartosc szukanej karty.</param>
        /// <param name="suit">Kolor szukanej karty.</param>
        /// <param name="outCard">Out parametr.</param>
        /// <returns>True jezeli ma, false jezeli nie ma.</returns>
        public bool Has(CardRank rank, CardSuit suit, out Card outCard)
        {
            outCard = GetCard(rank, suit);
            if (outCard != null)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Sprawdza czy talia ma jakakolwiek karte o podanym kolorze.
        /// </summary>
        /// <param name="suit">Szukany kolor.</param>
        /// <returns>
        ///   <c>Prawda</c> if [has] [the specified rank]; w przeciwnym wypadku, <c>false</c>.
        /// </returns>
        public bool HasColor(CardSuit suit)
        {
            if (GetColor(suit) != null)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Gets the card in the deck matching the specified integer
        /// rank value and the specified suit.
        /// </summary>
        /// <param name="number">The number of the rank to match.</param>
        /// <param name="suit">The suit to match.</param>
        /// <returns>The matching card.</returns>
        public Card GetCard(int number, CardSuit suit)
        {
            return GetCard((CardRank)number, suit);
        }

        /// <summary>
        /// Gets the first matching card in the deck matching the
        /// specified suit and rank.
        /// </summary>
        /// <param name="rank">The rank to match.</param>
        /// <param name="suit">The suit to match.</param>
        /// <returns>The matching card.</returns>
        public Card GetCard(CardRank rank, CardSuit suit)
        {
            return Cards.FirstOrDefault(card => (card.Rank == rank) && (card.Suit == suit));
        }

        /// <summary>
        /// Zwraca pierwsza karte o podanym kolorze
        /// </summary>
        /// <param name="suit">Kolor karty</param>
        /// <returns>Pierwsza karta o kolorze z argumentu.</returns>
        public Card GetColor(CardSuit suit)
        {
            return Cards.FirstOrDefault(card => card.Suit == suit);
        }

        /// <summary>
        /// Zwraca liczbe kart kazdego koloru w decku
        /// </summary>
        /// <returns>Lista kart kolor z ich liczebnoscia, ulozona wg 
        /// starszenstwa kier -> pik -> karo -> trefl.</returns>
        public List<ColorsLength> CountColors()
        {
            List<ColorsLength> colorsLength = new List<ColorsLength>();

            int hearts = Cards.Count(e => e.Suit == CardSuit.Hearts);
            int spades = Cards.Count(e => e.Suit == CardSuit.Spades);
            int diamonds = Cards.Count(e => e.Suit == CardSuit.Diamonds);
            int clubs = Cards.Count(e => e.Suit == CardSuit.Clubs);

            colorsLength.Add(new ColorsLength(hearts, CardSuit.Hearts));
            colorsLength.Add(new ColorsLength(spades, CardSuit.Spades));
            colorsLength.Add(new ColorsLength(diamonds, CardSuit.Diamonds));
            colorsLength.Add(new ColorsLength(clubs, CardSuit.Clubs));

            return colorsLength;
        }

        /// <summary>
        /// Zwraca najdluzszy kolor. Jezeli kolory sa tej samej dlugosci to zwraca
        /// w kolejnosci kier - pik - karo - trefl
        /// </summary>
        /// <param name="reverse">Odwrotne starszenstwo kolorow (trefl - karo - pik - kier).</param>
        /// <param name="skip">Ktory z kolei kolor chcesz (od 0 do 3).</param>
        /// <returns>Najdluzszy kolor.</returns>
        public CardSuit LongestColor(bool @reverse = false, int @skip = 0)
        {
            List<ColorsLength> colorsLength = CountColors();

            if(reverse)
                colorsLength = colorsLength.OrderByDescending(x => x.ColorLength).ThenByDescending(y => y.ColorNumber).ToList();
            else
                colorsLength = colorsLength.OrderByDescending(x => x.ColorLength).ThenBy(y => y.ColorNumber).ToList();

            if(skip > 0)
            {
                //jak przekroczysz indeks
                try
                {
                    if (colorsLength[skip].ColorLength > 0)
                        return colorsLength[skip].Color;
                    else
                        return colorsLength.FirstOrDefault().Color;
                }
                catch
                {
                    return colorsLength.FirstOrDefault().Color;
                }
            }
            else
                return colorsLength.FirstOrDefault().Color;
        }

        /// <summary>
        /// Zwraca najkrotszy kolor. Jezeli kolory sa tej samej dlugosci to zwraca
        /// w kolejnosci kier - pik - karo - trefl
        /// </summary>
        /// <param name="reverse">Odwrotne starszenstwo kolorow (trefl - karo - pik - kier).</param>
        /// <param name="skip">Ktory z kolei kolor chcesz (od 0 do 3).</param>
        /// <returns>Najkrotszy kolor.</returns>
        public CardSuit ShortestColor(bool @reverse = false, int @skip = 0)
        {
            List<ColorsLength> colorsLength = CountColors();

            if (reverse)
                colorsLength = colorsLength.OrderBy(x => x.ColorLength).ThenByDescending(y => y.ColorNumber).ToList();                
            else
                colorsLength = colorsLength.OrderBy(x => x.ColorLength).ThenBy(y => y.ColorNumber).ToList();

            if (skip > 0)
            {
                //jak przekroczysz indeks
                try
                {
                    if (colorsLength[skip].ColorLength > 0)
                        return colorsLength[skip].Color;
                    else
                        return colorsLength.FirstOrDefault(e => e.ColorLength > 0).Color; ;
                }
                catch
                {
                    return colorsLength.FirstOrDefault(e => e.ColorLength > 0).Color; ;
                }
            }
            else
                return colorsLength.FirstOrDefault(e => e.ColorLength > 0).Color;
        }

        /// <summary>
        /// Zwraca liczbe kart w podanym kolorze.
        /// </summary>
        /// <param name="chosenColor">Szukany kolor.</param>
        /// <returns>Liczba kart w szukanym kolorze.</returns>
        public int ColorLength(CardSuit chosenColor)
        {
            int chosenColorCount = Cards.Count(e => e.Suit == chosenColor);
            return chosenColorCount;
        }

        #endregion

        #region Sort Methods

        /// <summary>
        /// Shuffles the the deck's cards list one time.
        /// </summary>
        public void Shuffle()
        {
            Shuffle(1);
        }

        /// <summary>
        /// Shuffles the deck's cards list specified number of times.
        /// </summary>
        /// <param name="times">The number of times.</param>
        public void Shuffle(int times)
        {
            for (int time = 0; time < times; time++)
            {
                for (int i = 0; i < Cards.Count; i++)
                {
                    Cards[i].Shuffle();
                }
            }

            if (SortChanged != null)
                SortChanged(this, null);
        }

        /// <summary>
        /// Sorts the cards in the deck  using the Game's suit comparer.
        /// </summary>
        public void Sort()
        {
            Cards.Sort(Game.CardSuitComparer);

            if (SortChanged != null)
                SortChanged(this, null);
        }
        
        #endregion

        #region Draw Cards Methods

        /// <summary>
        /// Draws/Moves the specified amount of cards to the specified deck.
        /// </summary>
        /// <param name="toDeck">To deck.</param>
        /// <param name="count">The count.</param>
        public void Draw(Deck toDeck, int count)
        {
            for (var i = 0; i < count; i++)
            {
                TopCard.Deck = toDeck;
            }
        }

        /// <summary>
        /// Podobnie co Draw ale nie okreslona liczbe kart z gory tylko
        /// okreslona w parametrze karte. Jezeli karty nie ma w talii
        /// to nic sie nie dzieje.
        /// </summary>
        /// <param name="toDeck">Do ktorej talii.</param>
        /// <param name="cardToMove">Karta ktora ma zostac przeniesiona.</param>
        public void DrawSpecificCard(Deck toDeck, Card cardToMove)
        {
            for (int i = 0; i < this.Cards.Count; i++)
            {
                if (this.Cards[i] == cardToMove)
                {
                    this.Cards[i].Deck = toDeck;
                    return;
                }
            }
        }

        #endregion

        #region Generic Methods

        /// <summary>
        /// Method to toggle the visibility of all cards in the deck.
        /// </summary>
        public void FlipAllCards()
        {
            foreach (var t in Cards)
            {
                t.Visible = !t.Visible;
            }
        }

        public void ClearDeck()
        {
            _cards.Clear();
        }

        /// <summary>
        /// Wszystkie karty w talii staja sie widczone.
        /// </summary>
        public void AllCardsVisible()
        {
            foreach (var t in Cards)
            {
                t.Visible = true;
            }
        }

        /// <summary>
        /// Wszystkie karty w talii staja sie niewidczone.
        /// </summary>
        public void AllCardsNotVisible()
        {
            foreach (var t in Cards)
            {
                t.Visible = false;
            }
        }

        /// <summary>
        /// Method to set the value of Enabled to the provided value on
        /// all of the cards in the deck.
        /// </summary>
        /// <param name="enable">if set to <c>true</c> [enable].</param>
        public void EnableAllCards(bool enable)
        {
            foreach (var t in Cards)
            {
                t.Enabled = enable;
            }
        }

        /// <summary>
        /// Method to set the value of the IsDragable on all of the cards
        /// in the deck.
        /// </summary>
        /// <param name="isDragable">if set to <c>true</c> [is dragable].</param>
        public void MakeAllCardsDragable(bool isDragable)
        {
            foreach (var t in Cards)
            {
                t.IsDragable = isDragable;
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            var output = new StringBuilder();

            output.Append("[" + Environment.NewLine);

            foreach (var t in Cards)
            {
                output.Append(t.ToString() + Environment.NewLine);
            }

            output.Append("]" + Environment.NewLine);

            return output.ToString();
        }

        #endregion
    }

    public class ColorsLength
    {
        private int colorLength;
        public int ColorLength
        {
            get { return colorLength; }
            set { colorLength = value; }
        }

        private CardSuit color;
        public CardSuit Color
        {
            get { return color; }
            set { color = value; colorNumber = (int)value; }
        }

        private int colorNumber;
        public int ColorNumber
        {
            get { return colorNumber; }
        }

        public ColorsLength(int colLength, CardSuit col)
        {
            colorLength = colLength;
            color = col;
            colorNumber = (int)col;
        }
    }
}
