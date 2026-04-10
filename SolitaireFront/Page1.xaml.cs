using SolitaireBack;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace SolitaireFront
{
    public partial class Page1 : Page
    {
        private CardViewModel? _selectedCardVm;
        private PileViewModel? _selectedPileVm;
        private FrameworkElement? _selectedElement;

        public Page1() : this(GameManager.Instance) { }
        public Page1(GameManager gm)
        {
            InitializeComponent();
            DataContext = new Page1ViewModel(gm);
            UpdateTopBar();
        }

        private void StockImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Debug.WriteLine("Stock clicked");
            var gm = GameManager.Instance;
            int stockCount = gm.Stock?.cards.Count ?? -1;
            int wasteCount = gm.Waste?.cards.Count ?? -1;
            Debug.WriteLine($"Stock count = {stockCount}, Waste count = {wasteCount}");

            bool ok = false;
            try
            {
                ok = gm.drawFromStock();
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine("drawFromStock threw: " + ex.Message);
            }
            Debug.WriteLine("drawFromStock returned: " + ok);

            Dispatcher.Invoke(() =>
            {
                if (DataContext is Page1ViewModel vm) vm.Refresh();
                UpdateTopBar();
                ClearSelection();
            });

            e.Handled = true;
        }

        private void CardImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Debug.WriteLine("Card clicked");
            if (sender is not FrameworkElement fe)
            {
                Debug.WriteLine("sender not FrameworkElement");
                return;
            }

            Debug.WriteLine("Element DataContext type: " + (fe.DataContext?.GetType().FullName ?? "<null>"));

            if (fe.DataContext is not CardViewModel clickedCardViewModel)
            {
                Debug.WriteLine("DataContext is not CardViewModel");    
                return;
            }

            // Prevent selecting face-down tableau cards
            if (!clickedCardViewModel.Model.isFaceUp)
            {
                Debug.WriteLine("Clicked card is face-down -> ignored");
                e.Handled = true;
                return;
            }

            // Try find a PileViewModel parent first (tableau / foundation / stock area if applicable)
            var targetPileVm = GetParentPileViewModel(fe);

            // If no parent pile found, this may be a waste-card from WasteTop.
            if (targetPileVm == null && DataContext is Page1ViewModel vm)
            {
                if (vm.WasteTop.Contains(clickedCardViewModel))
                {
                    // treat the source as the Waste pile
                    targetPileVm = vm.Waste;
                    Debug.WriteLine("Resolved clicked card as WasteTop -> using Waste pile as target/source");
                }
            }

            Debug.WriteLine("targetPileVm: " + (targetPileVm == null ? "<null>" : targetPileVm.Model.GetType().Name));

            if (targetPileVm == null)
            {
                e.Handled = true;
                return;
            }

            // If the clicked card lives in the Waste pile, only allow selecting the top waste card
            if (DataContext is Page1ViewModel vm2 && targetPileVm == vm2.Waste && !clickedCardViewModel.IsTop)
            {
                Debug.WriteLine("Clicked waste card is not the top waste card -> ignored");
                e.Handled = true;
                return;
            }

            // Select if nothing is selected
            if (_selectedCardVm == null)
            {
                _selectedCardVm = clickedCardViewModel;
                _selectedPileVm = targetPileVm;
                _selectedElement = fe;
                _selectedElement.Opacity = 0.6; // visual cue
                Debug.WriteLine($"Selected {clickedCardViewModel.Model.TextureName} from {targetPileVm.Model.GetType().Name}");

                // Prevent the click from bubbling to the pile handler (which would immediately attempt a move)
                e.Handled = true;
                return;
            }

            // If same card clicked again, deselect
            if (_selectedCardVm == clickedCardViewModel && _selectedPileVm == targetPileVm)
            {
                Debug.WriteLine("Deselected");
                ClearSelection();
                e.Handled = true;
                return;
            }

            // Try move selected -> clicked card's pile
            TryExecuteMove(_selectedCardVm, targetPileVm);
            e.Handled = true;
        }

        private void PileImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Debug.WriteLine("Pile clicked");
            if (sender is not FrameworkElement fe) return;

            var targetPileVm = fe.DataContext as PileViewModel ?? GetParentPileViewModel(fe);
            Debug.WriteLine("targetPileVm: " + (targetPileVm == null ? "<null>" : targetPileVm.Model.GetType().Name));

            if (targetPileVm == null) return;

            if (_selectedCardVm == null)
            {
                Debug.WriteLine("No card selected to move");
                return;
            }

            TryExecuteMove(_selectedCardVm, targetPileVm);
            e.Handled = true;
        }

        private void TryExecuteMove(CardViewModel sourceCardVm, PileViewModel targetPileVm)
        {
            Debug.WriteLine($"Attempting move: {sourceCardVm.Model.TextureName} -> {targetPileVm.Model.GetType().Name}");
            var gm = GameManager.Instance;

            bool moved = false;
            try
            {
                moved = gm.TryMove(sourceCardVm.Model, targetPileVm.Model);
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine("TryMove threw: " + ex.Message);
            }
            Debug.WriteLine("TryMove returned: " + moved);

            Dispatcher.Invoke(() =>
            {
                if (DataContext is Page1ViewModel vm)
                {
                    vm.Refresh();
                    Debug.WriteLine("Page1ViewModel.Refresh called after move");
                }
                UpdateTopBar();
            });

            if (!moved)
            {
                Debug.WriteLine("Move invalid — check GameManager.canAccept rules and card face-up state.");
            }

            ClearSelection();
        }

        private void ClearSelection()
        {
            if (_selectedElement != null)
            {
                _selectedElement.Opacity = 1.0;
                _selectedElement = null;
            }
            _selectedCardVm = null;
            _selectedPileVm = null;
            Debug.WriteLine("Selection cleared");
        }

        private PileViewModel? GetParentPileViewModel(DependencyObject start)
        {
            DependencyObject? current = start;
            while (current != null)
            {
                if (current is FrameworkElement fe && fe.DataContext is PileViewModel pv) return pv;
                current = VisualTreeHelper.GetParent(current);
            }
            return null;
        }

        private void btn_Reset_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is Page1ViewModel viewModel)
            {
                viewModel.ResetGame();
                NavigationService.Navigate(new Login());
            }
        }

        private void UpdateTopBar()
        {
            var lg = LoginManager.Instance;
            var gm = GameManager.Instance;

            lbl_FuulName.Content = lg.currentPlayer.fName;
            lbl_WinRate.Content = lg.currentPlayer.gamesW + "/" + lg.currentPlayer.gamesP;

            if (lg.gamble)
            {
                lbl_Balance.Opacity = 100;
                lbl_BalanceAmount.Opacity = 100;
                lbl_BalanceAmount.Content = "£" + lg.currentPlayer.balance;
            }
            else
            {
                lbl_Balance.Opacity = 0;
                lbl_BalanceAmount.Opacity = 0;
            }

            lbl_movesTaken.Content = gm.movesTaken;

            Debug.WriteLine($"first name: {lg.currentPlayer.fName}");
            Debug.WriteLine($"last name: {lg.currentPlayer.lName}");
            Debug.WriteLine($"dob: {lg.currentPlayer.dob}");
            Debug.WriteLine($"balance: {lg.currentPlayer.balance}");
            Debug.WriteLine($"games played: {lg.currentPlayer.gamesP}");
            Debug.WriteLine($"games won: {lg.currentPlayer.gamesW}");
            Debug.WriteLine($"games lost: {lg.currentPlayer.gamesL}");
        }
    }
}
