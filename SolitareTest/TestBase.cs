using System.Reflection;
using SolitaireBack;

public abstract class TestBase
{
    protected TestBase()
    {
        var lm = LoginManager.Instance;

        // Clear internal players list
        var playersField = typeof(LoginManager).GetField("players",
            BindingFlags.NonPublic | BindingFlags.Instance);

        var internalList = (List<Player>)playersField.GetValue(lm);
        internalList.Clear();

        // Set default player
        lm.currentPlayer = new Player("Test", "User", DateOnly.FromDateTime(DateTime.Now.AddYears(-20)), 0);
        lm.setGuestMode(false);
        lm.setDifficulty("easy");
        lm.setVegasMode(false);

        // Reset GameManager
        var gm = GameManager.Instance;
        try { gm.resetGame(); } catch { }

        gm.StartGame();

        /*
         * This setup ensures that each test starts with a clean slate, avoiding any unintended interactions between tests.
         * It also allows us to test specific scenarios by configuring the LoginManager and GameManager as needed.
         */

    }
}
