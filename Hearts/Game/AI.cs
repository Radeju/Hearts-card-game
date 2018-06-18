using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Hearts;
using Hearts.Core.Controls;
using Hearts.Core.Model;

namespace Hearts
{
    public enum AlgorithmTypes
    {
        Random = 1,
        Greedy,
        Heuristic,
    }


    /* Metody odpowiedzialne za sztuczna inteligencje znajduja sie w tym pliku.
     * Korzystaja z funkcji znajdujacych sie w GameRules.cs. Parametrami kazdego
     * algorytmu sa zmienne unikalne dla kazdego gracza, czyli:
     * - mozliwe ruchy
     * - pozycja w lewie (od 1 do 4)
     * - karty lezace na stole w obecnej lewie, ustawione w kolejnosci ich zagrywki
     * 
     *  W kierkach:
     *  - zwyciestwo = nie wziecie lewy
     *  - przegrana = wziecie lewy
     */
    public partial class MainWindow : Window
    {

        /// <summary>
        /// Zwraca obecnie wygrywajaca lewe karte. Jezeli jest tylko 1 karta w lewie to zwroci ja.
        /// Kolorem obowiazujacym jest kolor wistu (czyli 1sza karta na liscie).
        /// </summary>
        /// <param name="curTrickCards">Obecna lewa. Moze sie skladac z od 0 do 4 kart.</param>
        /// <param name="outHighestCard">Zwracana karta w out'ie ktora jest najwysza.</param>
        /// <returns>PRAWDA jezeli jest jakas karta w lewie. Jezeli lewa jest pusta to nie ma co zwroc
        /// i zwraca FALSE, zas w out'ie jest null. </returns>
        public bool CurTrickHighestCard(List<Card> curTrickCards, out Card outHighestCard)
        {
            Card highestCard = null;

            //jezeli pusty to zwroc nulla i false (nie ma najwyzszej)
            if (curTrickCards.Count <= 0)
            {
                outHighestCard = highestCard;
                return false;
            }

            highestCard = curTrickCards.First();
            //jezeli jeden element to zwroc go
            if (curTrickCards.Count == 1)
            {
                outHighestCard = highestCard;
                return true;
            }

            //jezeli wiecej niz 1 element to porownaj po kolei
            for (int i = 0; i < curTrickCards.Count; i++)
            {
                GameRules.CompareCards(highestCard, curTrickCards[i], out highestCard);
            }

            outHighestCard = highestCard;
            return true;
        }

        /// <summary>
        /// Zlicza liczbe kart zagranych w danym kolorze. Uwzglednia skonczone lewy oraz obecna lewe
        /// (tzn. to co lezy na stole)
        /// </summary>
        /// <param name="chosenSuit">Wybrany kolor.</param>
        /// <param name="curTrickCards">Karty lezace na stole.</param>
        /// <returns>Liczbe kart juz zagranych w danym kolorze.</returns>
        public int CardsPlayedInChosenSuit(CardSuit chosenSuit, List<Card> curTrickCards)
        {
            int cardSuit = 0;

            //obecna liczba kart w kolorze na stole
            cardSuit += curTrickCards.Sum(e =>
            {
                int result = 0;
                if (e.Suit == chosenSuit) result++;
                return result;
            });

            //liczba kart o wybranym kolorze w juz zagranych lewach
            cardSuit += PlayedCards.Deck.Cards.Sum(e =>
            {
                int result = 0;
                if (e.Suit == chosenSuit) result++;
                return result;
            });

            return cardSuit;
        }

        public List<Card> CardsLeftInSuit(CardSuit chosenSuit, List<Card> curTrickCards)
        {
            List<Card> LeftCardsChosenSuit = ExtraFunctions.AllCardsInChosenSuit(chosenSuit);

            for (int i = 0; i < curTrickCards.Count; i++)
            {
                if (curTrickCards[i].Suit == chosenSuit)
                {
                    //sprobuj usunac
                    try
                    {
                        int index = LeftCardsChosenSuit.FindIndex(e => e.Suit == curTrickCards[i].Suit
                                                                    && e.Rank == curTrickCards[i].Rank);
                        if (index >= 0)
                            LeftCardsChosenSuit.RemoveAt(index);
                    }
                    catch
                    {
                        //bledy zwiazane sa z niewystepowaniem elementu wiec nic nie trzeba robic
                    }
                }
            }

            for (int i = 0; i < PlayedCards.Deck.Cards.Count; i++)
            {
                if (PlayedCards.Deck.Cards[i].Suit == chosenSuit)
                {
                    int index = LeftCardsChosenSuit.FindIndex(e => e.Suit == PlayedCards.Deck.Cards[i].Suit
                                                                && e.Rank == PlayedCards.Deck.Cards[i].Rank);
                    if (index >= 0)
                        LeftCardsChosenSuit.RemoveAt(index);
                }
            }

            return LeftCardsChosenSuit;
        }

