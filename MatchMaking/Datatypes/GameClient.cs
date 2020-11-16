using Matchmaking.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Matchmaking.Datatypes
{
    public class GameClient
    {
        //Auth data
        public string SessionId;

        public bool Queued;
        public GameModeEnum wantedGamemode;

        public bool Matching = false;

        public GameClient()
        {

        }
    }
}
