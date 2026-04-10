using SolitaireBack;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows; // for Application
using System.Windows.Resources;

namespace SolitaireFront
{
    public class PileViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<CardViewModel> Cards { get; }
        public CardPile Model { get; }

        public PileViewModel(CardPile model)
        {
            Model = model;
            Cards = new ObservableCollection<CardViewModel>(
                model.cards.Select(c => new CardViewModel(c))
            );
        }

        public void Refresh()
        {
            Cards.Clear();
            foreach (var card in Model.cards)
            {
                Cards.Add(new CardViewModel(card));
            }

            OnPropertyChanged(nameof(TopCardImagePath));
        }

        // Use pack URIs and fall back if suit-specific resource is missing
        public string TopCardImagePath
        {
            get
            {
                // If pile empty -> show foundation-specific empty image when applicable
                if (Model.cards == null || Model.cards.Count == 0)
                {
                    if (Model is SolitaireBack.CardPiles.Foundation foundation)
                    {
                        string rel = $"Recources/Playing Cards/card-foundation-{foundation.suit}.png";
                        string pack = PackUri(rel);
                        if (ResourceExists(pack)) return pack;

                        // fallback to generic foundation-default
                        return PackUri("Recources/Playing Cards/card-foundation-default.png");
                    }

                    // generic empty image for non-foundation piles
                    return PackUri("Recources/Playing Cards/card-foundation-default.png");
                }

                var top = Model.cards.Last();
                if (!top.isFaceUp)
                {
                    // show back image using pack uri
                    return PackUri("Recources/Playing Cards/card-back2.png");
                }

                // show the face of the top card
                string faceRel = $"Recources/Playing Cards/card-{top.suit}-{top.rank}.png";
                return PackUri(faceRel);
            }
        }

        private static string PackUri(string relativePath)
        {
            // assembly name is SolitaireFront; adjust if different
            return $"pack://application:,,,/SolitaireFront;component/{relativePath}";
        }

        private static bool ResourceExists(string packUri)
        {
            try
            {
                var uri = new Uri(packUri, UriKind.Absolute);
                StreamResourceInfo info = Application.GetResourceStream(uri);
                return info != null;
            }
            catch
            {
                return false;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
