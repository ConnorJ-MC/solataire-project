using System.Diagnostics;
using SolitaireBack.CardPiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolitaireBack
{
    public class GameManager
    {
        private Deck deck;
        private List<Tableau> tableaus = new List<Tableau>();
        private List<Foundation> foundations = new List<Foundation>();
        private Stock stock;
        private Waste waste;
        public int movesTaken;

        private LoginManager login;

        public static GameManager Instance { get; } = new GameManager();
        private GameManager()
        {
            login = LoginManager.Instance;
        }

        public IReadOnlyList<Tableau> Tableaus => tableaus;
        public IReadOnlyList<Foundation> Foundations => foundations;
        public Stock Stock => stock;
        public Waste Waste => waste;

        public bool StartGame()
        {
            if (login.gamble)
            {
                login.currentPlayer.balance -= 52;
            }

            deck = new Deck();
            if (deck == null) return false;

            tableaus = new List<Tableau>();
            for (int i = 0; i < 7; i++)
            {
                tableaus.Add(new Tableau());
            }
            foundations = new List<Foundation>();
            foreach (Suit suit in Enum.GetValues(typeof(Suit)))
            {
                foundations.Add(new Foundation(suit));
            }
            stock = new Stock();
            waste = new Waste();

            movesTaken = 0;
            return deal();
        }
        public bool deal()
        {
            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j <= i; j++)
                {
                    Card card = deck.draw();
                    if (card == null) return false;
                    tableaus[i].addCard(card);
                }
                tableaus[i].peakTop();
            }

            while (!deck.isEmpty())
            {
                stock.addCard(deck.draw());
            }
            return true;
        }

        public bool TryMove(Card c, CardPile target)
        {
            CardPile source = findPileContaining(c);
            if (source == null) return false;

            List<Card> stack = source.getStackFrom(c);
            if (stack == null || stack.Count == 0) return false;

            if (!target.canAccept(stack[0])) return false;

            List<Card> removedStack = source.removeStack(c);
            if (removedStack == null) return false;

            // Ensure moved cards are face-up so UI converters render them correctly
            foreach (var movedCard in removedStack)
            {
                movedCard.isFaceUp = true;
            }

            if (!target.addStack(removedStack)) return false;

            // debug: if moved to foundation, log details
            if (target is Foundation f)
            {
                Debug.WriteLine($"Added {removedStack.Count} card(s) to Foundation {f.suit}. Foundation count now: {f.cards.Count}");
            }

            // flip new top of source if present
            try
            {
                if (!source.isEmpty())
                {
                    source.peakTop();
                }
            }
            catch { }

            applyVegasScoring(source, target);
            movesTaken++;
            return true;
        }

        private CardPile findPileContaining(Card c)
        {
            foreach (var t in tableaus)
            {
                if (t.contains(c)) return t;
            }

            foreach (var f in foundations)
            {
                if (f.contains(c)) return f;
            }

            if (stock.contains(c)) return stock;

            if (waste.contains(c)) return waste;

            return null;
        }

        public bool applyVegasScoring(CardPile source, CardPile target)
        {
            if (login.gamble)
            {
                if (source is not Foundation && target is Foundation)
                {
                    login.currentPlayer.balance += 5;
                    return true;
                }
                else if (source is Foundation && target is not Foundation)
                {
                    login.currentPlayer.balance -= 5;
                    return true;
                }
            }

            return false;
        }
        public bool winCheck()
        {
            foreach (Foundation f in foundations)
            {
                if (f.cards.Count != 13) return false;

                for (int i = 0; i < 13; i++)
                {
                    Card card = f.cards[i];

                    if (card.rank != i + 1) return false;
                    if (card.suit != f.suit) return false;
                }
            }

            return true;
        }

        public bool resetGame()
        {
            if (!login.isGuest)
            {
                login.currentPlayer.gamesP += 1;
                if (winCheck()) login.currentPlayer.gamesW += 1;
                else login.currentPlayer.gamesL += 1;
            }

            deck = null;
            tableaus.Clear();
            foundations.Clear();
            stock = null;
            waste = null;
            movesTaken = 0;


            login.jsonSaved = false;
            return login.savePlayerData(login.currentPlayer);
        }

        public bool drawFromStock()
        {
            if (stock.isEmpty()) return stockRecycle();
            else
            {
                // ensure we draw at least one card
                int drawCount = Math.Max(1, (int)login.difficulty);
                List<Card> drawnCards = stock.draw(drawCount);
                if (drawnCards == null || drawnCards.Count == 0) return false;

                foreach (Card card in drawnCards) card.flip();
                movesTaken++;   
                return waste.addStack(drawnCards);
            }
        }

        public bool stockRecycle()
        {
            if (stock.isEmpty() && !waste.isEmpty())
            {
                List<Card> cardsToReturn = waste.reset();
                if (cardsToReturn == null || cardsToReturn.Count == 0) return false;
                return stock.reset(cardsToReturn);
            }
            return false;
        }
    }
}

