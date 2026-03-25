using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.IO;

namespace SolitaireBack
{

    public enum Difficulty
    {
        easy = 1,
        medium = 3
    }
    public class LoginManager
    {
        private static LoginManager _instance;
        public static LoginManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new LoginManager();
                }
                return _instance;
            }
        }



        public Difficulty difficulty { get; private set; }
        public bool gamble { get; set; }
        private List<Player> players = new List<Player>();
        public IReadOnlyList<Player> Players => players.AsReadOnly();
        public Player currentPlayer;
        //private String tempNewFName;
        //private String tempNewLName;
        //private DateOnly tempNewDOB;
        //private int newBalance;
        //private int newGamesP;
        //private int newGamesW;
        //private int newGamesL;

        public bool isGuest { get; set; }

        private readonly string jsonFilePath;

        public LoginManager()
        {
            string saveFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "NCL",
                "Solitaire"
            );

            Directory.CreateDirectory(saveFolder);

            jsonFilePath = Path.Combine(saveFolder, "players.json");

            loadAllPlayersFromJSON();
        }

        public bool setDifficulty(String diff)
        {
            if (diff == "easy")
            {
                difficulty = Difficulty.easy;
                return true;
            }
            else if (diff == "medium")
            {
                difficulty = Difficulty.medium;
                return true;
            }
            else
            {
                return false;
            }
        }

        public void setVegasMode(bool selected)
        {
            if (!selected || isGuest)
            {
                gamble = false;
            }
            else
            {
                gamble = true;
            }
        }
        public bool AgeVertification(DateOnly dob)
        {
            DateOnly today = DateOnly.FromDateTime(DateTime.Today);
            int age = today.Year - dob.Year;
            if (dob > today.AddYears(-age)) age--;
            return age >= 18;
        }

        public void setGuestMode(bool slected)
        {
            isGuest = slected;

            currentPlayer = new Player("Guest", "Player", DateOnly.MinValue, 0)
            {
                balance = 0,
                gamesP = 0,
                gamesW = 0,
                gamesL = 0
            };
        }

        public Player createPlayer(string fName, string lName, DateOnly dob)
        {
            Player p = new(fName, lName, dob, 0)
            {
                balance = 0,
                gamesP = 0,
                gamesW = 0,
                gamesL = 0
            };

            players.Add(p);

            return p;
        }

        public bool savePlayerData(Player p)
        {
            if (isGuest)
            {
                isGuest = false; // Exiting guest mode, but we won't save guest data to JSON.
                return true;
            }

            for (int i = 0; i < players.Count; i++)
            {
                if (players[i].fName == p.fName && players[i].lName == p.lName && players[i].dob == p.dob)
                {
                    players[i] = p;
                    return saveAllPlayersToJSON();
                }
            }

            return false; // Player not found, cannot save.
        }

        public Player LoadPlayerData(string fName, string lName, DateOnly dob)
        {
            if (isGuest)
            {
                currentPlayer = new Player("Guest", "Player", DateOnly.MinValue, 0)
                {
                    balance = 0,
                    gamesP = 0,
                    gamesW = 0,
                    gamesL = 0
                };

                return currentPlayer; // Guest mode doesn't load from JSON.
            }

            currentPlayer = players.
                FirstOrDefault(p => p.fName == fName && p.lName == lName && p.dob == dob)
                ?? createPlayer(fName, lName, dob);

            return currentPlayer;
        }

        private bool jsonLoaded = false;
        public bool jsonSaved = true; //will start true opon launch, but will be switch to false when a game is won/losed/reset to ensure player data is saved

        public bool loadAllPlayersFromJSON()
        {
            if (jsonLoaded)
            {
                return true; // Already loaded, no need to load again.
            }

            try
            {
                if (!File.Exists(jsonFilePath))
                {
                    // If the file doesn't exist, we can consider it as successfully "loaded" with an empty list.
                    jsonLoaded = true;
                    return true;
                }

                string jsonData = File.ReadAllText(jsonFilePath);
                players = JsonSerializer.Deserialize<List<Player>>(jsonData) ?? new List<Player>();
                jsonLoaded = true;
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool saveAllPlayersToJSON()
        {
            if (isGuest)
            {
                return true; // No need to save guest data.
            }

            if (jsonSaved)
            {
                return true;
            }

            try
            {
                string jsonData = JsonSerializer.Serialize(players, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(jsonFilePath, jsonData);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
