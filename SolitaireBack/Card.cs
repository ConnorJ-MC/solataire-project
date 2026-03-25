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
    }

    public class Card
    {
        public Suit suit { get; }

        public int rank { get; }

        public string TextureName => $"card-{suit}-{rank}";

        public static string BackTextureName => "card-back2";

        public bool isFaceUp { get; set; } = false;

        public Card(Suit suit, int rank)
        {
            if (rank < 1 || rank > 13)
                throw new ArgumentOutOfRangeException(nameof(rank), "Rank must be between 1 and 13.");

            this.suit = suit;
            this.rank = rank;
        }

        public void flip() => isFaceUp = !isFaceUp;

        public bool isRed() => suit == Suit.diamonds || suit == Suit.hearts;
    }
}
