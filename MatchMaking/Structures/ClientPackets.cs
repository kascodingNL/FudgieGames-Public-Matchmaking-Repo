using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace Matchmaking.Structures
{
    public class ClientPackets
    {
    }

    struct JoinQueue : INetSerializable
    {
        public string SessionId;
        public int wantedGamemode;

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(SessionId);
            writer.Put(wantedGamemode);
        }

        public void Deserialize(NetDataReader reader)
        {
            SessionId = reader.GetString();
            wantedGamemode = reader.GetInt();
        }
    }

    struct MatchFound : INetSerializable
    {
        public string IP;
        public int Port;

        public int mapId;

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(IP);
            writer.Put(Port);

            writer.Put(mapId);
        }

        public void Deserialize(NetDataReader reader)
        {
            IP = reader.GetString();
            Port = reader.GetInt();

            mapId = reader.GetInt();
        }
    }
}