        public int CardsLeftInSuitOther(Deck playerPossibleMoves, CardSuit chosenSuit, List<Card> curTrickCards, out List<Card> outCardsLeft)
        {
            List<Card> cardsLeft = CardsLeftInSuit(chosenSuit, curTrickCards);

            for (int i = 0; i < playerPossibleMoves.Cards.Count; i++)
            {
                Card readCard = playerPossibleMoves.Cards[i];
                int found = cardsLeft.FindIndex(e => e.Suit == readCard.Suit && e.Rank == readCard.Rank);
                if (found >= 0)
                    cardsLeft.RemoveAt(found);
            }

            outCardsLeft = cardsLeft;
            return cardsLeft.Count;
        }

        /// <summary>
        /// Liczy punkty z obecnej lewy na stole.
        /// </summary>
        /// <param name="curTrickCards">Lista kart znajdujacych sie w obecnej lewie.</param>
        /// <returns>Liczba punktow w kartach na liscie podanej w argumencie.</returns>
        public int PointsOnTable(List<Card> curTrickCards)
        {
            int points = 0;

            points = curTrickCards.Sum(e =>
            {
                int result = 0;
                if (e.Suit == CardSuit.Hearts) result++;
                if (e.Suit == CardSuit.Spades && e.Rank == CardRank.Queen) result += 13;
                return result;
            });

            return points;
        }

        //Niewykorzystywana (dlatego nie skonczona)
        //probuje estymowac wartosc lewy 
        public float PointsPredictOnTable(List<Card> curTrickCards)
        {
            float predictedPoints = 0;
            predictedPoints += (float)PointsOnTable(curTrickCards);

            //brak estymacji punktow do konca lewy

            return predictedPoints;
        }

        /// <summary>
        /// Liczba punktow ktore zostaly na rekach graczy do konca rozdania.
        /// </summary>
        /// <returns>Liczba punktow.</returns>
        public int PointsLeftTillEnd()
        {
            int points = 0;

            points += PointsTaken(Player1UsedCards.CardsPlayed);
            points += PointsTaken(Player2UsedCards.CardsPlayed);
            points += PointsTaken(Player3UsedCards.CardsPlayed);
            points += PointsTaken(Player4UsedCards.CardsPlayed);

            //Maksymalna liczba punktow to 26. W points jest liczba punktow dotychczas zagranych
            //Zatem 26 - points to szukana liczba
            return 26 - points;
        }

        /// <summary>
        /// Zwraca srednia liczbe punktow na lewe do konca gry.
        /// Poczatkowo jest to 26 punktow na 13 lew = 2 punkty.
        /// </summary>
        /// <returns>Srednia liczba punktow na lewe do konca gry.</returns>
        public float PointsPerTrickTillEnd()
        {
            int tricksLeft = PlayedCards.Deck.Cards.Count;
            return (float)(52 - tricksLeft) / 4 / PointsLeftTillEnd();
        }

        /// <summary>
        /// Liczba lew do konca poza aktualnie rozgrywana. 
        /// Jezeli rozegralismy juz 6, teraz gramy 7 to zwroci 6.
        /// </summary>
        /// <returns>Liczbe pozostalych lew.</returns>
        public int TricksLeft()
        {
            int cardsLeft = PlayedCards.Deck.Cards.Count;
            return (52 - cardsLeft) / 4 - 1;
        }

        //wylicz w co wistowac
        //nie trzeba listy kart w obecnej lewie bo jest pusta
        public Card Greedy_WhistCard(Deck playerPossibleMoves)
        {
            Card whist = playerPossibleMoves.TopCard;
            //jezeli tylko 1 karta to zwroc (nie ma po co liczyc)
            if (playerPossibleMoves.Cards.Count == 1)
                return whist;

            //zwroc najslabsza karte w najdluzszym z mlodszych kolorow
            CardSuit longColor = playerPossibleMoves.LongestColor(true);
            whist = playerPossibleMoves.GetColor(longColor);

            return whist;
        }

        //Wygrana jak najmniejszym kosztem; najwyzsza karta pod wzgledem 
        //starszenstwa zapewniajaca nie wziecie lewy!
        public Card Greedy_MaxWinningCard(Deck playerPossibleMoves, List<Card> curTrickCards)
        {
            Card maxWin = playerPossibleMoves.TopCard;
            Card highestTillNow;

            //nie jestes wistujacym
            if (curTrickHighestCard(curTrickCards, out highestTillNow))
            {
                //jezeli masz do koloru
                if (playerPossibleMoves.TopCard.Suit == highestTillNow.Suit)
                {
                    //karty sa od najwiekszej do najmniejszej, zatem nie trzeba
                    //odwracac kolejnosci
                    //playerPossibleMoves.Cards.Reverse();
                    maxWin = playerPossibleMoves.Cards.FirstOrDefault(e =>
                    {
                        if (e.Suit == highestTillNow.Suit && highestTillNow.CompareToBool(e) ) return true;
                        else return false;
                    });

                    //nie masz nizszej karty niz najwyzsza karta na stole
                    if (maxWin == null)
                    {
                        maxWin = Greedy_MinLoosingCard(playerPossibleMoves, curTrickCards);
                        return maxWin;
                    }

                    //jest opcja ze nie mam nizszej niz na stole ale kolejni gracze moga miec wyzsze karty
                    //to do heurystyki
                }
                //jezeli nie masz do koloru
                else
                {
                    //jezeli mozesz zagrac dame pik to zagraj
                    if (playerPossibleMoves.Has(CardRank.Queen, CardSuit.Spades, out maxWin))
                    {
                        return maxWin;
                    }
                    //zagraj najwyzszego kiera
                    else if (playerPossibleMoves.HasColor(CardSuit.Hearts))
                    {
                        maxWin = playerPossibleMoves.GetColor(CardSuit.Hearts);
                        return maxWin;
                    }
                    //zagraj kolejna najwyzsza karte(wpierw karo, pozniej trefle)
                    else
                    {
                        maxWin = playerPossibleMoves.BottomCard;
                        return maxWin;
                    }
                }
            }
            //jestes wistujacym
            else
            {
                maxWin = Greedy_WhistCard(playerPossibleMoves);
                return maxWin;
            }

            return maxWin;
        }

