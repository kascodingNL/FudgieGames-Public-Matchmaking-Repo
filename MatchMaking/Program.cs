using Matchmaking.Datatypes;
using Matchmaking.Structures;
using MatchMaking;
using MatchMaking.Database;
using MatchMaking.Metrics;
using Sentry;
using System;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using System.Threading;

namespace Matchmaking
{
    class Program
    {
        public static MatchmakerServer GameConnector;
        public static ClientServer clientServer;

        public static DatabaseMethods db;

        static void Main(string[] args)
        {
            string Version = Assembly.GetExecutingAssembly().GetName().Version.ToString();

            System.Diagnostics.Stopwatch watch = System.Diagnostics.Stopwatch.StartNew();
            Console.WriteLine("Launching matchmaker V{0}!", Version);

            db = new DatabaseMethods();

            GameConnector = new MatchmakerServer();
            clientServer = new ClientServer();

            watch.Stop();
            Console.WriteLine("Launching took " + watch.Elapsed);

            new Thread(() =>
            {
                Console.WriteLine("Started ping thread.");
                while (true)
                {
                    foreach (var server in MatchmakerServer.gameServers.Values.ToList())
                    {
                        if (server.IP == null)
                        {
                            Ping pingSender = new Ping();
                            PingOptions options = new PingOptions();

                            options.DontFragment = false;

                            string data = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
                            byte[] buffer = Encoding.ASCII.GetBytes(data);
                            int timeout = 120;
                            try
                            {
                                PingReply reply = pingSender.Send(server.IP, timeout, buffer, options);
                                    //Console.WriteLine("IP: " + server.Value.IP + " RTT: " + reply.RoundtripTime);

                                    server.ApproxPing = reply.RoundtripTime;
                                if (server.IP == "161.97.123.52")
                                {
                                    server.ApproxPing += 500;
                                }
                            }
                            catch (Exception e)
                            {
                                server.ApproxPing = 10000;
                            }
                        }
                    }
                    Thread.Sleep(5000);
                }
            }).Start();
            Thread.Sleep(100);
            HandleLoop();
        }

        public static int testTimes = 0;

        public static void HandleLoop()
        {
            Console.WriteLine("Started polling");
            while(true)
            {
                Thread.Sleep(100);
                //Console.WriteLine("Poll");
                //Console.WriteLine("Polled Events");
                clientServer.server.PollEvents();
                GameConnector.server.PollEvents();

                foreach(var client in ClientServer.clients)
                {
                    GameServer currentBest = null;
                    if(client.Value.Queued && !client.Value.Matching)
                    {
                        foreach(var server in MatchmakerServer.gameServers)
                        {
                            if(server.Value.Available && server.Value.gamemode == client.Value.wantedGamemode
                                && server.Value.Players + 1 <= 10)
                            {
                                if (currentBest == null)
                                    currentBest = server.Value;

                                if(server.Value.ApproxPing < currentBest.ApproxPing)
                                {
                                    if (server.Value.Players > currentBest.Players)
                                    {
                                        currentBest = server.Value;
                                    }
                                }
                                else if(server.Value.Players > currentBest.Players)
                                {
                                    currentBest = server.Value;
                                }
                            }
                        }

                        if (currentBest != null)
                        {
                            client.Value.Queued = false;
                            currentBest.Players++;

                            var FoundPacket = new MatchFound()
                            {
                                IP = currentBest.IP,
                                Port = currentBest.Port,
                                mapId = currentBest.mapId
                            };

                            Console.WriteLine(string.Format("Match has been found for " +
                                "server with IP {0}", currentBest.IP + " in MapId " + currentBest.mapId));
                            clientServer.packetProcessor.SendNetSerializable(client.Key, FoundPacket, LiteNetLib.DeliveryMethod.ReliableOrdered);
                        }
                    }
                }
            }
        }
    }
}