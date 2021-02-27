using LiteNetLib;
using LiteNetLib.Utils;
using Matchmaking.Enums;
using Matchmaking.Structures;
using Sentry;
using System;
using System.Collections.Generic;
using System.Text;

namespace Matchmaking.Datatypes
{
    public class MatchmakerServer
    {
        public NetPacketProcessor packetProcessor;

        public EventBasedNetListener listener;
        public NetManager server { get; private set; }

        public static Dictionary<NetPeer, GameServer> gameServers = new Dictionary<NetPeer, GameServer>();

        public MatchmakerServer()
        {
            Console.WriteLine("Starting GameServer");
            packetProcessor = new NetPacketProcessor();
            listener = new EventBasedNetListener();
            server = new NetManager(listener);

            packetProcessor.SubscribeNetSerializable<InitializeServerPacket, NetPeer>(HandleInitialized);
            packetProcessor.SubscribeNetSerializable<PlayerCountUpdate, NetPeer>(PlayerCountUpdated);
            packetProcessor.SubscribeNetSerializable<PlayerUpdate, NetPeer>(HandlePlayerUpdate);

            Console.WriteLine("Start GameServer: " + server.Start(25586));

            listener.ConnectionRequestEvent += request =>
            {
                request.Accept();
            };

            listener.PeerConnectedEvent += peer =>
            {
                Console.WriteLine("Got an connect on GameServer channel");
                gameServers.Add(peer, new GameServer());
            };

            listener.PeerDisconnectedEvent += (peer, disconnectinfo) =>
            {
                if (gameServers.ContainsKey(peer))
                {
                    GameServer server = gameServers[peer];
                    Console.WriteLine("Server with IP " + server.IP + " and port "
                        + server.Port + " has disconnected.");

                    gameServers.Remove(peer);
                }
                else
                {
                    Console.WriteLine("Tried to remove server that doesn't exist");
                }
            };

            listener.NetworkReceiveEvent += (peer, reader, delivery) =>
            {
                //Console.WriteLine("ReceivePacket");
                try
                {
                    packetProcessor.ReadAllPackets(reader, peer);
                }
                catch(Exception e)
                {
                    throw e;
                }
            };
        }

        private void HandlePlayerUpdate(PlayerUpdate packet, NetPeer peer)
        {
            GameClient client = null;

            foreach(var gClient in ClientServer.clients.Values)
            {
                if(gClient.SessionId == packet.SessionId)
                {
                    client = gClient;
                    break;
                }
            }

            if (client != null)
            {
                //int MMR = client.MMR;

                int Kills = packet.Kills;
                int Deaths = packet.Deaths;

                int KD = (int) Kills / Deaths;

                // int PercentageOfOld = MMR / 100 * KD;
                /*if (PercentageOfOld < 1 && PercentageOfOld > 0)
                {
                    int cachedOld = PercentageOfOld;

                    PercentageOfOld = 0;
                    PercentageOfOld -= cachedOld;
                }*/

                //MMR = (MMR / 100 * KD) + (MMR * (KD/10));
                //client.MMR = MMR;
                //Program.db.SetMMR(packet.SessionId, MMR);
            }
        }

        private void PlayerCountUpdated(PlayerCountUpdate packet, NetPeer peer)
        {
            if(gameServers.ContainsKey(peer))
            {
                var server = gameServers[peer];
                server.Players = packet.playerCount;
            }
        }

        private void HandleInitialized(InitializeServerPacket packet, NetPeer peer)
        {
            if (gameServers.ContainsKey(peer))
            {
                var server = gameServers[peer];

                //gameServers.Add(peer, new GameServer(packet.IP, packet.Port, packet.Gamemode));

                server.IP = packet.IP;
                server.Port = packet.Port;
                server.mapId = packet.mapId;
                server.gamemode = (GameModeEnum)packet.Gamemode;

                server.Available = true;

                Console.WriteLine("IP: " + server.IP + ", port: " + server.Port + " Map: " + server.mapId);

                var ConfirmPacket = new ConfirmReceive()
                {
                    confirmed = true
                };

                packetProcessor.SendNetSerializable(peer, ConfirmPacket, DeliveryMethod.ReliableOrdered);
            }
            else
            {
                SentrySdk.CaptureMessage("Server was already initialized(Double initialize Exception?)", Sentry.SentryLevel.Warning);
                Console.WriteLine("List already contained server!");
            }
        }
    }
}