        //Zminimalizuj swoja przegrana - jezeli wiesz ze wezmiesz lewe to zrzuc najstarsza
        //karte w kolorze ; musi byc ten sam kolor co wistujacego!
        //najlepiej uzywac tej funkcji jezei jest sie 3cim/4tym grajacym
        public Card Greedy_MinLoosingCard(Deck playerPossibleMoves, List<Card> curTrickCards)
        {
            Card minLoose = playerPossibleMoves.TopCard;
            Card highestTillNow;

             //nie jestes wistujacym
            if (curTrickHighestCard(curTrickCards, out highestTillNow))
            {
                //Jezeli masz dodac do koloru to dodaj najwieksza
                if (curTrickCards[0].Suit == playerPossibleMoves.TopCard.Suit)
                {
                    minLoose = playerPossibleMoves.BottomCard;
                    return minLoose;
                }
                else
                {
                    //jak nie masz do koloru to nie mozesz przegrac
                    minLoose = Greedy_MaxWinningCard(playerPossibleMoves, curTrickCards);
                    return minLoose;
                }
            }
            //jestes wistujacym
            else
            {
                minLoose = Greedy_WhistCard(playerPossibleMoves);
                return minLoose;
            }
        }

        /// <summary>
        /// Algorytm robiacy losowy ruch z wszystkich mozliwych
        /// </summary>
        /// <param name="playerPossibleMoves">Zbior wszystkich mozliwych ruchow gracza.</param>
        /// <param name="trickPosition">Pozycja w lewie; 1 = wistjacy, itd az do 4 - ostatni.</param>
        /// <param name="curTrickCards">Lista kart ktore zostaly zagrane w obecnej lewie (dlugosc od 0 do 3)</param>
        public Card RandomAlgorithm(Deck playerPossibleMoves, int trickPosition, List<Card> curTrickCards)
        {
            //w przypadku algorytmu losowego nic nie bierzemy pod uwage - z wszystkich mozliwych ruchow bierzemy jakikolwiek
            Card randomCard = playerPossibleMoves.RandomCard;            

            currentTrick.Add(randomCard);
            return randomCard;
        }

        public Card GreedyAlgorithm(Deck playerPossibleMoves, int trickPosition, List<Card> curTrickCards)
        {
            //probuj caly czas maksymalnie wygrywac
            Card greedyCard = Greedy_MaxWinningCard(playerPossibleMoves, curTrickCards);

            currentTrick.Add(greedyCard);
            return greedyCard;
        }

        //HeuristicAlgorithm znajduje sie w AI_Heuristic.cs

        /// <summary>
        /// Zwraca ruch wg wybranego algorytmu.
        /// </summary>
        /// <param name="playerPossibleMoves">Mozliwe ruchy gracza.</param>
        /// <param name="trickPosition">Pozycja w lewie.</param>
        /// <param name="curTrickCards">Karty w lewie(od 0 do 3).</param>
        /// <param name="alg">Wybrany algorytm - enum.</param>
        /// <returns>Zwraca wyliczona karte wg wybranego algorytmu.</returns>
        public Card ChooseAlgorithm(Deck playerPossibleMoves, int trickPosition, List<Card> curTrickCards, AlgorithmTypes alg)
        {
            switch (alg)
            {
                case AlgorithmTypes.Random:
                    {
                        return RandomAlgorithm(playerPossibleMoves, trickPosition, curTrickCards);
                    }
                case AlgorithmTypes.Greedy:
                    {
                        return GreedyAlgorithm(playerPossibleMoves, trickPosition, curTrickCards);
                    }
                case AlgorithmTypes.Heuristic:
                    {
                        //HeuristicAlgorithm(...) znajduje sie w AI_Heuristic.cs
                        return HeuristicAlgorithm(playerPossibleMoves, trickPosition, curTrickCards);
                    }
                default:
                    return RandomAlgorithm(playerPossibleMoves, trickPosition, curTrickCards);
            }
        }
    }
}
