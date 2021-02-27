using MatchMaking.Database.SimulatedResponses;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MatchMaking.Database
{
    public class DatabaseMethods
    {
        private string connectionString;

        public MySqlConnection con;
        public MySqlCommand cmd;
        public MySqlDataReader reader;

        public DatabaseMethods()
        {
            connectionString = "Unneeded, just for auth";

            try
            {
                con = new MySqlConnection(connectionString);
            } catch(Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public int GetMMR(string sessionId)
        {
            con.Open();

            string query = "SELECT MMR FROM account WHERE currentguid=@SessionID";
            MySqlCommand cmd = new MySqlCommand(query, con);
            cmd.Parameters.AddWithValue("@SessionID", sessionId);

            var reader = cmd.ExecuteReader();
            int MMR = 0;

            while(reader.Read())
            {
                MMR = int.Parse(reader["MMR"].ToString());
            }

            con.Close();
            return MMR;
        }

        public void SetMMR(string sessionId, int MMR)
        {
            con.Open();
            string query = "SET MMR = " + MMR + " WHERE currentguid=@SessionID";
            MySqlCommand cmd = new MySqlCommand(query, con);
            cmd.Parameters.AddWithValue("@SessionID", sessionId);

            var reader = cmd.ExecuteReader();
            while(reader.Read())
            {

            }

            con.Close();
        }

        public bool SessionIdValid(string sessionId)
        {
            throw new NotImplementedException("bool SessionIdValid() is not yet implemented, implement it yourself or wait until the author introduces it.");
        }

        public SimulatedUserLookup GetUsernameWithGuid(string Guid)
        {
            con.Open();
            string query = "SELECT username FROM account WHERE guid=@Guid";

            MySqlCommand cmd = new MySqlCommand(query, con);
            cmd.Parameters.AddWithValue("@Guid", Guid);

            string usrReturning = string.Empty;
            bool success = false;

            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                usrReturning = reader["username"].ToString();
            }

            if (usrReturning != string.Empty)
            {
                success = true;
            }
            con.Close();
            return new SimulatedUserLookup(success, usrReturning);
        }
    }
}
