using SolitaireBack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SolitaireFront
{
    /// <summary>
    /// Interaction logic for Page1.xaml
    /// </summary>
    public partial class Page1 : Page
    {

        public Page1() : this(GameManager.Instance)
        {
        }
        public Page1(GameManager gm)
        {
            InitializeComponent();
            DataContext = new Page1ViewModel(gm);
            UpdateTopBar();
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
        }
    }
}
