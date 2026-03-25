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

        public CardViewModel(Card model)
        {
            Model = model;
        }

        public void Refresh()
        {
            OnPropertyChanged(nameof(ImagePath));
            OnPropertyChanged(nameof(Model.isFaceUp));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
