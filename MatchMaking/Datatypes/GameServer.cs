using Matchmaking.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Matchmaking.Datatypes
{
    public class GameServer
    {
        public string IP { get; set; }
        public int Port { get; set; }

        public int mapId = 1;

        public GameModeEnum gamemode { get; set; }
        public bool Available = false;
        public int Players;

        public long ApproxPing;

        public GameServer()
        {

        }

        public GameServer(string IP, int Port)
        {
            this.IP = IP;
            this.Port = Port;
            this.gamemode = GameModeEnum.UNRATED;
        }

        public GameServer(string IP, int Port, int mapId, int gamemode)
        {
            this.IP = IP;
            this.Port = Port;
            this.gamemode = (GameModeEnum) gamemode;
            this.mapId = mapId;
        }

        public GameServer(string IP, int Port, GameModeEnum gamemode)
        {
            this.IP = IP;
            this.Port = Port;
            this.gamemode = gamemode;
        }

        public GameServer(string IP, int Port, int gamemode)
        {
            this.IP = IP;
            this.Port = Port;
            this.gamemode = (GameModeEnum)gamemode;
            Available = true;
        }
    }
}
