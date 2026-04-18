using System;
using System.Diagnostics;
using System.Windows;
using SolitaireBack;

public partial class App : Application
{
    protected override void OnExit(ExitEventArgs e)
    {
        base.OnExit(e);

        try
        {
            var lm = LoginManager.Instance;
            if (lm != null && lm.currentPlayer != null)
            {
                // Treat closing as giving up / loss
                if (!lm.isGuest)
                {
                    lm.currentPlayer.gamesP += 1;
                    lm.currentPlayer.gamesL += 1;
                    Debug.WriteLine($"OnExit: Marked loss for {lm.currentPlayer.fName} {lm.currentPlayer.lName}");
                }
                else
                {
                    Debug.WriteLine("OnExit: Guest mode - not saving persistent stats.");
                }

                // Save the in-memory player back to the players list and flush to disk
                bool updated = lm.savePlayerData(lm.currentPlayer); // updates in-memory list and calls saveAllPlayersToJSON
                Debug.WriteLine("OnExit: savePlayerData returned: " + updated);
            }
            else
            {
                Debug.WriteLine("OnExit: LoginManager or currentPlayer is null; nothing to save.");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine("OnExit: Exception while saving player data: " + ex);
        }
    }
}