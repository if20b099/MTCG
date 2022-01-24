using NUnit.Framework;
using DB;
using Newtonsoft.Json;
using MTCGClassLib;
using System;
using System.Text;
using System.Collections.Generic;

namespace MTCGUnitTest
{
    public class GeneralTests
    {
        [SetUp]
        public void Setup()
        {
        }

        /*[Test]
        public void Test1()
        {
            Assert.Pass();
        }*/
        [Test]
        public void CreateDecks()
        {
            Deck p1 = new Deck();
            p1.AddCard(new Card("WaterGoblin", 10, CardType.Monster, Element.Water, MonsterRace.Goblin));
            p1.AddCard(new Card("FireGoblin", 10, CardType.Monster, Element.Fire, MonsterRace.Goblin));
            p1.AddCard(new Card("Dragonlord", 20, CardType.Monster, Element.Normal, MonsterRace.Dragon));
            p1.AddCard(new Card("Knight", 15, CardType.Monster, Element.Normal, MonsterRace.Knight));
            p1.AddCard(new Card("BubbleWizard", 10, CardType.Monster, Element.Fire, MonsterRace.Wizard));

            Deck p2 = new Deck();
            p2.AddCard(new Card("WaterGoblin", 10, CardType.Monster, Element.Water, MonsterRace.Goblin));
            p2.AddCard(new Card("FireGoblin", 10, CardType.Monster, Element.Fire, MonsterRace.Goblin));
            p2.AddCard(new Card("Dragonlord", 20, CardType.Monster, Element.Normal, MonsterRace.Dragon));
            p2.AddCard(new Card("Knight", 15, CardType.Monster, Element.Normal, MonsterRace.Knight));
            p2.AddCard(new Card("BubbleWizard", 10, CardType.Monster, Element.Fire, MonsterRace.Wizard));
        }
        [Test]
        public void TryDeserialize()
        {
            string json = "{\"Username\":\"altenhof\", \"Password\":\"markus\"}";
            AuthUser toRegist = JsonConvert.DeserializeObject<AuthUser>(json);
            Assert.AreEqual(toRegist.Username, "altenhof");
            Assert.AreEqual(toRegist.Password, "markus");
        }

        [Test]
        public void TryCreatePackageWithJson()
        {
            string json =
                "{\"pack\":[{\"guid\":\"8d3dae30-df87-4bff-a6f0-c1ce34784be1\", \"CardName\":\"WaterGoblin\", \"Atk\": 10, \"CardType\": 0, \"CardElement\": 3, \"Race\":1 }, {\"guid\":\"f9493b58-43e9-4259-88fa-80b4c51e8d15\", \"CardName\":\"WaterGoblin\", \"Atk\": 10, \"CardType\": 0, \"CardElement\": 3, \"Race\":1 },  {\"guid\":\"8c66896b4-309f-4578-a1c7-0c55fb0a40c\", \"CardName\":\"WaterGoblin\", \"Atk\": 10, \"CardType\": 0, \"CardElement\": 3, \"Race\":1 }]}";
            // string json =
            //    "[{\"guid\":\"8d3dae30-df87-4bff-a6f0-c1ce34784be1\", \"CardName\":\"WaterGoblin\", \"Atk\": 10, \"CardType\": 0, \"CardElement\": 3, \"Race\":1 }, {\"guid\":\"f9493b58-43e9-4259-88fa-80b4c51e8d15\", \"CardName\":\"WaterGoblin\", \"Atk\": 10, \"CardType\": 0, \"CardElement\": 3, \"Race\":1 },  {\"guid\":\"8c66896b4-309f-4578-a1c7-0c55fb0a40c\", \"CardName\":\"WaterGoblin\", \"Atk\": 10, \"CardType\": 0, \"CardElement\": 3, \"Race\":1 }]";
            Package pack = JsonConvert.DeserializeObject<Package>(json);

            Console.Write(pack);
            foreach (Card c in pack.pack)
            {
                //Console.WriteLine(c.Atk);
                Console.WriteLine(c.CardName);

            }
            Assert.Pass();
        }

        [Test]
        public void TryPrintCards()
        {
            Guid userToken = new Guid("80827e1f-b50e-4216-bb51-333702727637");
            DBc con = DBc.Instance;
            Deck UserDeck = con.GetUserCards(userToken);
            UserDeck.PrintCards();
            Assert.Pass();
        }
        [Test]
        public void TestEloCalc()
        {
            Stats winner = new Stats(1, 0, 1500);
            Stats loser = new Stats(0, 1, 1000);
            winner.CalcElo(loser);
            Assert.Pass();
        }

        [Test]
        public void TestBattleFull()
        {
            DBc con = DBc.Instance;
            Guid chall = new Guid("ad2641f4-0965-4fa4-b872-c484d67266bf");
            Guid opponent = con.ObtainGUID("RegisteredUser");
            User Challenger = con.GetUserByGuid(chall);
            User Protector = con.GetUserByGuid(opponent);
            Challenger.PlayerDeck.PrintCards();
            Protector.PlayerDeck.PrintCards();
            Combat fight = new Combat(Challenger, Protector);
            List<string> loglist = new List<string>();
            Combat.Result outcome = fight.fight(loglist);
            foreach(string s in loglist)
            {
                Console.WriteLine(s);
            }
            switch (outcome)
            {
                case Combat.Result.Victory:
                    {
                        int add = Challenger.PlayerStats.CalcElo(Protector.PlayerStats);
                        con.UpdateStats(chall, Challenger.PlayerStats.Wins + 1, Challenger.PlayerStats.Losses, Challenger.PlayerStats.Elo + add);
                        con.UpdateStats(opponent, Protector.PlayerStats.Wins, Protector.PlayerStats.Losses + 1, Protector.PlayerStats.Elo - add);
                        break;
                    }
                case Combat.Result.Defeat:
                    {
                        int add = Protector.PlayerStats.CalcElo(Challenger.PlayerStats);
                        con.UpdateStats(opponent, Protector.PlayerStats.Wins + 1, Protector.PlayerStats.Losses, Protector.PlayerStats.Elo + add);
                        con.UpdateStats(chall, Challenger.PlayerStats.Wins, Challenger.PlayerStats.Losses + 1, Challenger.PlayerStats.Elo - add);
                        break;
                    }
                case Combat.Result.Draw:
                    break;
                case Combat.Result.Error:
                    break;
            }
        }
    }
}