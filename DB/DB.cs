using System;
using System.Data.SqlTypes;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using MTCGClassLib;
using System.Data;

namespace DB
{
    public class DBc
    {
        public static DBc Instance = new DBc();
        /*
         * Functions to make
         * 
         * RegisterStats (for use in RegisterUser function) to add entry in TB_User_Scores
         * 
         * AttemptLogin Check login data in TB_User and generates a token that is then added in TB_User_Token. Token is returned
         * 
         * CheckAuth Checks whether a given Token is in TB_User_token and valid
         */
        public bool RegisterUser(AuthUser toRegist)
        {
            if (IsRegisteredUser(toRegist))
            {
                return false;
            }
            string connectionString = GetConnectionString();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(null, connection);

                // Create and prepare an SQL statement.
                command.CommandText =
                    "INSERT INTO dbo.TB_User (MemberGUID, Username, Password, Coins) " +
                    "VALUES (@guid, @username, @password, @coins)";

                SqlParameter guid = new SqlParameter("@guid", SqlDbType.UniqueIdentifier, 0);
                SqlParameter username = new SqlParameter("@username", SqlDbType.NVarChar, 100);
                SqlParameter password = new SqlParameter("@password", SqlDbType.NVarChar, 100);
                SqlParameter coins = new SqlParameter("@coins", SqlDbType.Int, 0);

                guid.Value = Guid.NewGuid();
                username.Value = toRegist.Username;
                password.Value = toRegist.Password;
                coins.Value = 50;
                command.Parameters.Add(guid); // Assign
                command.Parameters.Add(username);
                command.Parameters.Add(password);
                command.Parameters.Add(coins);

                // Call Prepare after setting the Commandtext and Parameters.
                command.Prepare();
                try
                {
                    command.ExecuteNonQuery(); // For Insert
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    connection.Close();
                    return false;
                }
                connection.Close();
            }
            if(RegisterStats(toRegist))
            {
                return true;
            }
  
