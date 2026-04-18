using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using SolitaireBack;

namespace SolitaireFront
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Ensure the app opens the login page on startup
            MainFrame.Navigate(new Login());

            // Attach closing handler to reliably save on window X
            this.Closing += MainWindow_Closing;

            // The MainWindow class is the main window of the Solitaire application.
        }

        private void MainWindow_Closing(object? sender, CancelEventArgs e)
        {
            try
            {
                var lm = LoginManager.Instance;
                if (lm != null && lm.currentPlayer != null)
                {
                    if (!lm.isGuest)
                    {
                        lm.currentPlayer.gamesP += 1;
                        lm.currentPlayer.gamesL += 1;
                        Debug.WriteLine($"MainWindow_Closing: Marked loss for {lm.currentPlayer.fName} {lm.currentPlayer.lName}");
                    }
                    else
                    {
                        Debug.WriteLine("MainWindow_Closing: Guest mode - not saving persistent stats.");
                    }

                    // Persist: updates in-memory list and writes JSON
                    bool saved = lm.savePlayerData(lm.currentPlayer);
                    Debug.WriteLine("MainWindow_Closing: savePlayerData returned: " + saved);
                }
                else
                {
                    Debug.WriteLine("MainWindow_Closing: LoginManager or currentPlayer is null; nothing saved.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("MainWindow_Closing: Exception while saving player data: " + ex);
            }

            /*
             * The MainWindow_Closing method is an event handler that is called when the main window is closing. 
             * It attempts to save the current player's data using the LoginManager. 
             * If the player is not a guest, it increments their games played and games lost statistics before saving. 
             * It also includes error handling to catch and log any exceptions that occur during the saving process.
             */
        }
    }
}