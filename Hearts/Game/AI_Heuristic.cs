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
    public partial class MainWindow : Window
    {
        /* Wszystkie funkcje dotyczace herustyki znajduja
         * sie w tym pliku. W AI.cs znajduja sie wszystkie 
         * metody pomocnicze oraz proste algorytmy Random
         * i Greedy
         */

        //MOZNA JESZCZE POPRAWIC
        //wylicz w co wistowac
        //nie trzeba listy kart w obecnej lewie bo jest pusta
        //to trzeba mocno poprawic
        public Card Heuristic_WhistCard(Deck playerPossibleMoves)
        {
            Card whist = playerPossibleMoves.TopCard;
            Card otherWhist = whist;
            List<Card> cardsLeft = new List<Card>();
            //jezeli tylko 1 karta to zwroc (nie ma po co liczyc)
            if (playerPossibleMoves.Cards.Count == 1)
                return whist;

            //zwroc najslabsza karte w najdluzszym z mlodszych kolorow
            CardSuit shortColor = playerPossibleMoves.ShortestColor(true);
            whist = playerPossibleMoves.GetColor(shortColor);

            //jezeli chcesz zawistowac w wysoka karte to mozesz to zrobic pod warunkiem ze zostalo sporo kart
            //w tym kolorze
            int numbOfColorLeft = CardsLeftInSuitOther(playerPossibleMoves, shortColor, currentTrick, out cardsLeft);
            if ( ! (numbOfColorLeft >= 8) || whist.Suit == CardSuit.Spades || whist.Suit == CardSuit.Hearts)
            {
                int i = 1;
                while (whist.cardLevel == CardGrade.High)
                {
                    shortColor = playerPossibleMoves.ShortestColor(true, i);
                    otherWhist = playerPossibleMoves.GetColor(shortColor);
                    if (otherWhist == whist)
                        break;

                    whist = otherWhist;
                    i++;
                }

            }
            while (playerPossibleMoves.Cards.Count > 1 && whist.Rank == CardRank.Queen && whist.Suit == CardSuit.Spades)
                whist = playerPossibleMoves.RandomCard;

            return whist;
        }

        //MOZNA JESZCZE POPRAWIC - patrz "//jezeli ISTNIEJE szansa ze gracze po tobie nie maja renonsu"
        //Wygrana jak najmniejszym kosztem; najwyzsza karta pod wzgledem 
        //starszenstwa zapewniajaca nie wziecie lewy!
        public Card Heuristic_TryToWinCard(Deck playerPossibleMoves, List<Card> curTrickCards, int pointsOnTable)
        {
            Card heuristicWin = playerPossibleMoves.TopCard;
            Card highestTillNow;

            //Na pewno nie jestes wistujacym -> HeuristicAlgorithm sie tym zajmuje
            curTrickHighestCard(curTrickCards, out highestTillNow);
            
            //jezeli masz do koloru
            if (highestTillNow.Suit == playerPossibleMoves.TopCard.Suit)
            {
                //karty sa od najwiekszej do najmniejszej, zatem nie trzeba odwracac kolejnosci
                //playerPossibleMoves.Cards.Reverse();
                heuristicWin = playerPossibleMoves.Cards.FirstOrDefault(e =>
                {
                    if (e.Suit == highestTillNow.Suit && highestTillNow.CompareToBool(e)) return true;
                    else return false;
                });
                if (heuristicWin != null)
                    return heuristicWin;

                bool possibleColor = false;
                //nie masz nizszej niz na stole ale kolejni gracze moga miec wyzsze karty niz twoja najnizsza
                for (int i = curTrickCards.Count; i < 3; i++)
                {
                    if (!PlayersUsedCards[IndexOfPlayer(i)].CardRenons[highestTillNow.Suit])
                        possibleColor = true;
                }

                //jezeli ISTNIEJE szansa ze gracze po tobie nie maja renonsu
                if (possibleColor)
                {
                    //mozna to poprawic - obecnie sprawdza czy masz nizsza w pozostalym kolorze
                    //a przeciez nastepni przeciwnicy wcale nie musza miec najnizszej karty
                    List<Card> cardsLeft = CardsLeftInSuit(highestTillNow.Suit, curTrickCards);
                    //odejmij karty ktore masz na rece
                    for (int i = 0; i < playerPossibleMoves.Cards.Count; i++)
                    {
                        Card readCard = playerPossibleMoves.Cards[i];
                        int found = cardsLeft.FindIndex(e => e.Suit == readCard.Suit && e.Rank == readCard.Rank);
                        if (found >= 0)
                            cardsLeft.RemoveAt(found);
                    }

                    if (cardsLeft.Count > 0)
                    {
                        //to trzeba poprawic
                        for (int i = 0; i < playerPossibleMoves.Cards.Count; i++)
                        {

                            //wez najstarsza mozliwa karte nizsza od wszystkich pozostalych kart
                            if (cardsLeft.Last().CompareToBool(playerPossibleMoves.Cards[i]))
                            {
                                heuristicWin = playerPossibleMoves.Cards[i];
                                return heuristicWin;
                            }

                        }
                    }
                    //masz ostatnia karte w kolorze, musisz ja zagrac
                    else
                    {
                        heuristicWin = Heuristic_MinLoosingCard(playerPossibleMoves, curTrickCards, pointsOnTable);
                        return heuristicWin;
                    }

                    //jezeli jest dama na stole to zrob wszystko byle tylko jej nie wziac
                    if (pointsOnTable > 13)
                    {
                        heuristicWin = playerPossibleMoves.Cards.Last();
                        return heuristicWin;
                    }
                }
                //nie masz nizszej karty niz najwyzsza karta na stole
                if (heuristicWin == null)
                {
                    heuristicWin = Heuristic_MinLoosingCard(playerPossibleMoves, curTrickCards, pointsOnTable);
                    return heuristicWin;
                }
            }
            //jezeli nie masz do koloru
            else
            {
                //jezeli mozesz zagrac dame pik to zagraj
                if (playerPossibleMoves.Has(CardRank.Queen, CardSuit.Spades, out heuristicWin))
                {
                    return heuristicWin;
                }
                //to trzeba zmienic - lepiej grac krotkosci
                //zagraj najwyzszego kiera
                else if (playerPossibleMoves.HasColor(CardSuit.Hearts))
                {
                    heuristicWin = playerPossibleMoves.GetColor(CardSuit.Hearts);
                    return heuristicWin;
                }
                //zagraj kolejna najwyzsza karte w krotkim kolorze
                else
                {
                    CardSuit shortColor = playerPossibleMoves.ShortestColor();
                    Card tempHeuristicWin;
                    heuristicWin = playerPossibleMoves.GetColor(shortColor);

                    int i = 1;
                    while (heuristicWin.cardLevel == CardGrade.Low)
                    {
                        shortColor = playerPossibleMoves.ShortestColor(true, i);
                        tempHeuristicWin = playerPossibleMoves.GetColor(shortColor);
                        if (tempHeuristicWin == heuristicWin)
                            break;

                        heuristicWin = tempHeuristicWin;
                        i++;
                    }

                    heuristicWin = playerPossibleMoves.BottomCard;
                    return heuristicWin;
                }
            }            

            return heuristicWin;
        }

        //Zminimalizuj swoja przegrana - jezeli przewidujesz(!) ze wezmiesz lewe to zrzuc najstarsza
        //karte w kolorze ; musi byc ten sam kolor co wistujacego!
        //najlepiej uzywac tej funkcji jezei jest sie 3cim/4tym grajacym
        public Card Heuristic_MinLoosingCard(Deck playerPossibleMoves, List<Card> curTrickCards, int pointsOnTable)
        {
            Card minLoose = playerPossibleMoves.TopCard;
            Card highestTillNow;

            //Na pewno nie jestes wistujacym -> HeuristicAlgorithm sie tym zajmuje
            curTrickHighestCard(curTrickCards, out highestTillNow);

            //Jezeli masz dodac do koloru to dodaj najwieksza
            if (highestTillNow.Suit == playerPossibleMoves.TopCard.Suit)
            {
                //Przy takim sortowaniu najwieksze karty sa na spodzie
                minLoose = playerPossibleMoves.BottomCard;
                //jezeli mialbys przypadkiem zagrac dame
                if (minLoose == new Card(CardRank.Queen, CardSuit.Spades))
                {
                    //jezeli mozesz zagrac COS innego niz dame to to zagraj
                    if (playerPossibleMoves.Cards.Count > 1)
                    {
                        minLoose = playerPossibleMoves.Cards[1]; //kolejnego pika po damie
                    }
                }
                return minLoose;
            }
            else
            {
                //jak nie masz do koloru to nie mozesz przegrac
                minLoose = Heuristic_TryToWinCard(playerPossibleMoves, curTrickCards, pointsOnTable);
                return minLoose;
            }            
        }

        public Card HeuristicAlgorithm(Deck playerPossibleMoves, int trickPosition, List<Card> curTrickCards)
        {
            Card heuristicCard = playerPossibleMoves.TopCard;
            //wez pod uwage krotkosci
            //wez pod uwage liczbe lew do konca
            //wez pod uwage zagrane karty
            //wez pod uwage przyszlosc ?

            //Card minLoose = MinLoosingCard(playerPossibleMoves, currentTrick);
            //Card maxWin = MaxWinningCard(playerPossibleMoves, currentTrick);
            //int howMany = CardsPlayedInChosenSuit(randomCard.Suit, currentTrick);
            //List<Card> cardsleft = CardsLeftInChosenSuit(randomCard.Suit, currentTrick);

            int pointsOnTable = PointsOnTable(curTrickCards);
            int pointsTillEnd = PointsLeftTillEnd();
            //poza obecna!
            int tricksLeft = (52 - PlayedCards.Deck.Cards.Count) / 4;
            Card highestTillNow;
            curTrickHighestCard(curTrickCards, out highestTillNow);

            //jestes wistujacym
            if (trickPosition == 0)
            {
                heuristicCard = Heuristic_WhistCard(playerPossibleMoves);
            }
            //jezeli jestes ostatni
            //popracowac nad tym jeszcze
            else if (trickPosition == 3)
            {
                if (pointsOnTable <= 1 && curTrickCards[0].Suit != CardSuit.Spades)
                {
                    heuristicCard = Heuristic_MinLoosingCard(playerPossibleMoves, curTrickCards, pointsOnTable);
                }
                //jezeli ktos zagral Asa lub Krola pik to zruc mu dame
                else if(playerPossibleMoves.Has(CardRank.Queen, CardSuit.Spades))
                {
                    if (curTrickCards.Exists(e => e == new Card(CardRank.Ace, CardSuit.Spades) || e == new Card(CardRank.King, CardSuit.Spades)))
                    {
                        heuristicCard = playerPossibleMoves.GetCard(CardRank.Queen, CardSuit.Spades);
                    }
                    else
                        heuristicCard = Heuristic_MinLoosingCard(playerPossibleMoves, curTrickCards, pointsOnTable);
                }
                else
                {
                    heuristicCard = Heuristic_TryToWinCard(playerPossibleMoves, curTrickCards, pointsOnTable);
                }
            }
            //jezeli jestes po wistujacym
            else if (trickPosition == 1)
            {
                if (pointsTillEnd >= 13 || pointsOnTable >= 13)
                {
                    heuristicCard = Heuristic_TryToWinCard(playerPossibleMoves, curTrickCards, pointsOnTable);
                }
                else
                {
                    heuristicCard = Heuristic_TryToWinCard(playerPossibleMoves, curTrickCards, pointsOnTable);
                }
            }
            //jezeli jestes przedostatni
            else if (trickPosition == 2)
            {
                if (pointsOnTable < 1 && (float)pointsTillEnd / tricksLeft <= 2 
                                      && PlayedCards.Deck.Has(CardRank.Queen, CardSuit.Spades))
                {
                    heuristicCard = Heuristic_MinLoosingCard(playerPossibleMoves, curTrickCards, pointsOnTable);
                }
                else
                {
                    heuristicCard = Heuristic_TryToWinCard(playerPossibleMoves, curTrickCards, pointsOnTable);
                }

            }
            //blad
            else
            {
                MessageBox.Show("Cos jest nie tak, nigdy nie powinienem tu byc", "Blad");
                Console.WriteLine("Cos jest nie tak, nigdy nie powinienem tu byc");
            }

            currentTrick.Add(heuristicCard);
            return heuristicCard;
        }
    }
}
