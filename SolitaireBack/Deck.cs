using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolitaireBack
{
    internal class Deck
    {
        private readonly List<Card> cards;

        public Deck()
        {
            cards = new List<Card>();
            initialise();
            shuffle();

            // The constructor initializes the deck by creating a standard set of 52 playing cards and shuffling them.
        }
        private void initialise()
        {
            foreach (Suit suit in Enum.GetValues(typeof(Suit)))
            {
                for (int rank = 1; rank <= 13; rank++)
                {
                    cards.Add(new Card(suit, rank));
                }
            }

            /* 
             * The initialise method populates the deck with a standard set of 52 playing cards, 
             * consisting of 13 ranks (Ace through King) in each of the four suits (diamonds, spades, hearts, clubs).
             */
        }

        private void shuffle()
        {
            Random rng = new Random();
            int n = cards.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                Card value = cards[k];
                cards[k] = cards[n];
                cards[n] = value;
            }

            // The shuffle method implements the Fisher-Yates algorithm to randomly shuffle the cards in the deck.
        }

        public Card draw()
        {
            if (isEmpty())
                throw new InvalidOperationException("Cannot draw from an empty deck.");

            Card topCard = cards[cards.Count - 1];
            cards.RemoveAt(cards.Count - 1);
            return topCard;

            /* 
            * The draw method removes and returns the top card from the deck. 
            * It checks if the deck is empty before attempting to draw, throwing an exception if there are no cards left.
            */
        }

        public List<Card> drawCards(int amount)
        {
            if (amount < 1)
                throw new ArgumentException("Amount must be at least 1.");
            if (amount > cards.Count)
                throw new InvalidOperationException("Not enough cards in the deck to draw the requested amount.");
            List<Card> drawnCards = new List<Card>();
            for (int i = 0; i < amount; i++)
            {
                drawnCards.Add(draw());
            }
            return drawnCards;

            /*
             * The drawCards method allows drawing multiple cards at once. 
             * It checks that the requested amount is valid and that there are enough cards in the deck before drawing.
             * It returns a list of drawn cards.
             */
        }

        public bool isEmpty() => cards.Count == 0; // A method that returns true if the deck is empty (i.e., contains no cards) and false otherwise.
    }
}
