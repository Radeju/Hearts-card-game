// -----------------------------------------------------------------------
// <copyright file="Enum.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------
using System;

namespace Hearts.Core.Model
{
    //kolejnosc specjalnie zamieniona
    //z powodu waznosci kierek oraz pikow
    public enum CardSuit
    {
        Hearts = 1,
        Spades,
        Diamonds,
        Clubs,
    }

    public enum CardRank
    {
        Ace = 1,
        Deuce,
        Three,
        Four,
        Five,
        Six,
        Seven,
        Eight,
        Nine,
        Ten,
        Jack,
        Queen,
        King
    }

    public enum CardColor
    {
        Black,
        Red
    }

    public enum CardOrder
    {
        Uptown,
        Downtown
    }

    public enum CardGrade
    {
        Low = 1,
        Medium,
        High
    }

    //zmodyfikowac to i powinno dzialac
    public static class CardEnumParser
    {
        //parsuje kolor katy
        public static bool Parse(string value, out CardSuit outParsedSuit)
        {
            if (value == null) throw new ArgumentNullException("Value is null.");
            value = value.Trim();
            if (value.Length == 0) throw new ArgumentException("Must specify valid information for parsing in the string", "value");

            switch(value)
            {
                case "♠":
                    {
                    outParsedSuit = CardSuit.Spades;
                    return true;
                    }
                case "♥":
                    {
                    outParsedSuit = CardSuit.Hearts;
                    return true;
                    }
                case "♣":
                    {
                    outParsedSuit = CardSuit.Clubs;
                    return true;
                    }
                case "♦":
                    {
                    outParsedSuit = CardSuit.Diamonds;
                    return true;
                    }
                default:
                    {
                    outParsedSuit = new CardSuit();
                    return false;
                    }
            }
        }

        //parsuje range karty
        public static bool Parse(string value, out CardRank outParsedRank)
        {
            if (value == null) throw new ArgumentNullException("Value is null.");
            value = value.Trim();
            if (value.Length == 0) throw new ArgumentException("Must specify valid information for parsing in the string", "value");

            switch (value)
            {
                case "A":
                    {
                        outParsedRank = CardRank.Ace;
                        return true;
                    }
                case "K":
                    {
                        outParsedRank = CardRank.King;
                        return true;
                    }
                case "Q":
                    {
                        outParsedRank = CardRank.Queen;
                        return true;
                    }
                case "J":
                    {
                        outParsedRank = CardRank.Jack;
                        return true;
                    }
                default:
                    {
                        int cardRank;
                        if (int.TryParse(value, out cardRank))
                        {
                            outParsedRank = (CardRank)cardRank;
                            return true;
                        }
                        else
                        {
                            outParsedRank = CardRank.Ace;
                            return false;
                        }
                    }
            }
        }
    }
}
