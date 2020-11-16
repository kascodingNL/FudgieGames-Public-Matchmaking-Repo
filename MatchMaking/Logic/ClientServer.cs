using LiteNetLib;
using LiteNetLib.Utils;
using Matchmaking.Enums;
using Matchmaking.Structures;
using MatchMaking;
using MatchMaking.Database;
using Sentry;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Matchmaking.Datatypes
{
    public class ClientServer
    {
        public NetPacketProcessor packetProcessor;

        public EventBasedNetListener listener;
        public NetManager server { get; private set; }

        public static Dictionary<NetPeer, GameClient> clients;

        public ClientServer()
        {
            Console.WriteLine("Starting ClientServer");
            clients = new Dictionary<NetPeer, GameClient>();

            packetProcessor = new NetPacketProcessor();
            listener = new EventBasedNetListener();
            server = new NetManager(listener);

            Console.WriteLine("Start ClientServer: " + server.Start(25583));

            packetProcessor.SubscribeNetSerializable<JoinQueue, NetPeer>(JoinQueuePacket);

            listener.ConnectionRequestEvent += request =>
            {
                Console.WriteLine("Connect Request");
                request.AcceptIfKey("RandomKey123");
            };

            listener.PeerConnectedEvent += peer =>
            {
                Console.WriteLine("Got an connect on GameClient channel");
                clients.Add(peer, new GameClient());
            };

            listener.PeerDisconnectedEvent += (peer, disconnectinfo) =>
            {
                GameClient client = clients[peer];
                clients.Remove(peer);
            };

            listener.NetworkReceiveEvent += (fromPeer, reader, delivery) =>
            {
                try
                {
                    packetProcessor.ReadAllPackets(reader, fromPeer);
                }
                catch (Exception e)
                {
                    //SentrySdk.CaptureException(e);
                }
            };
        }

        private void JoinQueuePacket(JoinQueue packet, NetPeer peer)
        {
            var lookup = Program.db.GetUsernameWithGuid(packet.SessionId);
            if (lookup.success)
            {
                Console.WriteLine("Connection on IP " + peer.EndPoint.Address.ToString() + " tried to connect using invalid SessionID(" + packet.SessionId + ")");
                clients[peer].Queued = false;
                return;
            }
            Console.WriteLine("Someone joined queue");
            GameClient client = clients[peer];

            client.SessionId = packet.SessionId;
            client.wantedGamemode = (GameModeEnum)packet.wantedGamemode;
            //client.MMR = Program.db.GetMMR(packet.SessionId);

            //Console.WriteLine("SesId: " + packet.SessionId);

            client.Queued = true;
        }
    }
}
