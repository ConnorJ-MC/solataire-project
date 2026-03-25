using SolitaireBack;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolitaireFront
{
    public class PileViewModel
    {
        public ObservableCollection<CardViewModel> Cards { get; }
        public CardPile Model { get; }

        public PileViewModel(CardPile model)
        {
            Model = model;
            Cards = new ObservableCollection<CardViewModel
                >(model.cards.Select(c => new CardViewModel(c))
            );
        }

        public void Refresh()
        {
            Cards.Clear();
            foreach (var card in Model.cards)
            {
                Cards.Add(new CardViewModel(card));
            }
        }
    }
}