            return false;
            
        }
        public bool IsRegisteredUser(AuthUser toCheck)
        {
            string connectionString = GetConnectionString();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(null, connection);
                // Create and prepare an SQL statement.
                command.CommandText =
                    "SELECT Username FROM dbo.TB_User " +
                    "WHERE Username = (@username)";

                SqlParameter nameParam = new SqlParameter("@username", SqlDbType.NVarChar, 100);
                nameParam.Value = toCheck.Username;
                command.Parameters.Add(nameParam); // Assign
                // Call Prepare after setting the Commandtext and Parameters.
                command.Prepare();
                command.ExecuteScalar(); // For select
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    Console.WriteLine("Username : " + toCheck.Username + " is already taken.");
                    connection.Close();
                    return true;
                }
                else
                {
                    Console.WriteLine("Username free.");
                    connection.Close();
                    return false;
                }


            }
        }
        public bool DeleteUser(User toDelete)
        {
            string connectionString = GetConnectionString();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(null, connection);

                // Create and prepare an SQL statement.
                command.CommandText =
                    "DELETE FROM dbo.TB_User WHERE Username = @username";
                SqlParameter nameParam = new SqlParameter("@username", SqlDbType.NVarChar, 100);
                nameParam.Value = toDelete.Username;
                command.Parameters.Add(nameParam); // Assign
                command.Prepare();
                try
                {
                    command.ExecuteNonQuery(); // For Insert
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    connection.Close();
                    return false;
                }
            }

            return DeleteUserStats(toDelete);
        }
        private bool DeleteUserStats(User toDelete)
        {
            string connectionString = GetConnectionString();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(null, connection);

                // Create and prepare an SQL statement.
                command.CommandText =
                    "DELETE FROM dbo.TB_User_Scores WHERE MemberGUID = @guid";
                SqlParameter guid = new SqlParameter("@guid", SqlDbType.UniqueIdentifier, 100);
                guid.Value = toDelete.guid;
                command.Parameters.Add(guid); // Assign
                command.Prepare();
                try
                {
                    command.ExecuteNonQuery(); 
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    connection.Close();
                    return false;
                }
            }
            return true;
        }
        public Guid ObtainGUID(string Username)
        {
            Guid memGuid;
            string connectionString = GetConnectionString();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(null, connection);

                // Create and prepare an SQL statement.
                command.CommandText =
                    "SELECT MemberGUID FROM dbo.TB_User WHERE Username = @username";
                SqlParameter username = new SqlParameter("@username", SqlDbType.NVarChar, 100);
                username.Value = Username;
                command.Parameters.Add(username);
                // Call Prepare after setting the Commandtext and Parameters.
                command.Prepare();
                try
                {
                    command.ExecuteScalar(); // For Select
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if(reader.Read())
                    {
                        memGuid = (Guid)reader[0];
                    }
                    else
                    {
                        memGuid = new Guid();
                    }
                   
                    Console.WriteLine("guid: " + memGuid);
                }
                connection.Close();
            }

            return memGuid;
        }
        public bool RegisterStats(AuthUser toRegist)
        {
            Guid memGuid = ObtainGUID(toRegist.Username);
            string connectionString = GetConnectionString();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand insertCmd = new SqlCommand(null, connection);

                insertCmd.CommandText =
                    "INSERT INTO dbo.TB_User_Scores Values (@guid, 0, 0, 1000)";
                SqlParameter guid = new SqlParameter("@guid", SqlDbType.UniqueIdentifier, 100);
                guid.Value = memGuid;
                insertCmd.Parameters.Add(guid);
                insertCmd.Prepare();
                try
                {
                    insertCmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    connection.Close();
                    return false;
                }
            }
            return true;
        }
        public bool DeleteOldToken(Guid Token)
        {
            string connectionString = GetConnectionString();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(null, connection);

                // Create and prepare an SQL statement.
                command.CommandText =
                    "DELETE FROM dbo.TB_User_Token WHERE Token = @token";
                SqlParameter tokenparam = new SqlParameter("@token", SqlDbType.UniqueIdentifier, 100);
                tokenparam.Value = Token;
                command.Parameters.Add(tokenparam); // Assign
                command.Prepare();
                try
                {
                    command.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    connection.Close();
                    return false;
                }
            }
            return true;
        }
        public Guid CreateToken(AuthUser user)
        {
            DateTime now = DateTime.Now;
            Guid memGuid = ObtainGUID(user.Username);
            if(memGuid == new Guid())
            {
                return new Guid();
            }
            string connectionString = GetConnectionString();
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand ins = new SqlCommand(null, connection);
            // Create and prepare an SQL statement.
            ins.CommandText =
                // Todo
                "INSERT INTO dbo.TB_User_Token " +
                "VALUES (@guid, @token, @start, @expire)";

            SqlParameter gui = new SqlParameter("@guid", SqlDbType.UniqueIdentifier, 100);
            SqlParameter tokenparam = new SqlParameter("@token", SqlDbType.UniqueIdentifier, 100);
            SqlParameter startdate = new SqlParameter("@start", SqlDbType.DateTime, 100);
            SqlParameter enddate = new SqlParameter("@expire", SqlDbType.DateTime, 100);
            gui.Value = memGuid;
            //string token = user.Username + "-mtcgtoken";
            //tokenparam.Value = token;
            Guid Token = Guid.NewGuid();
            tokenparam.Value = Token;
            startdate.Value = now;
            enddate.Value = now.AddMonths(1);

            ins.Parameters.Add(gui);
            ins.Parameters.Add(tokenparam);
            ins.Parameters.Add(startdate);
            ins.Parameters.Add(enddate);

            ins.Prepare();
            try
            {
                ins.ExecuteNonQuery(); // For select
                return Token;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                connection.Close();
                return new Guid(); // Contains zeros
            }
        }
        public Guid GetToken(AuthUser user)
        {
            DateTime now = DateTime.Now;
            Guid memGuid = ObtainGUID(user.Username);
            if (memGuid == new Guid())
            {
                return new Guid();
            }
            string connectionString = GetConnectionString();
            SqlConnection connection = new SqlConnection(connectionString);

            connection.Open();
            SqlCommand command = new SqlCommand(null, connection);
            // Create and prepare an SQL statement.
            command.CommandText =
                // Todo
                "SELECT Token, CreationDate, ExpiryDate FROM dbo.TB_User_Token " +
                //"INNER JOIN dbo.TB_User ON dbo.TB_User.MemberGUID="
                "WHERE MemberGUID = (@guid)";

            SqlParameter guidparam = new SqlParameter("@guid", SqlDbType.UniqueIdentifier, 100);
            guidparam.Value = memGuid;
            command.Parameters.Add(guidparam); // Assign
                                               // Call Prepare after setting the Commandtext and Parameters.
            command.Prepare();
            command.ExecuteScalar(); // For select
            SqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                DateTime expire = (DateTime)reader[2];
                Console.WriteLine("Expire: " + expire + " DB: " + reader[2].ToString());
                Console.WriteLine("Now: " + now);
                
                if (DateTime.Compare(expire, now) < 0) // Expired (expire is earlier than now)
                {
                    DeleteOldToken((Guid)reader[0]); // Ensures there is only one token
                    connection.Close();
                    return CreateToken(user);
                }
                else
                {
                    Guid token = (Guid)reader[0];
                    connection.Close();
                    return token;
                }

            }
            else
            {
                Console.WriteLine("Create new Token");
                return CreateToken(user);
            }
        }
        public Guid AttemptLogin(AuthUser user)
        {
            string connectionString = GetConnectionString();
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = new SqlCommand(null, connection);
            // Create and prepare an SQL statement.
            command.CommandText =
                "SELECT Username, Password FROM dbo.TB_User " +
                "WHERE Username = (@username) AND Password = (@password)";

            SqlParameter nameParam = new SqlParameter("@username", SqlDbType.NVarChar, 100);
            SqlParameter pwParam = new SqlParameter("@password", SqlDbType.NVarChar, 100);
            nameParam.Value = user.Username;
            pwParam.Value = user.Password;
            command.Parameters.Add(nameParam); // Assign
            command.Parameters.Add(pwParam); // Assign
            // Call Prepare after setting the Commandtext and Parameters.
            command.Prepare();
            command.ExecuteScalar(); // For select
            SqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                Console.WriteLine("Login Successful. Token can be accessed.");
                Console.WriteLine("Username: " + user.Username + " DB: " + reader[0].ToString());
                Console.WriteLine("Password: " + user.Password + " DB: " + reader[1].ToString());
                connection.Close();
                return GetToken(user);
            }
            else
            {
                Console.WriteLine("No Username and Password match");
                connection.Close();
                return new Guid();
            }  
        }
        public bool AuthByToken(Guid access)
        {
            DateTime now = DateTime.Now;
            string connectionString = GetConnectionString();
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = new SqlCommand(null, connection);
            // Create and prepare an SQL statement.
            command.CommandText =
              // Todo
              "SELECT Token, CreationDate, ExpiryDate FROM dbo.TB_User_Token " +
              "WHERE Token = (@token)";

            SqlParameter guidparam = new SqlParameter("@token", SqlDbType.UniqueIdentifier, 100);
            guidparam.Value = access;
            command.Parameters.Add(guidparam); // Assign
                                               // Call Prepare after setting the Commandtext and Parameters.
            command.Prepare();
            command.ExecuteScalar(); // For select
            SqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {

                DateTime expire = (DateTime)reader[2];
                Console.WriteLine("Expire: " + expire + " DB: " + reader[2].ToString());
                Console.WriteLine("Now: " + now);
                connection.Close();
                if (DateTime.Compare(expire, now) < 0) // Expired (expire is earlier than now)
                {
                    Console.WriteLine("Token expired");
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                Console.WriteLine("Token not found");
                connection.Close();
                return false;
            }
        }
        public bool AddPackage(Package pack)
        {
            string connectionString = GetConnectionString();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                Guid packid = Guid.NewGuid();
                for (int i = 0; i < pack.pack.Length; i++)
                {
                    SqlCommand insertCmd = new SqlCommand(null, connection);

                    insertCmd.CommandText =
                        "INSERT INTO dbo.TB_Card_Package Values (@packgui, @name, @type, @element, @race, @cost, @atk)";

                    SqlParameter packguid = new SqlParameter("@packgui", SqlDbType.UniqueIdentifier, 100);
                    SqlParameter cardname1 = new SqlParameter("@name", SqlDbType.NVarChar, 100);
                    SqlParameter cardtype1 = new SqlParameter("@type", SqlDbType.Int, 100);
                    SqlParameter element1 = new SqlParameter("@element", SqlDbType.Int, 100);
                    SqlParameter race1 = new SqlParameter("@race", SqlDbType.Int, 100);
                    SqlParameter cost1 = new SqlParameter("@cost", SqlDbType.Int, 100);
                    SqlParameter atk1 = new SqlParameter("@atk", SqlDbType.Int, 100);
                    packguid.Value = packid;
                    cardname1.Value = pack.pack[i].CardName;
                    cardtype1.Value = (int)pack.pack[i].CardType;
                    element1.Value = (int)pack.pack[i].CardElement;
                    race1.Value = (int)pack.pack[i].Race;
                    cost1.Value = 20; // Magic number | Base cost of a package
                    atk1.Value = (int)pack.pack[i].Atk;
                    insertCmd.Parameters.Add(packguid);
                    insertCmd.Parameters.Add(cardname1);
                    insertCmd.Parameters.Add(cardtype1);
                    insertCmd.Parameters.Add(element1);
                    insertCmd.Parameters.Add(race1);
                    insertCmd.Parameters.Add(cost1);
                    insertCmd.Parameters.Add(atk1);
                    insertCmd.Prepare();
                    try
                    {
                        insertCmd.ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        connection.Close();
                        return false;
                    }
                }  
            }

            return true;
        }
        public int GetUserCoins(Guid token)
        {
            if(!AuthByToken(token))
            {
                return 0;
            }
            string connectionString = GetConnectionString();
            SqlConnection connection = new SqlConnection(connectionString);

            connection.Open();
            SqlCommand command = new SqlCommand(null, connection);
            // Create and prepare an SQL statement.
            command.CommandText =
                // Todo
                "SELECT dbo.TB_User.Coins " +
                "FROM dbo.TB_User INNER JOIN dbo.TB_User_Token ON " +
                "dbo.TB_User.MemberGUID = dbo.TB_User_Token.MemberGUID AND dbo.TB_User_Token.Token = (@guid)";
            SqlParameter guidparam = new SqlParameter("@guid", SqlDbType.UniqueIdentifier, 100);
            guidparam.Value = token;
            command.Parameters.Add(guidparam); // Assign
            command.Prepare();
            command.ExecuteScalar();
            SqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                int coin = (int)reader[0];
                connection.Close();
                Console.WriteLine(coin);
                return coin;
            }
            else
            {
                connection.Close();
                return 0;
            }
        }
        public Guid GetMemberGuidByToken(Guid token)
        {
           
            string connectionString = GetConnectionString();
            SqlConnection connection = new SqlConnection(connectionString);

            connection.Open();
            SqlCommand command = new SqlCommand(null, connection);
            // Create and prepare an SQL statement.
            command.CommandText =
                // Todo
                "SELECT dbo.TB_User.MemberGUID " +
                "FROM dbo.TB_User INNER JOIN dbo.TB_User_Token ON " +
                "dbo.TB_User.MemberGUID = dbo.TB_User_Token.MemberGUID AND dbo.TB_User_Token.Token = (@guid)";
            SqlParameter guidparam = new SqlParameter("@guid", SqlDbType.UniqueIdentifier, 100);
            guidparam.Value = token;
            command.Parameters.Add(guidparam); // Assign
            command.Prepare();
            command.ExecuteScalar();
            SqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                Guid gui = (Guid)reader[0];
                Console.WriteLine(gui);
                connection.Close();
                Console.WriteLine(gui);
                return gui;
            }
            else
            {
                connection.Close();
                return new Guid();
            }
        }
        public Guid GetTokenByMemberGUID(Guid user)
        {
            string connectionString = GetConnectionString();
            SqlConnection connection = new SqlConnection(connectionString);

            connection.Open();
            SqlCommand command = new SqlCommand(null, connection);
            // Create and prepare an SQL statement.
            command.CommandText =
                // Todo
                "SELECT dbo.TB_User_Token.Token " +
                "FROM dbo.TB_User_Token INNER JOIN dbo.TB_User ON " +
                "dbo.TB_User.MemberGUID = dbo.TB_User_Token.MemberGUID AND dbo.TB_User.MemberGUID = (@guid)";
            SqlParameter guidparam = new SqlParameter("@guid", SqlDbType.UniqueIdentifier, 100);
            guidparam.Value = user;
            command.Parameters.Add(guidparam); // Assign
            command.Prepare();
            command.ExecuteScalar();
            SqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                Guid gui = (Guid)reader[0];
                Console.WriteLine(gui);
                connection.Close();
                Console.WriteLine(gui);
                return gui;
            }
            else
            {
                connection.Close();
                return new Guid();
            }
        }
        public int GetPackageCost(Guid packGuid)
        {
            string connectionString = GetConnectionString();
            SqlConnection connection = new SqlConnection(connectionString);

            connection.Open();
            SqlCommand command = new SqlCommand(null, connection);
            // Create and prepare an SQL statement.
            command.CommandText =
                // Todo
                "SELECT Cost " +
                "FROM dbo.TB_Card_Package WHERE PackGUID = (@guid)";
            SqlParameter guidparam = new SqlParameter("@guid", SqlDbType.UniqueIdentifier, 100);
            guidparam.Value = packGuid;
            command.Parameters.Add(guidparam); // Assign
            command.Prepare();
            command.ExecuteScalar();
            SqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                int coin = (int)reader[0];
                connection.Close();
                Console.WriteLine(coin);
                return coin;
            }
            else
            {
                connection.Close();
                return 0;
            }
        }
        public Package GetPackageCards(Guid packGuid)
        {
            string connectionString = GetConnectionString();
            SqlConnection connection = new SqlConnection(connectionString);

            connection.Open();
            SqlCommand command = new SqlCommand(null, connection);
            // Create and prepare an SQL statement.
            command.CommandText =
                // Todo
                "SELECT CardName, Atk, CardType, Element, Race " +
                "FROM dbo.TB_Card_Package WHERE PackGUID = (@guid)";
            SqlParameter guidparam = new SqlParameter("@guid", SqlDbType.UniqueIdentifier, 100);
            guidparam.Value = packGuid;
            command.Parameters.Add(guidparam); // Assign
            command.Prepare();
            command.ExecuteScalar();
            SqlDataReader reader = command.ExecuteReader();
            Package pack = new Package();
            while (reader.Read())
            {
                pack.AddCardRaw(new Card((string)reader[0], (int)reader[1], (CardType)reader[2], (Element)reader[3], (MonsterRace)reader[4]));
            }
            connection.Close();
            return pack;
        }
        public bool AddPackageToUser(Guid userToken, Guid packGuid)
        {
            string connectionString = GetConnectionString();
            Package pack = GetPackageCards(packGuid);
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                for (int i = 0; i < pack.packL.Count; i++)
                {
                    SqlCommand insertCmd = new SqlCommand(null, connection);

                    insertCmd.CommandText =
                        "INSERT INTO dbo.TB_User_Inventory Values (@MemberGuid, @CardGuid, @name, @type, @element, @race, @atk, 0)";

                    SqlParameter memGuid = new SqlParameter("@MemberGuid", SqlDbType.UniqueIdentifier, 100);
                    SqlParameter cardGuid = new SqlParameter("@CardGuid", SqlDbType.UniqueIdentifier, 100);
                    SqlParameter cardname1 = new SqlParameter("@name", SqlDbType.NVarChar, 100);
                    SqlParameter cardtype1 = new SqlParameter("@type", SqlDbType.Int, 100);
                    SqlParameter element1 = new SqlParameter("@element", SqlDbType.Int, 100);
                    SqlParameter race1 = new SqlParameter("@race", SqlDbType.Int, 100);
                    SqlParameter atk = new SqlParameter("@atk", SqlDbType.Int, 100);
                    memGuid.Value = userToken;
                    cardGuid.Value = Guid.NewGuid();
                    cardname1.Value = pack.packL[i].CardName;
                    cardtype1.Value = (int)pack.packL[i].CardType;
                    element1.Value = (int)pack.packL[i].CardElement;
                    race1.Value = (int)pack.packL[i].Race;
                    atk.Value = pack.packL[i].Atk;
                    insertCmd.Parameters.Add(memGuid);
                    insertCmd.Parameters.Add(cardGuid);
                    insertCmd.Parameters.Add(cardname1);
                    insertCmd.Parameters.Add(cardtype1);
                    insertCmd.Parameters.Add(element1);
                    insertCmd.Parameters.Add(race1);
                    insertCmd.Parameters.Add(atk);
                    insertCmd.Prepare();
                    try
                    {
                        insertCmd.ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        connection.Close();
                        return false;
                    }
                }
            }
            return true;
        }
        public bool UpdateUserCoins(Guid userToken, int newCoins)
        {
            if (!AuthByToken(userToken))
            {
                return false;
            }
            string connectionString = GetConnectionString();
            SqlConnection connection = new SqlConnection(connectionString);

            connection.Open();
            SqlCommand command = new SqlCommand(null, connection);
            // Create and prepare an SQL statement.
            command.CommandText =
                    // Todo
                    "UPDATE dbo.TB_User " +
                    "SET dbo.TB_User.Coins = @coins " +
                    "FROM dbo.TB_User " + 
                    "INNER JOIN dbo.TB_User_Token ON " +
                    "dbo.TB_User.MemberGUID = dbo.TB_User_Token.MemberGUID WHERE dbo.TB_User_Token.Token = (@guid) ";
                   
            SqlParameter guidparam = new SqlParameter("@guid", SqlDbType.UniqueIdentifier, 100);
            SqlParameter coinparam = new SqlParameter("@coins", SqlDbType.Int, 100);
            guidparam.Value = userToken;
            coinparam.Value = newCoins;
            command.Parameters.Add(guidparam); // Assign
            command.Parameters.Add(coinparam); // Assign
            command.Prepare();
            try
            {
                command.ExecuteNonQuery();
                connection.Close();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                connection.Close();
                return false;
            }

        }
        public bool AcquirePackage(Guid userToken, Guid packGuid)
        {
            Guid memGuid = GetMemberGuidByToken(userToken);
            int coins = GetUserCoins(userToken);
            int cost = GetPackageCost(packGuid);
            if(coins == 0 || cost == 0 || coins < cost || memGuid == new Guid())
            {
                return false;
            }
            if(AddPackageToUser(memGuid, packGuid))
            {
                if (UpdateUserCoins(userToken, coins - cost))
                {
                    return true;
                }
            }

            return false;
        }
        public Deck GetUserCards(Guid token)
        {
            if (!AuthByToken(token))
            {
                return null;
            }
            string connectionString = GetConnectionString();
            SqlConnection connection = new SqlConnection(connectionString);

            connection.Open();
            SqlCommand command = new SqlCommand(null, connection);
            // Create and prepare an SQL statement.
            command.CommandText =
                // Todo
                "SELECT dbo.TB_User_Inventory.CardGUID, " +
                "dbo.TB_User_Inventory.CardName, " +
                "dbo.TB_User_Inventory.Atk, " +
                "dbo.TB_User_Inventory.CardType, " +
                "dbo.TB_User_Inventory.CardElement, " +
                "dbo.TB_User_Inventory.Race, " +
                "dbo.TB_User_Inventory.IsDeck " +
                "FROM dbo.TB_User_Inventory INNER JOIN dbo.TB_User_Token ON " +
                "dbo.TB_User_Inventory.MemberGUID = dbo.TB_User_Token.MemberGUID AND dbo.TB_User_Token.Token = (@guid)";
            SqlParameter guidparam = new SqlParameter("@guid", SqlDbType.UniqueIdentifier, 100);
            guidparam.Value = token;
            command.Parameters.Add(guidparam); // Assign
            command.Prepare();
            command.ExecuteScalar();
            SqlDataReader reader = command.ExecuteReader();
            Deck UserDeck = new Deck();
            while (reader.Read())
            {
                //Console.WriteLine((Guid)reader[0], (string)reader[1], (int)reader[2], (int)reader[3], (int)reader[4], (int)reader[5]);
                 UserDeck.AddCardRaw(new Card((Guid)reader[0], (string)reader[1], (int)reader[2], (int)reader[3], (int)reader[4], (int)reader[5], (int)reader[6]));
              
            }
            connection.Close();
            return UserDeck;
        }
        public Deck GetUserCardsByGuid(Guid user)
        {
            string connectionString = GetConnectionString();
            SqlConnection connection = new SqlConnection(connectionString);

            connection.Open();
            SqlCommand command = new SqlCommand(null, connection);
            // Create and prepare an SQL statement.
            command.CommandText =
                // Todo
                "SELECT dbo.TB_User_Inventory.CardGUID, " +
                "dbo.TB_User_Inventory.CardName, " +
                 "dbo.TB_User_Inventory.Atk, " +
                "dbo.TB_User_Inventory.CardType, " +
                "dbo.TB_User_Inventory.CardElement, " +
                "dbo.TB_User_Inventory.Race, " +
                "dbo.TB_User_Inventory.IsDeck " +
                "FROM dbo.TB_User_Inventory INNER JOIN dbo.TB_User ON " +
                "dbo.TB_User_Inventory.MemberGUID = dbo.TB_User.MemberGUID AND dbo.TB_User.MemberGUID = (@guid)";
            SqlParameter guidparam = new SqlParameter("@guid", SqlDbType.UniqueIdentifier, 100);
            guidparam.Value = user;
            command.Parameters.Add(guidparam); // Assign
            command.Prepare();
            command.ExecuteScalar();
            SqlDataReader reader = command.ExecuteReader();
            Deck UserDeck = new Deck();
            while (reader.Read())
            {
                //Console.WriteLine((Guid)reader[0], (string)reader[1], (int)reader[2], (int)reader[3], (int)reader[4], (int)reader[5]);

                UserDeck.AddCardRaw(new Card((Guid)reader[0], (string)reader[1], (int)reader[2], (int)reader[3], (int)reader[4], (int)reader[5], (int)reader[6]));

            }
            connection.Close();
            return UserDeck;
        }
        public bool SetDeck(List<Guid> cardGuids)
        {
            string connectionString = GetConnectionString();
            SqlConnection connection = new SqlConnection(connectionString);

            connection.Open();
            SqlCommand command = new SqlCommand(null, connection);
            // Create and prepare an SQL statement.
            command.CommandText =
                // Todo
                "UPDATE dbo.TB_User_Inventory " +
                "SET IsDeck = 1 " +
                "WHERE CardGUID IN (@c1, @c2, @c3, @c4)";
            SqlParameter c1 = new SqlParameter("@c1", SqlDbType.UniqueIdentifier, 100);
            SqlParameter c2 = new SqlParameter("@c2", SqlDbType.UniqueIdentifier, 100);
            SqlParameter c3 = new SqlParameter("@c3", SqlDbType.UniqueIdentifier, 100);
            SqlParameter c4 = new SqlParameter("@c4", SqlDbType.UniqueIdentifier, 100);

            c1.Value = cardGuids[0];
            c2.Value = cardGuids[1];
            c3.Value = cardGuids[2];
            c4.Value = cardGuids[3];
            command.Parameters.Add(c1); 
            command.Parameters.Add(c2);
            command.Parameters.Add(c3); 
            command.Parameters.Add(c4);
            command.Prepare();
            command.ExecuteScalar();

            SqlCommand remove = new SqlCommand(null, connection);
            remove.CommandText =
                // Todo
                "UPDATE dbo.TB_User_Inventory " +
                "SET IsDeck = 0" +
                "WHERE CardGUID NOT IN (@c5, @c6, @c7, @c8)";
            SqlParameter c5 = new SqlParameter("@c5", SqlDbType.UniqueIdentifier, 100);
            SqlParameter c6 = new SqlParameter("@c6", SqlDbType.UniqueIdentifier, 100);
            SqlParameter c7 = new SqlParameter("@c7", SqlDbType.UniqueIdentifier, 100);
            SqlParameter c8 = new SqlParameter("@c8", SqlDbType.UniqueIdentifier, 100);

            c5.Value = cardGuids[0];
            c6.Value = cardGuids[1];
            c7.Value = cardGuids[2];
            c8.Value = cardGuids[3];
            remove.Parameters.Add(c5);
            remove.Parameters.Add(c6);
            remove.Parameters.Add(c7);
            remove.Parameters.Add(c8);
            remove.Prepare();
            remove.ExecuteScalar();

            connection.Close();
            return true;

        }
        public bool UpdateStats(Guid user, int wins, int losses, int elo)
        {
            string connectionString = GetConnectionString();
            SqlConnection connection = new SqlConnection(connectionString);

            connection.Open();
            SqlCommand command = new SqlCommand(null, connection);
            // Create and prepare an SQL statement.
            command.CommandText =
                    // Todo
                    "UPDATE dbo.TB_User_Scores " +
                    "SET Wins = (@wins), Losses = (@losses), Elo = (@elo)" +
                    "WHERE MemberGUID = (@guid) ";
            SqlParameter guidparam = new SqlParameter("@guid", SqlDbType.UniqueIdentifier, 100);
            SqlParameter winparam = new SqlParameter("@wins", SqlDbType.Int, 100);
            SqlParameter lossparam = new SqlParameter("@losses", SqlDbType.Int, 100);
            SqlParameter eloparam = new SqlParameter("@elo", SqlDbType.Int, 100);
            guidparam.Value = user;
            winparam.Value = wins;
            lossparam.Value = losses;
            eloparam.Value = elo;
            command.Parameters.Add(guidparam); 
            command.Parameters.Add(winparam);
            command.Parameters.Add(lossparam); 
            command.Parameters.Add(eloparam);
            command.Prepare();
            try
            {
                command.ExecuteNonQuery();
                connection.Close();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                connection.Close();
                return false;
            }
        }
        public Stats GetUserStats(Guid user)
        {
            string connectionString = GetConnectionString();
            SqlConnection connection = new SqlConnection(connectionString);

            connection.Open();
            SqlCommand command = new SqlCommand(null, connection);
            // Create and prepare an SQL statement.
            command.CommandText =
                // Todo
                "SELECT Wins, Losses, Elo " +
                "FROM dbo.TB_User_Scores " +
                "WHERE MemberGUID = (@guid)";
            SqlParameter guidparam = new SqlParameter("@guid", SqlDbType.UniqueIdentifier, 100);
            guidparam.Value = user;
            command.Parameters.Add(guidparam); // Assign
            command.Prepare();
            command.ExecuteScalar();
            SqlDataReader reader = command.ExecuteReader();

            if (reader.Read())
            {
                Stats stats = new Stats((int)reader[0], (int)reader[1], (int)reader[2]);
                connection.Close();
                return stats;
            }
            connection.Close();
            return null;
        }
        public User GetUserByGuid(Guid user)
        {
            string connectionString = GetConnectionString();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(null, connection);

                // Create and prepare an SQL statement.
                command.CommandText =
                    "SELECT Username, Coins FROM dbo.TB_User WHERE MemberGUID = @guid";
                SqlParameter guidparam = new SqlParameter("@guid", SqlDbType.UniqueIdentifier, 100);
                guidparam.Value = user;
                command.Parameters.Add(guidparam);
                // Call Prepare after setting the Commandtext and Parameters.
                command.Prepare();
                try
                {
                    command.ExecuteScalar(); // For Select
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        string username = (string)reader[0];
                        string pw = "protected";
                        int coins = (int)reader[1];
                        connection.Close();
                        Deck deck = GetUserCardsByGuid(user);
                        Stats stats = GetUserStats(user);
                        return new User(user, username, pw, deck, stats);
                    }
                    else
                    {
                        
                    }
                }
            }
            return null;
        }
        public List<HighScores> GetScoreboard()
        {
            List<HighScores> hl = new List<HighScores>();
            string connectionString = GetConnectionString();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(null, connection);

                // Create and prepare an SQL statement.
                command.CommandText =
                "SELECT dbo.TB_User.Username, dbo.TB_User_Scores.Wins, dbo.TB_User_Scores.Losses, dbo.TB_User_Scores.Elo " +
                "FROM dbo.TB_User INNER JOIN dbo.TB_User_Scores ON dbo.TB_User.MemberGUID = dbo.TB_User_Scores.MemberGUID ORDER BY dbo.TB_User_Scores.Elo DESC";
                // Call Prepare after setting the Commandtext and Parameters.
                command.Prepare();
                try
                {
                    command.ExecuteScalar(); // For Select
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        hl.Add(new HighScores((string)reader[0], new Stats((int)reader[1], (int)reader[2], (int)reader[3])));
                        //Console.WriteLine((string)reader[0] + " " + (int)reader[1] + " " + (int)reader[2] + " " + (int)reader[3]);
                    }
                }
            }
            return hl;
        }
        static private string GetConnectionString()
        {
            // To avoid storing the connection string in your code,
            // you can retrieve it from a configuration file, using the
            // System.Configuration.ConfigurationManager.ConnectionStrings property
            return @"Data Source=WIN-2NM45E9BG9P\MTCG;Initial Catalog=mtcg;User ID=mtcg;Password=mtcg";
        }

        public DBc()
        {

        }
    }
}
