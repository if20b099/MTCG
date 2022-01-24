using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCGClassLib
{
    /*
     * DB structure
     * MemberGUID (uniqueidentifier)
     * Wins (int)
     * Losses (int)
     * Elo (int)
     */
    public class Stats
    {
        public int Wins { get; private set; }
        public int Losses { get; private set; }
        public int Elo { get; private set; }

        public Stats(int win, int loss, int rank)
        {
            Wins = win;
            Losses = loss;
            Elo = rank;
        }
        public List<string> PrintStatsRespond()
        {
            List<string> r = new List<string>();
            r.Add("Wins: " + Wins + " Losses: " + Losses + " Elo: " + Elo);
            return r;
        }
        public void PrintStats()
        {
            Console.WriteLine("Wins: " + Wins + " Losses: " + Losses + " Elo: " + Elo);
        }
        public int CalcElo(Stats loser)
        {
            /*
             * https://www.omnicalculator.com/sports/elo#how-to-find-elo-rating-change
             * 
            Your initial Elo rating is 1750, while your opponent's is 1810.
            After an exciting battle, you won. What is the change in your Elo rating?
            At this level of play, we assume that the k-factor is equal to 20. 
            If you want to check or change the development coefficient, 
            go to the Advanced mode of this calculator. Let's compute it step by step:
            Estimate the rating difference, 1810 - 1750 = 60.
            Divide it by 400, 60/400. We can simplify this fraction to 3/20. 
            We can leave it in this form or convert it to a decimal, which is 0.15.
            Raise ten to the power 0.15 and add one, 1 + 100.15 = 2.413.
            Find the expected score by working out the latter's multiplicative inverse, 
            expected_score = 1 / 2.413 = 0.414.
            Evaluate the rating change according to the Elo rating formula, 20 * (1 - 0.414) = 11.7.
            Finally, add this number to your current rating, 1750 + 11.7 = 1761.7. Typically,
            we round Elo ratings to one decimal digit.
            */
            double Diff = loser.Elo - Elo;
            double Frac = Diff / 400.0;
            double Tenth = 1 + Math.Pow(10.0, Frac);


            double Inverse = 1 / Tenth;
            int EloChange = (int)(20 * (1 - Inverse));
            Elo = Elo + EloChange;
            Console.WriteLine("Add elo: " + EloChange);
            Console.WriteLine("New Elo  " + Elo);

            return EloChange;
        }
    }
    public class HighScores
    {
        public string Username { get; set; }
        public Stats stats { get; set; }
        public static int rank { get; set; }
        public HighScores(string Username, Stats stats)
        {
            this.Username = Username;
            this.stats = stats;
        }

        public void PrintBoardLine()
        {
            rank++;
            Console.WriteLine("Rank " + rank + ": " + Username + ", Elo: " + stats.Elo + ", Wins: " + stats.Wins + ", Losses: " + stats.Losses);
        }

        public List<string> PrintBoardLineRespond()
        {
            List<string> r = new List<string>();
            rank++;
            r.Add("Rank " + rank + ": " + Username + ", Elo: " + stats.Elo + ", Wins: " + stats.Wins + ", Losses: " + stats.Losses);
            return r;
        }
    }
}
