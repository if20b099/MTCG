using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Text;
using System.Collections.Generic;
using Newtonsoft.Json;
using MTCGClassLib;
using DB;
using System.Threading;

namespace MTCG
{
    class Program
    {
        private static AuthUser ToJson(string p)
        {
            AuthUser test = JsonConvert.DeserializeObject<AuthUser>(p);
            return test;
        }
        public class ClientThreader
        {
            TcpClient socket { get; set; }
            public void respond(NetworkStream ns, string msg)
            {
                StringBuilder response = new StringBuilder();
                response.AppendLine(msg);
                var rep = Encoding.UTF8.GetBytes(response.ToString());
                ns.Write(rep, 0, rep.Length);
            }
            public void StartClient(TcpClient socketStart)
            {
                this.socket = socketStart;
                Thread cThread = new Thread(Handle);
                cThread.Start();
            }
            public async void Handle()
            {
                DBc con = DBc.Instance;
                using (NetworkStream ns = socket.GetStream())
                {
                    string Method, dir, Host, UserAgent,
                            Accept, ContentType, ContentLength, Content = "";

                    byte[] msg = new byte[2048];
                    ns.Read(msg, 0, msg.Length);
                    string request = Encoding.UTF8.GetString(msg);


                    StringBuilder response = new StringBuilder();
                    response.AppendLine("HTTP/1.1 200 OK");
                    response.AppendLine("Content-Type: text/plain");
                    response.AppendLine();
                    response.AppendLine("Connected, working\n");
                    var rep = Encoding.UTF8.GetBytes(response.ToString());
                    ns.Write(rep, 0, rep.Length);

                    using (StringReader reader = new StringReader(request))
                    {
                        //  POST /users HTTP/1.1
                        //  Host: localhost:10001
                        //  User-Agent: curl/7.55.1
                        //  Accept: */*
                        //  Content-Type: application/json
                        //  Content-Length: 36


                        // Examine request Method
                        string reading = await reader.ReadLineAsync(); // POST /users HTTP/1.1
                                                                       //Console.WriteLine("Received reading: " + reading);

                        Method = reading.Split(' ')[0]; // POST
                        dir = reading.Split(' ')[1]; // /users
                        switch (Method)
                        {
                            case "POST":
                                { 
                                    switch (dir)
                                    {
                                        case "/users": // Register
                                            {
                                                Host = await reader.ReadLineAsync();
                                                UserAgent = await reader.ReadLineAsync();
                                                Accept = await reader.ReadLineAsync();
                                                ContentType = await reader.ReadLineAsync();
                                                ContentLength = await reader.ReadLineAsync();
                                                Content = await reader.ReadLineAsync();
                                                string json = await reader.ReadLineAsync();

                                                Console.WriteLine(json);
                                                try
                                                {
                                                    AuthUser toRegist = ToJson(json);
                                                    if (con.RegisterUser(toRegist))
                                                    {
                                                        //Write success
                                                        Console.WriteLine("Successfully registered");
                                                        respond(ns, "Sucessfully registered");
                                                    }
                                                    else
                                                    {
                                                        // Write fail
                                                        Console.WriteLine("Failed registration. Maybe name is taken?");
                                                        respond(ns, "Failed registration. Maybe name is taken?");
                                                    }

                                                }
                                                catch (Exception exc)
                                                {
                                                    Console.WriteLine("error occurred: " + exc.Message);
                                                    respond(ns, "There has been an error, please contact system admin");
                                                }

                                                break;
                                            }
                                        case "/sessions": // Login
                                            {
                                                try
                                                {
                                                    Host = await reader.ReadLineAsync();
                                                    UserAgent = await reader.ReadLineAsync();
                                                    Accept = await reader.ReadLineAsync();
                                                    ContentType = await reader.ReadLineAsync();
                                                    ContentLength = await reader.ReadLineAsync();
                                                    Content = await reader.ReadLineAsync();
                                                    string jsonLogin = await reader.ReadLineAsync();
                                                    AuthUser login = ToJson(jsonLogin);
                                                    Guid token = con.AttemptLogin(login);
                                                    if (token != new Guid())
                                                    {
                                                        //Write Token
                                                        Console.WriteLine("Your token (KEEP PRIVATE AT ALL COST): " + token);
                                                        respond(ns, "Sucessfully logged in.\nYour token (KEEP PRIVATE AT ALL COST): " + token);
                                                    }
                                                    else
                                                    {
                                                        // Write fail
                                                        Console.WriteLine("Invalid login");
                                                        respond(ns, "Invalid login");
                                                    }

                                                }
                                                catch (Exception exc)
                                                {
                                                    Console.WriteLine("error occurred: " + exc.Message);
                                                    respond(ns, "There has been an error, please contact system admin");
                                                }
                                                break;
                                            }                                         
                                        case "/packages": // Create packages
                                            {
                                                Host = await reader.ReadLineAsync();
                                                UserAgent = await reader.ReadLineAsync();
                                                Accept = await reader.ReadLineAsync();
                                                ContentType = await reader.ReadLineAsync();
                                                ContentLength = await reader.ReadLineAsync();
                                                Content = await reader.ReadLineAsync();
                                                string Content2 = await reader.ReadLineAsync();
                                                string jsonPackage = await reader.ReadLineAsync();

                                                Console.WriteLine(jsonPackage);
                                                ContentLength = ContentLength.Split(' ')[1];
                                                Console.WriteLine(ContentLength);
                                                try
                                                {
                                                    if (ContentLength == "6a6a946e-069b-42c2-be49-b5f6cdd58ea1") // Token Admin
                                                    {
                                                        Package pack = JsonConvert.DeserializeObject<Package>(jsonPackage);
                                                        if (con.AddPackage(pack))
                                                        {
                                                            // Write Success
                                                            Console.WriteLine("Successfully added package by admin");
                                                            respond(ns, "Sucessfully added package by admin");
                                                        }
                                                    }
                                                    else
                                                    {
                                                        // Write fail
                                                        Console.WriteLine("You are unauthorized to perform this action");
                                                        respond(ns, "You are unauthorized to perform this action");
                                                    }

                                                }
                                                catch (Exception exc)
                                                {
                                                    Console.WriteLine("error occurred: " + exc.Message);
                                                    respond(ns, "There has been an error, please contact system admin");
                                                }
                                                break;
                                            }
                                        case "/transactions/packages": // Create packages
                                            {
                                                Host = await reader.ReadLineAsync();
                                                UserAgent = await reader.ReadLineAsync();
                                                Accept = await reader.ReadLineAsync();
                                                ContentType = await reader.ReadLineAsync();
                                                string Auth = await reader.ReadLineAsync();
                                                Content = await reader.ReadLineAsync();
                                                string Content3 = await reader.ReadLineAsync();
                                                string Package = await reader.ReadLineAsync();
                                                string AuthToken = Auth.Split(' ')[1];
                                                Console.WriteLine(AuthToken);
                                                Console.WriteLine(Package);
                                                try
                                                {
                                                    Package gui = JsonConvert.DeserializeObject<Package>(Package);
                                                    Guid userToken = new Guid(AuthToken);
                                                    //Guid PackageGuid = new Guid(Package.ToString());
                                                    if (con.AuthByToken(userToken)) // Token Admin
                                                    {
                                                        if (con.AcquirePackage(userToken, gui.gui))
                                                        {
                                                            // Write Success

                                                            Console.WriteLine("Successfully bought package");
                                                            respond(ns, "Successfully purchased package");


                                                        }
                                                        else
                                                        {
                                                            // Write fail pack or coin
                                                            Console.WriteLine("You either do not have enough coins, or the pack is invalid");
                                                            respond(ns, "You either do not have enough coins, or the pack is invalid");
                                                        }
                                                    }
                                                    else
                                                    {
                                                        // Write fail
                                                        Console.WriteLine("An error has occurred. Please make sure your command is correct");
                                                        respond(ns, "There has been an error. Please make sure your token is valid");
                                                    }

                                                }
                                                catch (Exception exc)
                                                {
                                                    Console.WriteLine("error occurred: " + exc.Message);
                                                    respond(ns, "There has been an error, please contact system admin");
                                                }
                                                    break;
                                            }
                                        case "/battles": // Attempt battle
                                            {
                                                Host = await reader.ReadLineAsync();
                                                UserAgent = await reader.ReadLineAsync();
                                                Accept = await reader.ReadLineAsync();
                                                string Auth = await reader.ReadLineAsync();
                                                Content = await reader.ReadLineAsync();
                                                string Content2 = await reader.ReadLineAsync();
                                                string Content3 = await reader.ReadLineAsync();
                                                string UserToBattle = await reader.ReadLineAsync();
                                                string AuthToken = Content.Split(' ')[1];
                                                Console.WriteLine(Content);
                                                Console.WriteLine(UserToBattle + " to be battled");
                                                Console.WriteLine(AuthToken);
                                                Guid userToken = new Guid(AuthToken);

                                                if (con.AuthByToken(userToken)) // Token Admin
                                                {
                                                    Console.WriteLine("Success");
                                                    // Get users with stats and guids
                                                    try
                                                    {
                                                        Guid chall = con.GetMemberGuidByToken(userToken);
                                                        Guid opponent = con.ObtainGUID(UserToBattle);
                                                        User Challenger = con.GetUserByGuid(chall);
                                                        User Protector = con.GetUserByGuid(opponent);
                                                        //Challenger.PlayerDeck.PrintCards();

                                                        if (Challenger.Username == Protector.Username)
                                                        {
                                                            Console.WriteLine("Cannot fight yourself.");
                                                            respond(ns, "Cannot fight yourself");
                                                        }
                                                        else
                                                        {
                                                            Combat fight = new Combat(Challenger, Protector);
                                                            List<string> loglist = new List<string>();
                                                            Combat.Result outcome = fight.fight(loglist);
                                                            foreach (string logmsg in loglist)
                                                            {
                                                                Console.WriteLine(logmsg);
                                                                respond(ns, logmsg);
                                                            }
                                                            switch (outcome)
                                                            {
                                                                case Combat.Result.Victory:
                                                                    {
                                                                        int add = Challenger.PlayerStats.CalcElo(Protector.PlayerStats);
                                                                        con.UpdateStats(chall, Challenger.PlayerStats.Wins + 1, Challenger.PlayerStats.Losses, Challenger.PlayerStats.Elo + add);
                                                                        con.UpdateStats(opponent, Protector.PlayerStats.Wins, Protector.PlayerStats.Losses + 1, Protector.PlayerStats.Elo - add);
                                                                        respond(ns, "You won, you gain " + add + " Elo.");
                                                                        break;
                                                                    }
                                                                case Combat.Result.Defeat:
                                                                    {
                                                                        int add = Protector.PlayerStats.CalcElo(Challenger.PlayerStats);
                                                                        con.UpdateStats(opponent, Protector.PlayerStats.Wins + 1, Protector.PlayerStats.Losses, Protector.PlayerStats.Elo + add);
                                                                        con.UpdateStats(chall, Challenger.PlayerStats.Wins, Challenger.PlayerStats.Losses + 1, Challenger.PlayerStats.Elo - add);
                                                                        respond(ns, "You lost, you lose " + add + " Elo.");
                                                                        break;
                                                                    }
                                                                case Combat.Result.Draw:
                                                                    respond(ns, "Draw, no change in elo");
                                                                    break;
                                                                case Combat.Result.Error:
                                                                    {
                                                                        Console.WriteLine("An error has occured, no points have been taken or given");
                                                                        respond(ns, "An error has occured, no points have been taken or given");
                                                                        break;
                                                                    }

                                                            }
                                                        }

                                                    }
                                                    catch (Exception e)
                                                    {
                                                        //An error has occurred.
                                                        Console.WriteLine("An error has occurred: " + e);
                                                        respond(ns, "An error has occured, please make sure your command is correct");
                                                    }
                                                }
                                                else
                                                {
                                                    Console.WriteLine("Unauthorized");
                                                    respond(ns, "Unauthorized");
                                                }
                                                break;
                                            }        
                                    }
                                    break;
                                }
                            case "GET":
                                {
                                    switch (dir)
                                    {
                                        case "/cards": // Get all cards
                                            {
                                                try
                                                {
                                                    Host = await reader.ReadLineAsync();
                                                    UserAgent = await reader.ReadLineAsync();
                                                    Accept = await reader.ReadLineAsync();
                                                    string Auth = await reader.ReadLineAsync();
                                                    string dispose = Auth.Split(' ')[0];
                                                    string AuthToken = Auth.Split(' ')[1]; // /users
                                                    Console.WriteLine(AuthToken);

                                                    //Package gui = JsonConvert.DeserializeObject<Package>(AuthToken);
                                                    Deck UserDeck = con.GetUserCards(new Guid(AuthToken));
                                                    //UserDeck.PrintCards();

                                                    List<string> loglist = UserDeck.PrintCardsRespond();
                                                    foreach (string logmsg in loglist)
                                                    {
                                                        Console.WriteLine(logmsg);
                                                        respond(ns, logmsg);
                                                    }
                                                }
                                                catch (Exception exc)
                                                {
                                                    Console.WriteLine("error occurred: " + exc.Message);
                                                }
                                                break;
                                            }
                                        case "/deck": // Show deck
                                            {
                                                try
                                                {
                                                    Host = await reader.ReadLineAsync();
                                                    UserAgent = await reader.ReadLineAsync();
                                                    Accept = await reader.ReadLineAsync();
                                                    string Auth = await reader.ReadLineAsync();
                                                    string dispose = Auth.Split(' ')[0];
                                                    string AuthToken = Auth.Split(' ')[1]; // /users
                                                    Console.WriteLine(AuthToken);
                                                    Deck UserDeck = con.GetUserCards(new Guid(AuthToken));
                                                    //UserDeck.PrintCardsSimple();

                                                    List<string> loglist = UserDeck.PrintCardsSimpleRespond();
                                                    foreach (string logmsg in loglist)
                                                    {
                                                        Console.WriteLine(logmsg);
                                                        respond(ns, logmsg);
                                                    }
                                                    break;
                                                }
                                                catch (Exception exc)
                                                {
                                                    Console.WriteLine("error occurred: " + exc.Message);
                                                    break;
                                                }
                                            }
                                        case "/deck?format=plain": // Show deck but different
                                            {
                                                try
                                                {
                                                    Host = await reader.ReadLineAsync();
                                                    UserAgent = await reader.ReadLineAsync();
                                                    Accept = await reader.ReadLineAsync();
                                                    string Auth = await reader.ReadLineAsync();
                                                    string dispose = Auth.Split(' ')[0];
                                                    string AuthToken = Auth.Split(' ')[1]; // /users
                                                    Console.WriteLine(AuthToken);
                                                    Deck UserDeck = con.GetUserCards(new Guid(AuthToken));
                                                    //UserDeck.PrintCardsAllInfo();

                                                    List<string> loglist = UserDeck.PrintCardsAllInfoRespond();
                                                    foreach (string logmsg in loglist)
                                                    {
                                                        Console.WriteLine(logmsg);
                                                        respond(ns, logmsg);
                                                    }
                                                    break;
                                                }
                                                catch (Exception exc)
                                                {
                                                    Console.WriteLine("error occurred: " + exc.Message);
                                                    break;
                                                }
                                            }
                                        case "/stats": // Show own stats
                                            {
                                                try
                                                {
                                                    Host = await reader.ReadLineAsync();
                                                    UserAgent = await reader.ReadLineAsync();
                                                    Accept = await reader.ReadLineAsync();
                                                    string Auth = await reader.ReadLineAsync();
                                                    string dispose = Auth.Split(' ')[0];
                                                    string AuthToken = Auth.Split(' ')[1]; // /users
                                                    Console.WriteLine(AuthToken);
                                                    Guid token = new Guid(AuthToken);
                                                    if (con.AuthByToken(token))
                                                    {
                                                        Guid user = con.GetMemberGuidByToken(token);
                                                        Stats stats = con.GetUserStats(user);
                                                        stats.PrintStats();

                                                        List<string> loglist = stats.PrintStatsRespond();
                                                        foreach (string logmsg in loglist)
                                                        {
                                                            Console.WriteLine(logmsg);
                                                            respond(ns, logmsg);
                                                        }
                                                    }
                                                }
                                                catch (Exception exc)
                                                {
                                                    Console.WriteLine("error occurred: " + exc.Message);
                                                }
                                                break;
                                            }
                                        case "/score": // Show scoreboard
                                            {
                                                try
                                                {
                                                    Host = await reader.ReadLineAsync();
                                                    UserAgent = await reader.ReadLineAsync();
                                                    Accept = await reader.ReadLineAsync();
                                                    string Auth = await reader.ReadLineAsync();
                                                    string dispose = Auth.Split(' ')[0];
                                                    string AuthToken = Auth.Split(' ')[1]; // /users
                                                    Console.WriteLine(AuthToken);
                                                    Guid token = new Guid(AuthToken);
                                                    if (con.AuthByToken(token))
                                                    {
                                                        List<HighScores> ScoreBoard = con.GetScoreboard();
                                                        foreach (HighScores h in ScoreBoard)
                                                        {
                                                            //h.PrintBoardLine();

                                                            List<string> loglist = h.PrintBoardLineRespond();
                                                            foreach (string logmsg in loglist)
                                                            {
                                                                Console.WriteLine(logmsg);
                                                                respond(ns, logmsg);
                                                            }
                                                        }

                                                        
                                                        HighScores.rank = 0;
                                                    }
                                                    else
                                                    {
                                                        Console.WriteLine("Unauthorized. Is your token valid?");
                                                        respond(ns, "Unauthorized. Is your token valid?");
                                                    }
                                                }
                                                catch (Exception exc)
                                                {
                                                    Console.WriteLine("error occurred: " + exc.Message);
                                                }
                                                break;
                                            }
                                    }
                                    break;
                                }                            
                            case "PUT":
                                {
                                    switch (dir)
                                    {
                                        case "/deck": // Decide cards for battles
                                            {
                                                try
                                                {
                                                    Host = await reader.ReadLineAsync();
                                                    UserAgent = await reader.ReadLineAsync();
                                                    Accept = await reader.ReadLineAsync();
                                                    string Auth = await reader.ReadLineAsync();


                                                    Content = await reader.ReadLineAsync();
                                                    string dispose = Content.Split(' ')[0];
                                                    string AuthToken = Content.Split(' ')[1];
                                                    Console.WriteLine(AuthToken);
                                                    string Content3 = await reader.ReadLineAsync();
                                                    string read = await reader.ReadLineAsync();
                                                    read = await reader.ReadLineAsync();
                                                    Console.WriteLine(read);

                                                    Package DeckList = JsonConvert.DeserializeObject<Package>(read);
                                                    if (DeckList.SetDeck.Count != 4)
                                                    {
                                                        // must be 4
                                                        Console.WriteLine("You must set 4 cards for your deck");
                                                        respond(ns, "You must set 4 cards for your deck at least");
                                                    }
                                                    else
                                                    {
                                                        if (con.AuthByToken(new Guid(AuthToken)))
                                                        {
                                                            con.SetDeck(DeckList.SetDeck);

                                                        }
                                                        else
                                                        {
                                                            //Not authorized
                                                            Console.WriteLine("You are not authorized to perform this action (maybe token expired?)");
                                                            respond(ns, "You are not authorized to perform this action (maybe token expired?)");
                                                        }
                                                    }

                                                }
                                                catch (Exception exc)
                                                {
                                                    Console.WriteLine("error occurred: " + exc.Message);
                                                    respond(ns, "An error occurred. Please make sure you have sent a correct request");
                                                }
                                                break;
                                            }

                                    }
                                    break;
                                }       
                            default:
                                // Error
                                Console.WriteLine("Unknown method");
                                respond(ns, "Unknown method");
                                break;
                        }

                    }
                }
            }
        }
        static async Task Main(string[] args)
        {
            
            TcpListener listener = new TcpListener(IPAddress.Loopback, 10001);
            TcpClient socket;
            listener.Start();
            Console.WriteLine("Server online");

            Console.CancelKeyPress += (senderc, e) => Environment.Exit(0);

            while (true)
            {
                try
                {
                    socket = await listener.AcceptTcpClientAsync();
                    ClientThreader UserClient = new ClientThreader();
                    UserClient.StartClient(socket);
                    
                }
                catch (Exception exc)
                {
                    Console.WriteLine("error occurred: " + exc.Message);
                }
            }

            socket.Close();
            listener.Stop();
        }


    }
}

