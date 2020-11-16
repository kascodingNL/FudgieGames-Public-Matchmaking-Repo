using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace Matchmaking.Structures
{
    public class GameServerPackets
    {
    }

    struct PlayerUpdate : INetSerializable
    {
        public string SessionId;

        public int Kills;
        public int Deaths;

        public void Deserialize(NetDataReader reader)
        {
            SessionId = reader.GetString();

            Kills = reader.GetInt();
            Deaths = reader.GetInt();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(SessionId);

            writer.Put(Kills);
            writer.Put(Deaths);
        }
    }

    struct PlayerCountUpdate : INetSerializable
    {
        public int playerCount;

        public void Deserialize(NetDataReader reader)
        {
            playerCount = reader.GetInt();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(playerCount);
        }
    }

    struct InitializeServerPacket : INetSerializable
    {
        public string IP;
        public int Port;

        public int Gamemode;
        public int mapId;

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(IP);
            writer.Put(Port);

            writer.Put(Gamemode);
            writer.Put(mapId);
        }

        public void Deserialize(NetDataReader reader)
        {
            IP = reader.GetString();
            Port = reader.GetInt();

            Gamemode = reader.GetInt();
            mapId = reader.GetInt();
        }
    }

    struct ConfirmReceive : INetSerializable
    {
        public bool confirmed;

        public void Deserialize(NetDataReader reader)
        {
            confirmed = reader.GetBool();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(confirmed);
        }
    }
}
