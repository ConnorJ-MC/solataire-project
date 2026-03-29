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
using SolitaireBack;

namespace SolitaireFront
{
    /// <summary>
    /// Interaction logic for login.xaml
    /// </summary>
    public partial class Login : Page
    {

        private SolitaireBack.LoginManager loginManager;
        bool isAdult;
        bool guest;
        DateOnly selectedDate;
        public Login()
        {
            InitializeComponent();
            loginManager = LoginManager.Instance;
            loginManager.loadAllPlayersFromJSON();
        }

        private void ageValidate(object sender, RoutedEventArgs e)
        {
            if (dp_DateOfBirth.SelectedDate is not DateTime dob) return;

            selectedDate = DateOnly.FromDateTime(dob);

            isAdult = loginManager.AgeVertification(selectedDate);

            UpdateVegasUI();
        }

        private void isaGuest(object sender, RoutedEventArgs e)
        {
            guest = true;
            loginManager.setGuestMode(true);
            UpdateVegasUI();
        }

        private void isNotaGuest(object sender, RoutedEventArgs e)
        {
            guest = false;
            loginManager.setGuestMode(false);
            UpdateVegasUI();
        }

        private String VegasTXT()
        {
            cb_VegasMode.IsEnabled = isAdult && !guest;

            if (guest)
            {
                return ": VEGAS MODE (disabled as a guest)";
            }

            if (!isAdult)
            {
                return ": VEGAS MODE (Must be 18+ to enable)";
            }

            return ": VEGAS MODE";
        }

        private void UpdateVegasUI()
        {
            cb_VegasMode.Content = VegasTXT();
            cb_VegasMode.IsEnabled = isAdult && !guest;
            if (cb_VegasMode.IsChecked == true && !cb_VegasMode.IsEnabled)
            {
                cb_VegasMode.IsChecked = false;
                loginManager.setVegasMode(false);
            }
        }

        private void VegasON(object sender, RoutedEventArgs e)
        {
            loginManager.setVegasMode(true);
        }

        private void VegasOFF(object sender, RoutedEventArgs e)
        {
            loginManager.setVegasMode(false);
        }

        private void howTo(object sender, RoutedEventArgs e) // fill in next timre but should containt a simple how to, info on dificulty, how vegas works, and the goal
        {
            MessageBox.Show(
            "HOW TO PLAY SOLITAIRE\n\n" +
            "GOAL\n" +
            "Move all 52 cards into the four Foundation piles (top right), each pile built from Ace up to King in the same suit.\n\n" +

            "THE TABLEAU (the 7 piles on the board)\n" +
            "• You can place a card onto another card if it is ONE rank lower and the OPPOSITE colour.\n" +
            "  Example: You can place a red 6 on a black 7.\n" +
            "• You can move a whole stack if it follows the same descending, alternating‑colour pattern.\n" +
            "• Only KINGS can be moved into an empty space.\n\n" +

            "THE STOCK & WASTE\n" +
            "• The Stock is your draw pile.\n" +
            "• Cards you draw go into the Waste.\n" +
            "• You can play cards from the Waste onto the Tableau or Foundations.\n\n" +

            "DIFFICULTY\n" +
            "• Easy: Draw 1 card at a time.\n" +
            "• Medium: Draw 3 cards at a time.\n\n" +

            "VEGAS MODE\n" +
            "• Costs 52 credits to start a game.\n" +
            "• You earn +5 credits for every card you move to a Foundation.\n" +
            "• You lose -5 credits if you move a card back off a Foundation.\n" +
            "• Your balance carries over between games.\n\n" +

            "WINNING\n" +
            "You win when all cards are moved to the Foundations in order from Ace to King.",
            "How to play Solitaire",
            MessageBoxButton.OK,
            MessageBoxImage.Information);
        }

        private void btn_Start_Click(object sender, RoutedEventArgs e)
        {

            var first = txtbx_firstN.Text?.Trim() ?? string.Empty;
            var last = txtbx_secondN.Text?.Trim() ?? string.Empty;
            var dob = dp_DateOfBirth.SelectedDate;
            if ((string.IsNullOrEmpty(first) || string.IsNullOrEmpty(last) || dp_DateOfBirth.SelectedDate is not DateTime) && !guest)
            {
                MessageBox.Show("Please fill in all fields.", "invalid feilds", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (combo_Difficulty.SelectedItem == null)
            {
                MessageBox.Show("Please select a difficulty", "unselected difficulty", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (cb_VegasMode.IsChecked == true)
            {
                var result = MessageBox.Show(
                    "Vegas Mode simulates gambling by deducting virtual money from a virtual balance, or adding onto it.\n\n" +
                    "If you feel gambling may be harmful or you want support, visit BeGambleAware.\n\n" +
                    "Do you want to continue?",
                    "Vegas Mode Warning",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.No)
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = "https://www.begambleaware.org",
                        UseShellExecute = true
                    });

                    return;
                }
            }
            ;

            loginManager.currentPlayer = loginManager.LoadPlayerData(first, last, selectedDate);

            //#1
            //var GameManager = new GameManager();
            GameManager.Instance.StartGame();
            
            NavigationService.Navigate(new Page1(GameManager.Instance));
        }
    }
}
