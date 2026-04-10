using SolitaireBack;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace SolitaireFront
{
    internal class Page1ViewModel : INotifyPropertyChanged
    {
        public GameManager Game { get; }

        public PileViewModel Stock { get; }
        public PileViewModel Waste { get; }
        public ObservableCollection<PileViewModel> Foundations { get; }
        public ObservableCollection<PileViewModel> Tableau { get; }

        // expose only the top N waste cards for UI (VMs from Waste.Cards)
        public ObservableCollection<CardViewModel> WasteTop { get; } = new ObservableCollection<CardViewModel>();

        public Page1ViewModel(GameManager gm)
        {
            Game = gm;

            Stock = new PileViewModel(Game.Stock);
            Waste = new PileViewModel(Game.Waste);

            Foundations = new ObservableCollection<PileViewModel>(
                Game.Foundations.Select(f => new PileViewModel(f))
            );

            Tableau = new ObservableCollection<PileViewModel>(
                Game.Tableaus.Select(t => new PileViewModel(t))
            );

            Debug.WriteLine("Tableaus: " + Game.Tableaus.Count);
            for (int i = 0; i < Game.Tableaus.Count; i++)
            {
                Debug.WriteLine($"Tableau {i}: {Game.Tableaus[i].cards.Count} cards");
            }

            Debug.WriteLine("Stock: " + Game.Stock.cards.Count);

            // initialize viewmodel state
            Refresh();
        }

        public void Refresh()
        {
            Debug.WriteLine("Page1ViewModel.Refresh called");

            Stock.Refresh();
            Waste.Refresh();

            // populate WasteTop with last up to 3 cards from Waste.Cards (preserve VM instances)
            WasteTop.Clear();
            int take = 3;
            var cards = Waste.Cards;
            for (int i = Math.Max(0, cards.Count - take); i < cards.Count; i++)
            {
                WasteTop.Add(cards[i]);
            }

            // mark only the last (top) waste card as interactable
            for (int i = 0; i < WasteTop.Count; i++)
            {
                WasteTop[i].IsTop = (i == WasteTop.Count - 1);
            }

            foreach (var foundation in Foundations)
            {
                foundation.Refresh();
                Debug.WriteLine($"Foundation count: {foundation.Cards.Count} top: {(foundation.Cards.Count > 0 ? foundation.Cards.Last().Model.TextureName : "<empty>")}");
            }
            foreach (var tableau in Tableau)
            {
                tableau.Refresh();
            }
        }

        public void ResetGame()
        {
            Game.resetGame();
            Refresh();
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
