using SolitaireBack;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolitaireFront
{
    internal class Page1ViewModel : INotifyPropertyChanged
    {
        public GameManager Game { get; }

        public PileViewModel Stock { get; }
        public PileViewModel Waste { get; }
        public ObservableCollection<PileViewModel> Foundations { get; }
        public ObservableCollection<PileViewModel> Tableau { get; }

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

            System.Diagnostics.Debug.WriteLine("Tableaus: " + Game.Tableaus.Count);
            for (int i = 0; i < Game.Tableaus.Count; i++)
            {
                System.Diagnostics.Debug.WriteLine($"Tableau {i}: {Game.Tableaus[i].cards.Count} cards");
            }

            System.Diagnostics.Debug.WriteLine("Stock: " + Game.Stock.cards.Count);
        }

        public void Refresh()
        {
            Stock.Refresh();
            Waste.Refresh();
            foreach (var foundation in Foundations)
            {
                foundation.Refresh();
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
