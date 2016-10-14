using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace ProShare
{
    public static class MQHandler
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

        public static void SendShareRequest(string scheme, string dealer, string player)
        {
            byte[] message = System.Text.Encoding.UTF8.GetBytes(""); //not important

            IBasicProperties props = model.CreateBasicProperties();
            props.ContentType = "text/plain";
            props.DeliveryMode = 2;
            props.Headers = new Dictionary<string, object>();
            props.Headers.Add("Type", "REQUEST");
            props.Headers.Add("Operation", "Generate");
            props.Headers.Add("Scheme", scheme);
            props.Headers.Add("Sender", dealer);
            props.Headers.Add("Recipient", player);

            model.BasicPublish(generateExchange, player, props, message);
        }

        public static void Close()
        {
            model.Close();
            conn.Close();
        }
    }
}
