using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MTCGClassLib
{
    /*
     * TB_User_Inventory
     * 
     * UID int
     * MemberGUID uniqueidentifier
     * CardGUID uniqueidentifier (pk)
     * IsDeck int (treat as bool)
     */
    public class Deck
    {
        public List<Card> PlayerDeck { get; set; } = new List<Card>(); // Used for fights
        public const int MAX_DECKSIZE = 50;

        public List<Card> UserDeck { get; set; } = new List<Card>(); // obtained from fights

        public Deck()
        {
        }
        public void AddCardRaw(Card Add)
        {
            UserDeck.Add(Add);
        }
        public List<string> PrintCardsRespond()
        {
            List<string> r = new List<string>();
            foreach (Card c in UserDeck)
            {

                r.Add(c.guid + ", " + c.CardName + ", " + c.CardType + ", Atk: " + c.Atk + ", " + c.CardElement + ", " + c.Race + " | In Deck: " + c.IsDeck);
            }

            return r;
        }
        public void PrintCards()
        {
            try
            {
                foreach (Card c in UserDeck)
                {

                    Console.WriteLine(c.guid + ", " + c.CardName + ", " + c.CardType + ", Atk: " + c.Atk + ", " + c.CardElement + ", " + c.Race + " | In Deck: " + c.IsDeck);
                }
            }
            catch(Exception e)
            {
                Console.Write(e);
            }
            
        }
        public List<string> PrintCardsAllInfoRespond()
        {
            List<string> r = new List<string>();
            int i = 0;
            r.Add("Cards in your deck: ");
            foreach (Card c in UserDeck)
            {
                if (c.IsDeck)
                {
                    i++;
                    r.Add(c.guid + ", " + c.CardName + ", " + c.CardType + ", Atk: " + c.Atk + ", " + c.CardElement + ", " + c.Race + " | In Deck: " + c.IsDeck);
                }
            }
            if (i == 0)
            {
                r.Add("No cards in your configured deck");
            }
            return r;
        }
        public void PrintCardsAllInfo()
        {
            try
            {
                int i = 0;
                Console.WriteLine("Cards in your deck: ");
                foreach (Card c in UserDeck)
                {
                    if (c.IsDeck)
                    {
                        i++;
                        Console.WriteLine(c.guid + ", " + c.CardName + ", " + c.CardType + ", Atk: " + c.Atk + ", " + c.CardElement + ", " + c.Race + " | In Deck: " + c.IsDeck);
                    }
                }
                if (i == 0)
                {
                    Console.WriteLine("No cards in your configured deck");
                }
            }
            catch (Exception e)
            {
                Console.Write(e);
            }

        }
        public List<string> PrintCardsSimpleRespond()
        {
            List<string> r = new List<string>();
            int i = 0;
            r.Add("Cards in your deck: ");
            foreach (Card c in UserDeck)
            {
                if (c.IsDeck)
                {
                    i++;
                    r.Add(c.CardName + ", Type: " + c.CardType + ", Atk: " + c.Atk + ", Element: " + c.CardElement + ", Race: " + c.Race);
                }
            }
            if (i == 0)
            {
                r.Add("No cards in your configured deck");
            }
            return r;
        }
        public void PrintCardsSimple()
        {
            try
            {
                int i = 0;
                Console.WriteLine("Cards in your deck: ");
                foreach (Card c in UserDeck)
                {
                    if(c.IsDeck)
                    {
                        i++;
                        Console.WriteLine(c.CardName + ", Type: " + c.CardType + ", Atk: " + c.Atk + ", Element: " + c.CardElement + ", Race: " + c.Race);
                    }
                    
                }
                if(i == 0)
                {
                    Console.WriteLine("No cards in your configured deck");
                }
            }
            catch (Exception e)
            {
                Console.Write(e);
            }

        }
        public void PrintCardsPlayerDeck()
        {
            try
            {
                foreach (Card c in PlayerDeck)
                {
                    Console.WriteLine(c.guid + ", " + c.CardName + ", " + c.CardType + ", Atk: " + c.Atk + ", " + c.CardElement + ", " + c.Race + " | In Deck: " + c.IsDeck);
                }
            }
            catch (Exception e)
            {
                Console.Write(e);
            }

        }
        public Deck(Card[] Cards)
        {
            foreach(Card Add in Cards)
            {
                AddCard(Add);
            }
        }
        public void AddCard(Card Add)
        {
            
            if(UserDeck.Count < 4 )
            {
                UserDeck.Add(Add);
            }
            else
            {
                Console.WriteLine("Deck is already too large card won't be added");
            }
        }
        public void AddCardNoLimit(Card Add)
        {
            UserDeck.Add(Add);
        }
        public void RemoveCard(Card Rm)
        {
            UserDeck.Remove(Rm);
        }

        public Card ChooseRandomCard()
        {
            int size = UserDeck.Count;
            if (size < 1)
            {
                return null;
            }
            Random rand = new Random();
            return UserDeck[rand.Next(0, size)]; // Return a random card

        }

        
    }

    /*
     * TB_Card_Package
     * 
     * GUID PK
     * Cardname nvarchar(50)
     * CardType int
     * Element int
     * MonsterRace int
     * Cost int
     */
    public class Package
    {

        [JsonProperty("pack")]
        public Card[] pack { get; set; }
        public List<Card> packL { get; set; } = new List<Card>();

        [JsonProperty("setdeck")]
        public List<Guid> SetDeck { get; set; } = new List<Guid>();
        public Guid gui { get; set; }
        public Package()
        {

        }
        public Package(Guid gui)
        {
            this.gui = gui;
        }

        public void AddCard(Card Add)
        { 
            if (pack.Length == 0)
            {
                pack[0] = Add;
            }
            else
            {
                pack[pack.Length - 1] = Add;
            }
        }

        public void AddCardRaw(Card Add)
        {
            packL.Add(Add);
        }

        public void PrintCards()
        {
           foreach(Card c in packL)
            {
                Console.WriteLine(c.CardName + " " + c.CardType + c.CardElement + c.Atk);
            }
        }
    }
}
