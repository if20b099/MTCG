using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NUnit.Framework;
using MTCGClassLib;
using DB;

namespace MTCGUnitTest
{
    class DBTest
    {
        [Test]
        public void TestRegisteredQuery()
        {
            AuthUser U = new AuthUser("UnregisteredUser", "testpw");
            AuthUser R = new AuthUser("RegisteredUser", "testpw");
            DBc con = DBc.Instance;
            Assert.IsFalse(con.IsRegisteredUser(U));
            Assert.IsTrue(con.IsRegisteredUser(R));
        }
        [Test]
         public void TestDeleteUser() // By id
         {
             User Del = new User("test2", "testpw");
             DBc con = DBc.Instance;
             Assert.IsTrue(con.DeleteUser(Del));
         }
        [Test]
        public void TestRegister()
        {
            AuthUser Reg = new AuthUser("UnregisteredUser", "testpw");
            DBc con = DBc.Instance;
            Assert.IsTrue(con.RegisterUser(Reg));
            //Assert.IsTrue(con.RegisterStats(Reg));
            Guid mem = con.ObtainGUID("UnregisteredUser");
            User Del = new User(mem, "UnregisteredUser", "testpw");
            Assert.IsTrue(con.DeleteUser(Del));

        }
       
  
        [Test]
        public void TestCreateToken()
        {
            AuthUser R = new AuthUser("RegisteredUser", "testpw");
            DBc con = DBc.Instance;
            Assert.AreNotEqual(con.CreateToken(R), new Guid()); // Token must NOT exist
            AuthUser F = new AuthUser("FalseUser", "AAAAAA");
            Assert.AreEqual(con.CreateToken(R), new Guid());
        }
        [Test]
        public void TestDeleteOldToken()
        {
            AuthUser R = new AuthUser("RegisteredUser", "testpw");
            DBc con = DBc.Instance;
            Guid token = con.CreateToken(R);
            Assert.IsTrue(con.DeleteOldToken(token));
        }
        [Test]
        public void TestLoginAuth()
        {
            AuthUser usr = new AuthUser("RegisteredUser", "testpw");
            DBc con = DBc.Instance;
            Guid token = con.AttemptLogin(usr);
            Assert.Pass();
        }
        [Test]
        public void GetTokenFalseUser()
        {
            AuthUser R = new AuthUser("UnregisteredUser", "testpw");
            DBc con = DBc.Instance;
            Assert.AreEqual(con.GetToken(R), new Guid());
        }

        [Test]
        public void TestAuthWithToken()
        {
            Guid tokenTaken = new Guid("36cedd22-5e9c-4d00-a836-5c0493c0172b");
            DBc con = DBc.Instance;
            Assert.IsTrue(con.AuthByToken(tokenTaken));
        }
        [Test]
        public void TestAuthWithFalseToken()
        {
            Guid tokenTaken = new Guid();
            DBc con = DBc.Instance;
            Assert.IsFalse(con.AuthByToken(tokenTaken));
        }

        [Test]
        public void TestAddPackage()
        {
            string json =
            "{\"pack\":[{\"guid\":\"8d3dae30-df87-4bff-a6f0-c1ce34314be1\", \"CardName\":\"WaterGoblin\", \"Atk\": 50, \"CardType\": 0, \"CardElement\": 3, \"Race\":1 }, {\"guid\":\"f9493b58-43e9-4259-88fa-99b4c51e8d15\", \"CardName\":\"WaterGoblin\", \"Atk\": 50, \"CardType\": 0, \"CardElement\": 3, \"Race\":1 },  {\"guid\":\"8c66106b4-309f-4578-a1c7-0c55fb0a40c\", \"CardName\":\"WaterGoblin\", \"Atk\": 50, \"CardType\": 0, \"CardElement\": 3, \"Race\":1 }]}";

            string json2 = "{\"pack\":[{\"guid\":\"8d3dae30-df87-4bff-a6f0-c1ce34314be1\", \"CardName\":\"WaterGoblin\", \"Atk\": 50, \"CardType\": 0, \"CardElement\": 3, \"Race\":1 }, {\"guid\":\"f9493b58-43e9-4259-88fa-99b4c51e8d15\", \"CardName\":\"WaterGoblin\", \"Atk\": 50, \"CardType\": 0, \"CardElement\": 3, \"Race\":1 },  {\"guid\":\"39cb94ea-ddc8-4e80-b5f6-8f0ac44c50b2\", \"CardName\":\"WaterGoblin\", \"Atk\": 50, \"CardType\": 0, \"CardElement\": 3, \"Race\":1 }]}";
            Package pack = JsonConvert.DeserializeObject<Package>(json2);
            DBc con = DBc.Instance;
            Assert.IsTrue(con.AddPackage(pack));
        }
        [Test]
        public void TestGetUserCoins()
        {
            Guid usrToken = new Guid("80827e1f-b50e-4216-bb51-333702727637");
            DBc con = DBc.Instance;
            Assert.AreNotEqual(con.GetUserCoins(usrToken), 0);
        }

