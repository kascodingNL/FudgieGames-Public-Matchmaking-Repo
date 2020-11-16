using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace Matchmaking.Structures
{
    public class GameServerPackets
    {
    }

    struct InitializeServerPacket : INetSerializable
    {
        public string IP;
        public int Port;

        public int Gamemode;

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(IP);
            writer.Put(Port);

            writer.Put(Gamemode);
        }

        public void Deserialize(NetDataReader reader)
        {
            IP = reader.GetString();
            Port = reader.GetInt();

            Gamemode = reader.GetInt();
        }
    }
}
