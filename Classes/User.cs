using System;
using Newtonsoft.Json;

namespace MTCGClassLib
{
    public class User : IUser // User for Game activity/interactions/dbwork
    {
        /*
         * DB structure:
         * UID (int) auto increment
         * MemberGUID (uniqueidentifier) | Primary
         * Username nvarchar(50)
         * Pw nvarchar(50)
         * Coins (int)
         */
        public string Username { get; private set; }
        public string Password { get; private set; }
        public Guid guid { get; private set; } = new Guid();
        public string Token { get; set; }
        private int Coins { get; set; }
        public Deck PlayerDeck { get;  set; }
        public Stats PlayerStats { get; set; }
       

        public User(string usr, string pw)
        {
            Username = usr;
            Password = pw;
        }
        public User(string usr, string pw, Deck deck)
        {
            Username = usr;
            Password = pw;
            PlayerDeck = deck;
        }
        public User(Guid mguid, string usr, string pw)
        {
            guid = mguid;
            Username = usr;
            Password = pw;
        }
        public User(Guid mguid, string usr, string pw, Deck deck, Stats stats)
        {
            guid = mguid;
            Username = usr;
            Password = pw;
            PlayerDeck = deck;
            PlayerStats = stats;
        }

    }

    public class AuthUser : IUser // For auth and register in db
    {

        /*
         * TB_User_Token
         * 
         * MemberGUID uniqueidentifier
         * Token varchar(50)
         * CreationDate datetime
         * ExpiryDate datetime
         */
        [JsonProperty("Username")]
        public string Username { get; private set; }
        [JsonProperty("Password")]
        public string Password { get; private set; }
        public Guid guid { get; set; } = new Guid();
        public string Token { get; set; }
        public AuthUser() { }
        public AuthUser(string Username, string Password)
        {
            this.Username = Username;
            this.Password = Password;
        }

        public AuthUser(Guid guid, string Username, string Password)
        {
            this.guid = guid;
            this.Username = Username;
            this.Password = Password;
        }
    }

   
}