        [Test]
        public void TestGetPackageCost()
        {
            Guid packGuid = new Guid("8d21b213-74da-4177-b83c-6797cf4a2f68");
            DBc con = DBc.Instance;
            Assert.AreNotEqual(con.GetPackageCost(packGuid), 0);
        }

        [Test]
        public void TestGetPackageCards()
        {
            Guid packGuid = new Guid("8d21b213-74da-4177-b83c-6797cf4a2f68");
            DBc con = DBc.Instance;
            Package pack = con.GetPackageCards(packGuid);
            pack.PrintCards();
            Console.WriteLine(pack.packL.Count);
            Assert.AreNotEqual(pack, null);

        }

        [Test]
        public void TestAddPackageToUser()
        {
            Guid packGuid = new Guid("8d21b213-74da-4177-b83c-6797cf4a2f68");
            Guid userToken = new Guid("80827e1f-b50e-4216-bb51-333702727637");
            DBc con = DBc.Instance;
            Assert.IsTrue(con.AddPackageToUser(userToken, packGuid));
        }

        [Test]
        public void TestUpdateUserCoins()
        {
            Guid userToken = new Guid("80827e1f-b50e-4216-bb51-333702727637");
            DBc con = DBc.Instance;
            Assert.IsTrue(con.UpdateUserCoins(userToken, 60));
        }

        [Test]
        public void TestAcquirePackage()
        {
            Guid packGuid = new Guid("8d21b213-74da-4177-b83c-6797cf4a2f68");
            Guid userToken = new Guid("80827e1f-b50e-4216-bb51-333702727637");
            DBc con = DBc.Instance;
            Assert.IsTrue(con.AcquirePackage(userToken, packGuid));
        }
        [Test]
        public void TestGetUserCards()
        {
            Guid userToken = new Guid("80827e1f-b50e-4216-bb51-333702727637");
            DBc con = DBc.Instance;
            Deck UserDeck = con.GetUserCards(userToken);
            //UserDeck.PrintCards();
            Assert.AreNotEqual(UserDeck.UserDeck.Count, 0);
        }

        [Test]
        public void TestSetDeck()
        {
            Guid userToken = new Guid("80827e1f-b50e-4216-bb51-333702727637");
            List<Guid> cards = new List<Guid> { { new Guid("2ecae8d7-5f66-49a9-b23d-e95fddc8d240") } , { new Guid("62eb040b-374a-41f2-b606-02b8fd7bd8a0") }, { new Guid("5baa4df4-2b15-492d-b573-c5fb0c98b85b") }, { new Guid("70e770bd-6b56-4462-a4b6-5248faa1b68d") } };
            DBc con = DBc.Instance;
            Assert.IsTrue(con.SetDeck(cards));
        }

        [Test]
        public void TestUpdateStats()
        {
            Guid userGuid = new Guid("bb1b6719-202e-4946-b5c5-34f81a54406b");
            DBc con = DBc.Instance;
            Assert.IsTrue(con.UpdateStats(userGuid, 1, 0, 1010));
        }

        [Test]
        public void TestGetScoreboard()
        {
            DBc con = DBc.Instance;
            List<HighScores> ScoreBoard = con.GetScoreboard();
            foreach (HighScores h in ScoreBoard)
            {
                h.PrintBoardLine();
            }
            Assert.Pass();
        }

    }
}
