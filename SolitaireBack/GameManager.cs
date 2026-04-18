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

            /*
             * The GameManager class is implemented as a singleton to ensure that there is only one instance of the game state throughout the application. 
             * It manages the core components of the Solitaire game, including the deck, tableaus, foundations, stock, and waste piles. 
             * The constructor initializes the login manager instance for handling player data and authentication.
             */
        }

        public IReadOnlyList<Tableau> Tableaus => tableaus;
        public IReadOnlyList<Foundation> Foundations => foundations;
        public Stock Stock => stock;
        public Waste Waste => waste;
        /*
         * These properties provide read-only access to the game components,
         * allowing other parts of the application (such as the UI) to interact with the game state without directly modifying it.
         */

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

            /*
             * The StartGame method initializes the game state for a new game. 
             * If the player has chosen to gamble, it deducts 52 from their balance. 
             * It then creates a new deck, initializes the tableaus, foundations, stock, and waste piles, and resets the move counter. 
             * Finally, it calls the deal method to set up the initial card layout on the table.
             */
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

            /*
            * The deal method is responsible for distributing the cards from the deck to the tableaus and stock at the start of the game. 
            * It uses nested loops to place the appropriate number of cards in each tableau, ensuring that only the top card of each tableau is face-up. 
            * After dealing to the tableaus, it moves any remaining cards in the deck to the stock pile.
            */
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

            /*
            * The TryMove method attempts to move a card (and any cards stacked on top of it) from its current location to a target pile. 
            * It first identifies the source pile containing the card, retrieves the stack of cards to be moved, and checks if the target pile can accept the move. 
            * If all checks pass, it removes the stack from the source, ensures they are face-up, and adds them to the target pile. 
            * It also handles flipping the new top card of the source pile if necessary and applies Vegas scoring rules if applicable.
            */
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

            /*
             * The findPileContaining method searches through all the piles in the game (tableaus, 
             * foundations, stock, and waste) to find which one contains the specified card. 
             * It returns the pile if found, or null if the card is not present in any pile.
             */
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
            }

            return false;

            /*
             * The applyVegasScoring method implements the scoring rules for Vegas mode. 
             * If the player is gambling and moves a card from a non-foundation pile to a foundation, it adds 5 to their balance. 
             * It returns true if scoring was applied, or false otherwise.
             */
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

            /*
             * The winCheck method verifies if the player has won the game by checking each foundation pile. 
             * It ensures that each foundation contains exactly 13 cards, 
             * and that those cards are in the correct order (Ace to King) and match the suit of the foundation. 
             * If all foundations meet these conditions, it returns true, indicating a win; otherwise, it returns false.
             */
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


            return login.savePlayerData(login.currentPlayer);

            /*
            * The resetGame method is responsible for resetting the game state to start a new game. 
            * If the player is not a guest, it updates their game statistics (games played, won, lost) based on the outcome of the current game. 
            * It then clears all game components and resets the move counter. Finally, it saves the player's data to persist their updated statistics.
            */
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

            /*
            * The drawFromStock method handles the action of drawing cards from the stock pile. 
            * If the stock is empty, it attempts to recycle the waste back into the stock. 
            * If the stock has cards, it draws a number of cards based on the selected difficulty level (at least one), 
            * flips them face-up, increments the move counter, and adds them to the waste pile.
            */
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

            /*
             * The stockRecycle method is responsible for recycling the waste pile back into the stock pile when the stock is empty. 
             * It checks if the stock is empty and the waste is not empty, then retrieves the cards from the waste, 
             * resets them in the stock, and returns true if successful. 
             * If the conditions are not met or if there are no cards to return, it returns false.
             */
        }
    }
}

