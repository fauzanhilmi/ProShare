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
        private static string generateExchange = "generate";
        private static string reconstructExchange = "reconstruct";
        private static string updateExchange = "update";

        private static IConnection conn;
        private static IModel model;


        public static void Connect()
        {
            ConnectionFactory CF = new ConnectionFactory();
            CF.HostName = server;
            CF.UserName = uid;
            CF.Password = pwd;

            conn = CF.CreateConnection();
            model = conn.CreateModel();
        }

        public static void CreateQueue(string name)
        {
            model.QueueDeclare(name, true, false, false);
            model.QueueBind(name, generateExchange, name);
            model.QueueBind(name, reconstructExchange, name);
            model.QueueBind(name, updateExchange, name);
        }

        public static void Close()
        {
            model.Close();
            conn.Close();
        }
    }
}
