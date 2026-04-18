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

            // The constructor for the PileViewModel class takes a CardPile object as a parameter
        }

        public void Refresh()
        {
            Cards.Clear();
            foreach (var card in Model.cards)
            {
                Cards.Add(new CardViewModel(card));
            }

            OnPropertyChanged(nameof(TopCardImagePath));

            /* 
             * This method clears the existing Cards collection and repopulates it based on the current state of the Model's cards. 
             * It also raises a PropertyChanged event for the TopCardImagePath property, 
             * which allows the UI to update the displayed image for the top card of the pile whenever the pile's contents change.
             */
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

            /* 
            * The TopCardImagePath property is a read-only property that determines the file path of the image to be displayed for the top card of the pile. 
            * It checks if the pile is empty and returns a foundation-specific empty image if applicable, or a generic empty image for non-foundation piles. 
            * If the pile is not empty, it checks if the top card is face down and returns the back image if so. 
            * Otherwise, it constructs the file path for the face image of the top card based on its suit and rank.
            */
        }

        private static string PackUri(string relativePath)
        {
            // assembly name is SolitaireFront; adjust if different
            return $"pack://application:,,,/SolitaireFront;component/{relativePath}";

            /* 
            * The PackUri method takes a relative file path as input and constructs a pack URI that can be used to access the resource within the application. 
            * The URI format is "pack://application:,,,/AssemblyName;component/RelativePath", 
            * where "AssemblyName" is the name of the assembly containing the resource (in this case, 
            * "SolitaireFront") and "RelativePath" is the path to the resource within the assembly. 
            * This method allows for consistent construction of pack URIs throughout the application.
            */
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

            /* 
            * The ResourceExists method checks if a resource exists at the specified pack URI. 
            * It attempts to create a Uri object from the provided pack URI and then uses Application.GetResourceStream to check if the resource can be accessed. 
            * If the resource is found, it returns true; if an exception occurs 
            * (e.g., if the URI is invalid or the resource does not exist), it catches the exception and returns false.
            */
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

            /* 
            * The OnPropertyChanged method is a helper method that raises the PropertyChanged event for a given property name. 
            * This allows the UI to be notified when a property value changes, so that it can update the display accordingly. 
            * The method checks if there are any subscribers to the PropertyChanged event and, if so, invokes the event with the appropriate arguments.
            */
        }
    }
}
