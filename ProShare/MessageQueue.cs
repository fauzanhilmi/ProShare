using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace ProShare
{
    public static class MessageQueue
    {
        private static string server = "localhost";
        private static string uid = "guest";
        private static string pwd = "guest";

        private static IConnection MQConn;

        public static void Connect()
        {
            ConnectionFactory CF = new ConnectionFactory();
            CF.HostName = server;
            CF.UserName = uid;
            CF.Password = pwd;

            MQConn = CF.CreateConnection();
        }
    }
}
