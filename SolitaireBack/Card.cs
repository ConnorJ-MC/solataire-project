using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SolitaireBack
{
    public enum Suit
    {
        diamonds,
        spades,
        hearts,
        clubs
    }

    public class Card : INotifyPropertyChanged
    {
        public Suit suit { get; }

        public int rank { get; }

        //#1
        private bool _isTopCard;
        //public string TextureName => $"card-{suit}-{rank}";
        public string ImagePath => $"/Recources/Playing Cards/card-{suit}-{rank}.png";
        // The UI can bind to this to know if it should trigger the flip animation
        public double CurrentAngle => isFaceUp ? 0 : 180;
        public bool IsTopCard
        {
            get => _isTopCard;
            set { _isTopCard = value; OnPropertyChanged(); }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }



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
