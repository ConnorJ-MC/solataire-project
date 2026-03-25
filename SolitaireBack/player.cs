using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SolitaireBack
{
    public class Player(string fName, string lName, DateOnly dob, int v)
    {
        public string fName { get; set; } = fName;
        public string lName { get; set; } = lName;
        public DateOnly dob { get; set; } = dob;
        public int balance { get; set; } = 0;
        public int gamesP { get; set; } = 0;
        public int gamesW { get; set; } = 0;
        public int gamesL { get; set; } = 0;
    }
}
