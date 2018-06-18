// -----------------------------------------------------------------------
// <copyright file="Game.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Hearts.Core.Model
{
    /// <summary>
    /// Game class for game level settings and operations.
    /// </summary>
    public class Game
    {
        #region Properties

        private List<Card> cards = new List<Card>();
        public List<Card> Cards
        {
            get
            {
                return cards;
            }
        }
        /// <summary>
        /// Usuwa wszystkie karty od wskazanego indeksu. Jest obsluga bledu.
        /// </summary>
        /// <param name="beginIndex">Indeks od ktorego wszystko jest usuwane.</param>
        public void RemoveCards(int beginIndex)
        {
            if (cards.Count > beginIndex)
                cards.RemoveRange(beginIndex, cards.Count - beginIndex);
            else
                Console.WriteLine("Indeks poczatkowy poza zakresem listy kart, nic nie robie.");
        }

        private List<Deck> decks = new List<Deck>();
        public List<Deck> Decks
        {
            get
            {
                return decks;
            }
        }
        /// <summary>
        /// Usuwa wszystkie decki gry od wskazanego indeksu. Jest obsluga bledu.
        /// </summary>
        /// <param name="beginIndex">Indeks od ktorego wszystko jest usuwane.</param>
        public void RemoveDecks(int beginIndex)
        {
            if (decks.Count > beginIndex)
                decks.RemoveRange(beginIndex, decks.Count - beginIndex);
            else
                Console.WriteLine("Indeks poczatkowy poza zakresem listy talii, nic nie robie.");
        }

        internal Random random = new Random();
        internal HighCardSuitComparer CardSuitComparer = new HighCardSuitComparer();

        #endregion
    }
}
