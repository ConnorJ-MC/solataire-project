using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolitaireBack
{
    public enum Suit
    {
        diamonds,
        spades,
        hearts,
        clubs

        // The four suits of a standard deck of playing cards. The order is not important, but they are commonly represented in this way. 
    }

    public class Card
    {
        public Suit suit { get; }

        public int rank { get; }

        public string TextureName => $"card-{suit}-{rank}"; // The name of the texture for this card, which can be used to look up the appropriate image for the card in a texture atlas or similar resource.


        public bool isFaceUp { get; set; } = false;

        public Card(Suit suit, int rank)
        {
            if (rank < 1 || rank > 13)
                throw new ArgumentOutOfRangeException(nameof(rank), "Rank must be between 1 and 13.");

            this.suit = suit;
            this.rank = rank;

            /* The constructor for the Card class, which takes a Suit and an integer rank as parameters.
             * The rank must be between 1 and 13, where 1 represents an Ace, 11 represents a Jack, 12 represents a Queen, and 13 represents a King.
             * The suit can be any of the four suits defined in the Suit enum. The card is initialized as face down (isFaceUp = false) by default.
             */
        }

        public void flip() => isFaceUp = !isFaceUp; // A method to flip the card, which toggles the isFaceUp property between true and false.

        public bool isRed => suit == Suit.diamonds || suit == Suit.hearts; // A property that returns true if the card is a red suit (diamonds or hearts) and false if it is a black suit (spades or clubs).
    }
}
