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
            loginManager = new LoginManager();
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
            cb_VegasMode.IsEnabled = isAdult && !loginManager.isGuest;

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
            cb_VegasMode.IsEnabled = isAdult || !guest;
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

            loginManager.LoadPlayerData(first, last, selectedDate);

            var GameManager = new GameManager();

            GameManager.StartGame();

            NavigationService.Navigate(new Page1(GameManager));
        }
    }
}
