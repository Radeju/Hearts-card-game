// -----------------------------------------------------------------------
// <copyright file="Hand.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;

namespace Hearts.Core.Model
{
    /// <summary>
    /// Class to represent a single hand of cards in a multiplayer card game.
    /// </summary>
    public class Hand
    {
        private List<Card> cardsInHand { get; set; }
        public List<Card> CardsInHand
        {
            get { return cardsInHand; }
        }

        private List<Card> cardsPlayed { get; set; }
        public List<Card> CardsPlayed
        {
            get { return cardsPlayed; }
        }

        //renonse(krotkosci) w poszczegolnych kolorach
        private Dictionary<CardSuit, bool> cardRenons;
        public Dictionary<CardSuit, bool> CardRenons
        {
            get{ return cardRenons; }
            set { cardRenons = value; }
        }

        internal HighCardSuitComparer HighCardSuitComparer = new HighCardSuitComparer();
        internal LowCardSuitComparer LowCardSuitComparer = new LowCardSuitComparer();

        public void AddInHand(Card handCard)
        {
            cardsInHand.Add(handCard);
        }

        public void AddPlayed(Card playedCard)
        {
            cardsPlayed.Add(playedCard);
        }

        public Hand()
        {
            cardsInHand = new List<Card>();
            cardsPlayed = new List<Card>();

            //nie ma krotkosci w zadnym z kolorow
            cardRenons = new Dictionary<CardSuit, bool>();
            cardRenons.Add(CardSuit.Spades, false);
            cardRenons.Add(CardSuit.Hearts, false);
            cardRenons.Add(CardSuit.Clubs, false);
            cardRenons.Add(CardSuit.Diamonds, false);
        }

        public void DeleteRenons()
        {
            cardRenons[CardSuit.Spades] = false;
            cardRenons[CardSuit.Hearts] = false;
            cardRenons[CardSuit.Diamonds] = false;
            cardRenons[CardSuit.Clubs] = false;
        }

        public void SortHigh()
        {
            cardsInHand.Sort(HighCardSuitComparer);
            cardsPlayed.Sort(HighCardSuitComparer);
        }

        public void SortLow()
        {
            cardsInHand.Sort(LowCardSuitComparer);
            cardsPlayed.Sort(LowCardSuitComparer);
        }

        /// <summary>
        /// Czysci obie listy - zarowno kart zagranych jak i wygranych lew
        /// </summary>
        public void ClearHand()
        {
            cardsInHand.Clear();
            cardsPlayed.Clear();
        }
    }
}
