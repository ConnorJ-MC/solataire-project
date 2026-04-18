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

            /* 
             * The constructor initializes the Page1ViewModel by setting up references to the GameManager 
             * and creating PileViewModel instances for the Stock, Waste, Foundations, and Tableau piles. 
             * It also initializes the WasteTop collection to expose only the top N waste cards for the UI. 
             * Finally, it calls the Refresh method to populate the viewmodel state based on the current game state.
             */
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

            /* 
             * The Refresh method updates the state of the viewmodel based on the current game state. 
             * It calls Refresh on the Stock and Waste PileViewModels to update their card collections. 
             * It then populates the WasteTop collection with the last up to 3 cards from the Waste.Cards collection, 
             * preserving the existing CardViewModel instances. 
             * It also marks only the last (top) waste card as interactable by setting the IsTop property. 
             * Finally, it calls Refresh on each Foundation and Tableau PileViewModel to update their card collections.
             */
        }

        public void ResetGame()
        {
            Game.resetGame();
            Refresh();

            /* 
            * The ResetGame method resets the game state by calling the resetGame method on the GameManager instance, 
            * and then calls Refresh to update the viewmodel state to reflect the new game state. 
            * This allows the UI to update and display the initial setup of a new game.
            */
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
