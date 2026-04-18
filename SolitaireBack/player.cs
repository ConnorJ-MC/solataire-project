using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SolitaireBack
{
    public class Player
    {
        public string fName { get; set; }
        public string lName { get; set; }
        public DateOnly dob { get; set; }
        public int balance { get; set; }
        public int gamesP { get; set; }
        public int gamesW { get; set; }
        public int gamesL { get; set; }

        public Player() { }

        // convenience constructor (parameter names match properties)
        public Player(string fName, string lName, DateOnly dob, int balance)
        {
            this.fName = fName;
            this.lName = lName;
            this.dob = dob;
            this.balance = balance;
            this.gamesP = 0;
            this.gamesW = 0;
            this.gamesL = 0;
        }
    }
}
