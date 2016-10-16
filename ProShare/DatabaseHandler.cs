using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Management;
using System.Diagnostics;
using System.Windows.Forms;

namespace ProShare
{
    public static class DatabaseHandler
    {
        private static string server = "127.0.0.1";
        private static string uid = "root";
        private static string pwd = "";
        private static string db = "ProShare";

        private static string macAddress;
        private static MySql.Data.MySqlClient.MySqlConnection SqlConn;

        static DatabaseHandler()
        {
            /* Getting MAC address */
            ManagementObjectSearcher objMOS = new ManagementObjectSearcher("Select * FROM Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection objMOC = objMOS.Get();

            macAddress = "";
            foreach (ManagementObject objMO in objMOC)
            {
                object tempMacAddrObj = objMO["MacAddress"];

                if (tempMacAddrObj == null) //Skip objects without a MACAddress
                {
                    continue;
                }
                if (macAddress == String.Empty) // only return MAC Address from first card that has a MAC Address
                {
                    macAddress = tempMacAddrObj.ToString();
                }
                objMO.Dispose();
            }
            macAddress = macAddress.Replace(":", "");

            //TEST
            //macAddress = GetRandomMacAddress();
            //macAddress = "A9D37DD1D8F0";
            //TEST
        }

        /* Connect to MySQL server */
        public static void Connect()
        {
            try
            {
                string SqlConnString = "server=" + server + ";uid=" + uid + ";pwd=" + pwd + ";database=" + db;
                SqlConn = new MySql.Data.MySqlClient.MySqlConnection(SqlConnString);
                SqlConn.Open();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex) //is it properly handled?
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        /* Close SQL Connection */
        public static void Close()
        {
            try
            {
                SqlConn.Close();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex) //is it properly handled?
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        /* ************************ ACCOUNT-related methods ******************* */
        /* Generates random MAC Address, FOR TESTING ONLY */
        private static string GetRandomMacAddress()
        {
            var random = new Random();
            var buffer = new byte[6];
            random.NextBytes(buffer);
            var result = String.Concat(buffer.Select(x => string.Format("{0}:", x.ToString("X2"))).ToArray());
            result = result.Replace(":", "");
            return result;
        }

        /* Add new user with detected MAC Address and given username & password to table 'Account'
         * Will return 3 possible return codes
         * > 1      : Operation is successfully executed 
         * > 0      : Record already exists; Operation is not executed 
         * > else   : Unknown error, return code = SQL error code; Operation is not executed  */
        public static int AddAccount(string username, string password)
        {
            int returnCode = 1; //default : operation is successful
            try
            {
                string query = "INSERT INTO account (mac_address, username, password)  VALUES ('" + macAddress + "', '" + username + "', '" + password + "')";
                Debug.WriteLine(query);
                MySqlCommand cmd = new MySqlCommand(query, SqlConn);
                cmd.ExecuteNonQuery();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                if (ex.Number == 1062) //Account already exists
                {
                    returnCode = 0;
                }
                else
                {
                    returnCode = ex.Number;
                    Debug.WriteLine(ex.Number + " : " + ex.Message);
                    throw;
                }
                //case username is too long?
                //more error codes?
            }
            return returnCode;
        }

        /* Check whether an account already exists or not
         * Will return 4 possible return codes
         * >  1      : Account is exist, username & password match
         * >  0      : Account is exist, but given username & password don't match
         * > -1      : Account is not exist
         * >  else   : Unknown error. Return code = MySQL error code */
        public static int DoesAccountExist(string username, string password)
        {
            int returnCode = -1; //default : not found
            try
            {
                string query = "SELECT * FROM account WHERE mac_address = '" + macAddress + "'";
                MySqlCommand cmd = new MySqlCommand(query, SqlConn);
                MySqlDataReader reader = cmd.ExecuteReader();
                Debug.WriteLine(query);
                while (reader.Read())
                {
                    returnCode = 0; //found matching mac address, but not sure about the username &  password
                    Debug.WriteLine(reader[0] + " " + reader[1] + " " + reader[2]);
                }

                if(returnCode == 0)
                {
                    if (reader[1].ToString() == username && reader[2].ToString() == password)
                    {
                        returnCode = 1; //found matching username & password
                    }
                    else
                    {
                        returnCode = 0; //mismatch username or password
                    }
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex) 
            {
                Debug.WriteLine(ex.Number + " " + ex.Message);
                returnCode = ex.Number;
                throw;
            }
            return returnCode;
        }

        /* Check whether all accounts in names exist or not
         * Will return 3 possible return codes
         * >  1      : All accounts exist
         * >  0      : There is account that does not exist
         * >  else   : Unknown error. Return code = MySQL error code */
        public static int DoAccountsExist(List<string> names)
        {
            int returnCode = 0;
            try
            {
                string query = "SELECT COUNT(username) FROM account WHERE username in (";
                for (int i = 0; i < names.Count; i++)
                {
                    query = query + "'" + names[i] + "'";
                    if (i < names.Count - 1)
                    {
                        query += ",";
                    }
                }
                query += ")";
                Debug.WriteLine(query);
                MySqlCommand cmd = new MySqlCommand(query, SqlConn);
                MySqlDataReader reader = cmd.ExecuteReader();
                int numAccounts = 0;
                while (reader.Read())
                {
                    numAccounts = reader.GetInt32(0);
                }
                Debug.WriteLine("Found " + numAccounts+"/"+names.Count);
                if (numAccounts == names.Count)
                {
                    returnCode = 1;
                }
                else if (numAccounts < names.Count)
                {
                    returnCode = 0;
                }
                //else does not possible
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                Debug.WriteLine(ex.Number + " " + ex.Message);
                returnCode = ex.Number;
                throw;
            }
            return returnCode;
        }

        /* ************************ SCHEME-related methods ******************* */
        /* UNTESTED UNTUK DEALER! */
        /* Add new scheme with given name, k value, and n value 
         * Will return 3 possible return codes
         * > 1      : Operation is successfully executed 
         * > 0      : Record already exists; Operation is not executed 
         * > else   : Unknown error, return code = SQL error code; Operation is not executed  */
        public static int AddScheme(string scheme, string dealer, byte k, byte n) /* UNTESTED UNTUK DEALER! */
        {
            int returnCode = 1;
            try
            {
                string query = "INSERT INTO scheme (name, dealer, k, n)  VALUES ('" + scheme + "', '"  + dealer+ "', '" + k + "', '" + n + "')";
                Debug.WriteLine(query);
                MySqlCommand cmd = new MySqlCommand(query, SqlConn);
                cmd.ExecuteNonQuery();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                if (ex.Number == 1062) //Scheme already exists
                {
                    returnCode = 0;
                }
                else
                {
                    returnCode = ex.Number;
                    Debug.WriteLine(ex.Number + " : " + ex.Message);
                    throw;
                }
            }
            return returnCode;
        }

        /* ALREADY COVERED IN ADDSCHEME */
        /* Check whether a scheme already exists or not 
         * Will return 3 possible return codes
         * >  1      : Scheme exists
         * >  0      : Scheme does not exist
         * >  else   : Unknown error. Return code = MySQL error code */
        /*public static int DoesSchemeExist(string name)
        {
            int returnCode = 0;
            try
            {
                string query = "SELECT * from scheme WHERE name = '" + name + "'";
                MySqlCommand cmd = new MySqlCommand(query, SqlConn);
                MySqlDataReader reader = cmd.ExecuteReader();
                Debug.WriteLine(query);
                while (reader.Read())
                {
                    returnCode = 1; //found scheme
                    Debug.WriteLine("Found scheme "+reader[0]);
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                Debug.WriteLine(ex.Number + " " + ex.Message);
                returnCode = ex.Number;
            }
            return returnCode;
        }*/

        /* ************************ PLAYERS-related methods ******************* */
        /* Add new players given scheme name & list of players
        * It is assumed that no scheme & player combinations have already exist before
        * Will return 2 possible return codes
        * > 1      : Operation is successfully executed 
        * > else   : Unknown error, return code = SQL error code; Operation is not executed  */
        public static int AddPlayers(string scheme, List<string> players)
        {
            int returnCode = 1;
            try
            {
                string query = "INSERT INTO players (scheme, player)  VALUES ";
                for(int i=0; i<players.Count; i++)
                {
                    query = query + "('" + scheme + "', '" + players[i] + "')";
                    if(i<players.Count-1)
                    {
                        query += ", ";
                    }
                }
                Debug.WriteLine(query);
                MySqlCommand cmd = new MySqlCommand(query, SqlConn);
                cmd.ExecuteNonQuery();
                returnCode = 1;
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                Debug.WriteLine(ex.Number + " " + ex.Message);
                returnCode = ex.Number;
                throw;
            }
            return returnCode;
        }
    }
}
