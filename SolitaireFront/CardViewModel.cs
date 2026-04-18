using SolitaireBack;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;
using System.Xml.Linq;

namespace SolitaireFront
{
    public class CardViewModel : INotifyPropertyChanged
    {
        public Card Model { get; }

        public string ImagePath => Model.isFaceUp
            ? $"Recources/Playing Cards/card-{Model.suit}-{Model.rank}.png"
            : "Recources/Playing Cards/card-back2.png";
        // The ImagePath property returns the file path to the image that should be displayed for this card.

        // new: top / interactable marker used for waste UI
        private bool _isTop;
        public bool IsTop
        {
            get => _isTop;
            set
            {
                if (_isTop == value) return;
                _isTop = value;
                OnPropertyChanged(nameof(IsTop));
            }

            // The IsTop property is a boolean that indicates whether this card is the top card in a pile (e.g., the waste pile).
        }

        public CardViewModel(Card model)
        {
            Model = model;

            // The constructor for the CardViewModel class takes a Card object as a parameter and initializes the Model property with it.
        }

        public void Refresh()
        {
            OnPropertyChanged(nameof(ImagePath));
            OnPropertyChanged(nameof(Model.isFaceUp));
            OnPropertyChanged(nameof(IsTop));

            /* 
             * The Refresh method is used to notify the UI that certain properties of the card have changed and that the UI should update accordingly. 
             * It raises the PropertyChanged event for the ImagePath, Model.isFaceUp, and IsTop properties, 
             * which will trigger any data bindings in the UI to refresh and display the updated information.
             */
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

            /* 
            * The OnPropertyChanged method is a helper method that raises the PropertyChanged event with the name of the property that has changed. 
            * This allows the UI to know which property has been updated and to refresh the display accordingly.
            */
        }

        /* 
        * The CardViewModel class implements the INotifyPropertyChanged interface, which allows it to notify the UI when certain properties have changed. 
        * This is important for data binding in WPF, as it enables the UI to automatically update when the underlying data changes. 
        * The OnPropertyChanged method is a helper method that raises the PropertyChanged event with the name of the property that has changed.
        */
    }
}
