using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatchMaking.Database.SimulatedResponses
{
    public class SimulatedUserLookup
    {
        public bool success { get; private set; }
        public string username { get; private set; }

        public SimulatedUserLookup(bool success, string username)
        {
            this.success = success;
            this.username = username;
        }
    }
}
