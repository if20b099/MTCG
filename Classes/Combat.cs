using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCGClassLib
{
    public class Combat
    {
        // Functions choose cards, User decks
        // -> higher calc damage wins
        // Card gets moved into winners deck
        // Highly effective 200%
        // Not effective 50%
        // Comparison otherwise

        // Methods that override damage to each other
        // No attack (0 dmg)
        // Instead kill (bool)
        // Immunity (exception - obviously not as throw lmao)
        // Evasion (aka immunity but renamed)
        //
        // Enum result -> Switch statement -> return early if necessary
        //                                 
        //
        //
        //
        //
        public enum Result
        {
            Victory,
            Defeat,
            Draw,
            Error
        }
        public enum Immunity
        {
            // Need to mention two for each case since perspectives change
            None,
            Scared,
            Terrifying,
            Controlled,
            Controlling,
            Drowning,
            TooWetForHim,
            Evasion,
            Miss,
            SpellImmune,
            OtherImmuneLol

        }
        public enum Advantage
        {
            None,
            Adv,
            DisAdv
        }
        public Deck player1 { get; set; } = new Deck();
        public Deck player2 { get; set; } = new Deck();
        public Card p1Card { get; set; } = new Card();
        public Card p2Card { get; set; } = new Card();
        public Guid guid { get; set; } = Guid.NewGuid(); // For DB entry
        public double Player1_HP { get; set; } = 100.00;
        public double Player2_HP { get; set; } = 100.00;
        public int CalculateDamage()
        {
            return 0;
        }
        public Immunity GetImmune()
        {
            MonsterRace p2Race = p2Card.Race;
            Element p2Ele = p2Card.CardElement;
            CardType p2CT = p2Card.CardType;
            switch(p1Card.Race)
            {
                case MonsterRace.Goblin:
                    if (p2Race == MonsterRace.Dragon)
                        return Immunity.Scared;
                    break;
                case MonsterRace.Dragon:
                    switch(p2Race)
                    {
                        case MonsterRace.Goblin:
                            return Immunity.Terrifying;
                        case MonsterRace.FireElf:
                            return Immunity.Miss;
                    }
                    break;
                case MonsterRace.Wizard:
                    if (p2Race == MonsterRace.Ork)
                        return Immunity.Controlling;
                    break;
                case MonsterRace.Ork:
                    if (p2Race == MonsterRace.Wizard)
                        return Immunity.Controlled;
                    break;
                case MonsterRace.Knight:
                    if (p2Ele == Element.Water && p2CT == CardType.Spell)
                        return Immunity.Drowning;
                    break;
                case MonsterRace.Kraken:
                    if (p2CT == CardType.Spell)
                        return Immunity.SpellImmune;
                    break;
                case MonsterRace.FireElf:
                    if (p2Race == MonsterRace.Dragon)
                        return Immunity.Evasion;
                    break;
                default:
                    break;
            }

            switch(p1Card.CardType)
            {
                case CardType.Spell:
                    if (p1Card.CardElement == Element.Water && p2Race == MonsterRace.Knight)
                        return Immunity.TooWetForHim;
                    if (p2Race == MonsterRace.Kraken)
                        return Immunity.OtherImmuneLol;
                    break;
            }

           return Immunity.None;
        }
        public Advantage GetEffectivity()
        {
            Element p2Ele = p2Card.CardElement;
            switch(p1Card.CardElement)
            { 
                case Element.Fire:
                    switch(p2Ele)
                    {
                        case Element.Fire:
                            return Advantage.None;
                        case Element.Normal:
                            return Advantage.Adv;
                        case Element.Water:
                            return Advantage.DisAdv;
                    }
                    break;
                case Element.Normal:
                    switch(p2Ele)
                    {
                        case Element.Fire:
                            return Advantage.DisAdv;
                        case Element.Normal:
                            return Advantage.None;
                        case Element.Water:
                            return Advantage.Adv;
                    }
                    break;
                case Element.Water:
                    switch (p2Ele)
                    {
                        case Element.Fire:
                            return Advantage.Adv;
                        case Element.Normal:
                            return Advantage.DisAdv;
                        case Element.Water:
                            return Advantage.None;
                    }
                    break;
            }
            return Advantage.None;
            // Actually exception it lol
        }
        public Combat()
        {

        }
        public Combat(User Challenger, User Protector)
        {
            // Copy the cards from the deck only where the card is set to be in the deck
            try
            {
                foreach (Card card in Challenger.PlayerDeck.UserDeck) // bruh
                {
                    if (card.IsDeck == true)
                    {
                        //Console.WriteLine("Added player 1 card");
                        player1.AddCardRaw(card);
                    }
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
           
            foreach (Card card in Protector.PlayerDeck.UserDeck)
            {
                if (card.IsDeck == true)
                {
                    //Console.WriteLine("Added player 2 card");
                    player2.AddCardRaw(card);
                }
            }

        }

        public Result fight(List<string> loglist)
        {
            if(player1.UserDeck.Count < 4 || player2.UserDeck.Count < 4)
            {
                return Result.Error;
            }
            Console.WriteLine("Player 1 HP: " + Player1_HP);
            Console.WriteLine("Player 2 HP: " + Player2_HP);

            for (int round = 0; round<=100; round++)
            {
                // Draw cards
                // Get Immunity
                // If Immunity, calculate damage
                // Calculate with advantage
          
                
                p1Card = player1.ChooseRandomCard();
                p2Card = player2.ChooseRandomCard();
                double p1dmg = p1Card.Atk;
                double p2dmg = p2Card.Atk;
                bool p1crit = false;
                bool p2crit = false;

                Immunity imm = GetImmune();
                if(imm != Immunity.None)
                {
                    switch(imm)
                    {
                        case Immunity.Scared:
                        case Immunity.Controlled:
                        case Immunity.Drowning:
                        case Immunity.Evasion:
                        case Immunity.SpellImmune:
                            p1dmg = 0;
                            break;
                        case Immunity.Terrifying:
                        case Immunity.Controlling:
                        case Immunity.TooWetForHim:
                        case Immunity.Miss:
                        case Immunity.OtherImmuneLol:
                            p2dmg = 0;
                            break;
                    }
                }

                Advantage adv = GetEffectivity();
                if(adv != Advantage.None)
                {
                    switch(adv)
                    {
                        case Advantage.Adv:
                            p1dmg *= 2.0;
                            p2dmg *= 0.5;
                            break;
                        case Advantage.DisAdv:
                            p1dmg *= 0.5;
                            p2dmg *= 2.0;
                            break;
                    }
                }

                // Critical Strike
                Random rnd = new Random();
                int p1r = rnd.Next(10000) + 1;
                int p2r = rnd.Next(10000) + 1;
                if (p1r <= 2000) // 20%
                {
                    if (p1dmg > 0)
                    {
                        p1crit = true;
                        p1dmg *= 2;
                    }
                }
                if (p2r  <= 2000)
                {
                    if(p2dmg > 0)
                    {
                        p2crit = true;
                        p2dmg *= 2;
                    }
                    
                }

                if (p1dmg != p2dmg)
                {
                    Player1_HP -= p2dmg;
                    Player2_HP -= p1dmg;

                    // Swap cards
                    if (p1dmg > p2dmg)
                    {
                        
                        Console.WriteLine(
                            p1Card.CardElement + " " + p1Card.CardName + " " + p1dmg 
                            + " > " +
                            p2dmg + " " + p2Card.CardElement + " " + p2Card.CardName);

                        if(p1crit)
                        {
                            Console.WriteLine("Player 1 Critical Strike!!!");
                            loglist.Add("Player 1 Critical Strike!!!");
                        }
                        if (p2crit)
                        {
                            Console.WriteLine("Player 2 Critical Strike!!!");
                            loglist.Add("Player 2 Critical Strike!!!");
                        }

                        loglist.Add(p1Card.CardElement + " " + p1Card.CardName + " " + p1dmg
                            + " > " +
                            p2dmg + " " + p2Card.CardElement + " " + p2Card.CardName);

                        player1.AddCardNoLimit(p2Card);
                        player2.RemoveCard(p2Card);
                        loglist.Add("Your HP: " + Player1_HP);
                        loglist.Add("Enemy HP: " + Player2_HP);
                    }
                    else
                    {
                        Console.WriteLine(
                            p1Card.CardElement + " " + p1Card.CardName + " " + p1dmg
                            + " < " +
                            p2dmg + " " + p2Card.CardElement + " " + p2Card.CardName);

                        loglist.Add(p1Card.CardElement + " " + p1Card.CardName + " " + p1dmg
                            + " < " +
                            p2dmg + " " + p2Card.CardElement + " " + p2Card.CardName);

                        if (p1crit)
                        {
                            Console.WriteLine("Player 1 Critical Strike!!!");
                            loglist.Add("Player 1 Critical Strike!!!");
                        }
                        if (p2crit)
                        {
                            Console.WriteLine("Player 2 Critical Strike!!!");
                            loglist.Add("Player 2 Critical Strike!!!");
                        }

                        player2.AddCardNoLimit(p1Card);
                        player1.RemoveCard(p1Card);
                        loglist.Add("Your HP: " + Player1_HP);
                        loglist.Add("Enemy HP: " + Player2_HP);
                    }

                    if (Player1_HP <= 0 || Player2_HP <= 0 || player1.UserDeck.Count <= 0 || player2.UserDeck.Count <= 0)
                    {
                        Console.WriteLine("Player 1 HP: " + Player1_HP);
                        Console.WriteLine("Player 2 HP: " + Player2_HP);
                        break;
                    }
                }
                else
                {
                    // Round is a draw
                    Console.WriteLine(
                           p1Card.CardElement + " " + p1Card.CardName + " " + p1dmg
                           + " = " +
                           p2dmg + " " + p2Card.CardElement + " " + p2Card.CardName);

                    loglist.Add(p1Card.CardElement + " " + p1Card.CardName + " " + p1dmg
                           + " = " +
                           p2dmg + " " + p2Card.CardElement + " " + p2Card.CardName + "\nDraw Round");
                    Console.WriteLine("Round draw");
                }
                Console.WriteLine("Player 1 HP: " + Player1_HP);
                Console.WriteLine("Player 2 HP: " + Player2_HP);
            }

           // End game
           // Determine winner
           if(player1.UserDeck.Count <= 0)
           {
                Console.WriteLine("Player 2 wins through deck adv");
                loglist.Add("You lose through deck disadvantage");
                return Result.Defeat;
           }
           else if(player2.UserDeck.Count <= 0)
           {
                Console.WriteLine("Player 1 wins through deck adv");
                loglist.Add("You win through deck advantage");
                return Result.Victory;
           }
           else
            {
                if (Player1_HP == Player2_HP)
                {
                    // Game is Draw
                    Console.WriteLine("Game draw");
                    loglist.Add("The game is a draw, no changes will be made");
                    return Result.Draw;
                }
                else
                {
                    if (Player1_HP > Player2_HP)
                    {
                        // Player 1 wins
                        Console.WriteLine("Player 1 wins HP KO");
                        loglist.Add("You win! You killed your enemy");
                        return Result.Victory;
                    }
                    else
                    {
                        // Player 2 wins;
                        Console.WriteLine("Player 2 wins HP KO");
                        loglist.Add("You died. You lose");
                        return Result.Defeat;
                    }
                }
            }

        }

        

        ~Combat()
        {
            // Do 
        }
    }
}
