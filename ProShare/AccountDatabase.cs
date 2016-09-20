using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Management;
using System.Diagnostics;

namespace ProShare
{
    public static class AccountDatabase
    {
        private static string server = "127.0.0.1";
        private static string uid = "root";
        private static string pwd = "";
        private static string db = "ProShare";

        private static string macAddress = "test";
        private static MySql.Data.MySqlClient.MySqlConnection SqlConn;

        static AccountDatabase()
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

        /* Add new record with detected MAC Address and given username & password to table 'Account'
         * Will return 3 possible return codes
         * > 1      : Operation is successfully executed 
         * > 0      : Record already exists; Operation is not executed 
         * > else   : Unknown error, return code = SQL error code; Operation is not executed  */
        public static int Add(string username, string password)
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
                }
                //case username is too long?
                //more error codes?
            }
            return returnCode;
        }

        /* Check whether an account is already exist or not
         * Will return 4 possible return codes
         * >  1      : Account is exist
         * > -1      : Account is exist, but given username & password don't match
         * >  0      : Account is not exist
         * >  else   : Unknown error. Return code = MySQL error code */
        public static int isExist (string username, string password)
        {
            int returnCode = 0; //default : not found
            try
            {
                string query = "SELECT * FROM account WHERE mac_address = '" + macAddress + "'";
                MySqlCommand cmd = new MySqlCommand(query, SqlConn);
                MySqlDataReader reader = cmd.ExecuteReader();
                Debug.WriteLine(query);
                while (reader.Read())
                {
                    returnCode = -1; //found matching mac address, but not sure about the username &  password
                    Debug.WriteLine(reader[0] + " " + reader[1] + " " + reader[2]);
                }

                if(returnCode == -1)
                {
                    if (reader[1].ToString() == username && reader[2].ToString() == password)
                    {
                        returnCode = 1; //found matching username & password
                    }
                    else
                    {
                        returnCode = -1; //mismatch username or password
                    }
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex) 
            {
                Debug.WriteLine(ex.Number + " " + ex.Message);
                returnCode = ex.Number;
            }
            return returnCode;
        }

        public static void CloseConnection()
        {
            SqlConn.Close();
        }
    }
}
