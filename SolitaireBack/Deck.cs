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
        }

        public Card draw()
        {
                if (isEmpty())
                    throw new InvalidOperationException("Cannot draw from an empty deck.");
    
                Card topCard = cards[cards.Count - 1];
                cards.RemoveAt(cards.Count - 1);
                return topCard;
        }

        public List<Card> drawCards(int amount) //will either be 1 or 3, but can be more for testing purposes
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
        }

        public bool isEmpty() => cards.Count == 0;
    }
}
