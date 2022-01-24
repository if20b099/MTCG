using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using MTCGClassLib;

namespace MTCGUnitTest
{
    public class CombatTest
    {
        [Test]
        public void TestDecks()
        {
            
            Card p11 = new Card("Affe", 10, CardType.Monster, Element.Normal, MonsterRace.Goblin);
            Card p12 = new Card("WasserAffe", 10, CardType.Monster, Element.Water, MonsterRace.Goblin);
            Card p13 = new Card("Drache", 10, CardType.Monster, Element.Normal, MonsterRace.Dragon);
            Card p14 = new Card("Alter FeuerMagier", 10, CardType.Monster, Element.Fire, MonsterRace.Wizard);
            Card[] arr1 = { p11, p12, p13, p14 };
            Deck p1D = new Deck(arr1);

            Card p21 = new Card("Affe", 10, CardType.Monster, Element.Normal, MonsterRace.Goblin);
            Card p22 = new Card("WasserAffe", 10, CardType.Monster, Element.Water, MonsterRace.Goblin);
            Card p23 = new Card("Drache", 10, CardType.Monster, Element.Normal, MonsterRace.Dragon);
            Card p24 = new Card("Alter FeuerMagier", 10, CardType.Monster, Element.Fire, MonsterRace.Wizard);

            Card[] arr2 = { p21, p22, p23, p24 };
            Deck p2D = new Deck(arr1);

            p1D.PrintCardsPlayerDeck();
            Assert.Pass();
        }

        [Test]
        public void TestImmunity()
        {
            Card p11 = new Card("Affe", 10, CardType.Monster, Element.Normal, MonsterRace.Goblin);
            Card p23 = new Card("Drache", 10, CardType.Monster, Element.Normal, MonsterRace.Dragon);
            Combat cmb = new Combat();
            cmb.p1Card = p11;
            cmb.p2Card = p23;
            Assert.AreEqual(Combat.Immunity.Scared, cmb.GetImmune());
        }

        [Test]
        public void TestAdvantage()
        {
            Card p11 = new Card("Affe", 10, CardType.Monster, Element.Normal, MonsterRace.Goblin);
            Card p23 = new Card("Drache", 10, CardType.Monster, Element.Normal, MonsterRace.Dragon);
            Combat cmb = new Combat();
            cmb.p1Card = p11;
            cmb.p2Card = p23;
            Assert.AreEqual(Combat.Advantage.None, cmb.GetEffectivity());
        }

        [Test]
        public void TestFight()
        {
            Card p11 = new Card("Affe", 10, CardType.Monster, Element.Normal, MonsterRace.Goblin, 1);
            Card p12 = new Card("WasserAffe", 10, CardType.Monster, Element.Water, MonsterRace.Goblin, 1);
            Card p13 = new Card("Drache", 10, CardType.Monster, Element.Normal, MonsterRace.Dragon, 1);
            Card p14 = new Card("Alter FeuerMagier", 10, CardType.Monster, Element.Fire, MonsterRace.Wizard, 1);
            Card[] arr1 = { p11, p12, p13, p14 };
            Deck p1D = new Deck(arr1);

            Card p21 = new Card("Affe", 10, CardType.Monster, Element.Normal, MonsterRace.Goblin, 1);
            Card p22 = new Card("WasserAffe", 10, CardType.Monster, Element.Water, MonsterRace.Goblin, 1);
            Card p23 = new Card("Drache", 10, CardType.Monster, Element.Normal, MonsterRace.Dragon, 1);
            Card p24 = new Card("Alter FeuerMagier", 10, CardType.Monster, Element.Fire, MonsterRace.Wizard, 1);

            Card[] arr2 = { p21, p22, p23, p24 };
            Deck p2D = new Deck(arr2);

            Combat cmb = new Combat(new User("Test 1", "Test2", p1D), new User("Test 2", "Test2", p2D));
            List<string> loglist = new List<string>();
            cmb.fight(loglist);
            Assert.Pass();
        }

    }
}
