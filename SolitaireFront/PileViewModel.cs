using SolitaireBack;
using System.Collections.ObjectModel;

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
                >(model.Cards.Select(c => new CardViewModel(c))
            );
        }

        public void Refresh()
        {
            Cards.Clear();
            foreach (var card in Model.Cards)
            {
                Cards.Add(new CardViewModel(card));
            }
        }
    }
}
