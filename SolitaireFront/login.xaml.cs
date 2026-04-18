using System;
using System.Collections.Generic;
using System.Diagnostics;
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

            /*
             * The Login class is a WPF Page that serves as the login screen for the Solitaire game. 
             * It interacts with the LoginManager to handle user authentication, age verification, and game settings such as difficulty and Vegas mode. 
             * The class contains event handlers for various UI elements, such as buttons and checkboxes, to manage user input and update the UI accordingly.
             */
        }

        private void ageValidate(object sender, RoutedEventArgs e)
        {
            if (dp_DateOfBirth.SelectedDate is not DateTime dob) return;

            selectedDate = DateOnly.FromDateTime(dob);

            isAdult = loginManager.AgeVertification(selectedDate);

            UpdateVegasUI();

            /* 
             * The ageValidate method is an event handler that is triggered when the user selects a date of birth. 
             * It checks if the selected date is valid and then uses the LoginManager's AgeVertification method to determine if the user is an adult (18 years or older). 
             * Based on the result, it updates the Vegas mode UI to enable or disable it accordingly.
             */
        }

        private void isaGuest(object sender, RoutedEventArgs e)
        {
            guest = true;
            loginManager.setGuestMode(true);
            UpdateVegasUI();

            /* 
            * The isaGuest method is an event handler that is triggered when the user indicates that they want to play as a guest. 
            * It sets the guest variable to true, updates the LoginManager to reflect that the user is in guest mode, 
            * and then updates the Vegas mode UI to disable it since guests cannot access Vegas mode.
            */
        }

        private void isNotaGuest(object sender, RoutedEventArgs e)
        {
            guest = false;
            loginManager.setGuestMode(false);
            UpdateVegasUI();

            /* 
             * The isNotaGuest method is an event handler that is triggered when the user indicates that they do not want to play as a guest. 
             * It sets the guest variable to false, updates the LoginManager to reflect that the user is not in guest mode, 
             * and then updates the Vegas mode UI to enable it if the user is an adult.
             */
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

            /* 
            * The VegasTXT method returns a string that is used as the content for the Vegas mode checkbox. 
            * It checks if the user is a guest or not an adult and returns a corresponding message to indicate why Vegas mode is disabled, 
            * or simply returns "VEGAS MODE" if it is enabled.
            */
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

            /* 
            * The UpdateVegasUI method updates the Vegas mode checkbox's content and enabled state based on the user's age and guest status. 
            * If Vegas mode is currently checked but becomes disabled due to a change in user status, 
            * it unchecks the box and updates the LoginManager to reflect that Vegas mode is no longer active.
            */
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
            "• Your balance carries over between games.\n\n" +

            "WINNING\n" +
            "You win when all cards are moved to the Foundations in order from Ace to King.",
            "How to play Solitaire",
            MessageBoxButton.OK,
            MessageBoxImage.Information);

            /* 
             * The howTo method is an event handler that displays a message box with instructions on how to play Solitaire,
             * including the goal of the game, rules for moving cards, difficulty settings, and information about Vegas mode. 
             * This provides players with the necessary information to understand how to play the game and what to expect from different settings.
             */
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

            GameManager.Instance.StartGame();

            NavigationService.Navigate(new Page1(GameManager.Instance));

            /* 
            * The btn_Start_Click method is an event handler that is triggered when the user clicks the "Start" button. 
            * It validates the user's input for first name, last name, date of birth, and difficulty selection. 
            * If any required fields are missing or invalid, it shows a warning message and returns early. 
            * If Vegas mode is selected, it displays a warning about gambling and provides a link to support resources. 
            * If the user confirms they want to continue with Vegas mode, it proceeds to load the player's data, start the game, 
            * and navigate to the main game page.
            */
        }

        private void combo_Difficulty_DropDownClosed(object sender, EventArgs e)
        {
            // determine difficulty robustly (ComboBox items are TextBlocks)
            int idx = combo_Difficulty.SelectedIndex;
            string diffText;
            if (idx >= 0)
            {
                // map index 0 -> easy, index 1 -> medium
                diffText = idx == 0 ? "easy" : "medium";
            }
            else
            {
                diffText = (combo_Difficulty.SelectedItem as TextBlock)?.Text ?? combo_Difficulty.Text;
            }

            if (!loginManager.setDifficulty(diffText))
            {
                MessageBox.Show("Invalid difficulty selection.", "Invalid", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            /* 
            * The combo_Difficulty_DropDownClosed method is an event handler that is triggered when the user closes the difficulty selection dropdown. 
            * It determines the selected difficulty level based on the selected index or text of the ComboBox, 
            * and then updates the LoginManager with the selected difficulty. 
            * If the difficulty selection is invalid, it shows a warning message to the user.
            */
        }
    }
}
