using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Windows.Forms;
using System.Diagnostics;

namespace ProShare
{
    public static class MQHandler
    {
        private static string server = "localhost";
        private static string uid = "guest";
        private static string pwd = "guest";

        //not needed anymore?
        /*private static string generateExchange = "generate";
        private static string reconstructExchange = "reconstruct";
        private static string updateExchange = "update";*/

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

        /*                  Generate Methods            */
        //Called during register
        public static void CreateQueue(string name)
        {
            model.QueueDeclare(name, true, false, false);

            //not needed anymore?
            /*model.QueueBind(name, generateExchange, name);
            model.QueueBind(name, reconstructExchange, name);
            model.QueueBind(name, updateExchange, name);*/
        }

        //Pulling messages from given queue, then invoked callback function
        public static void GetMessage(string queue, Action<ulong, IDictionary<string, object>, byte[]> callback)
        {
            ulong dTag = 0;
            EventingBasicConsumer consumer = new EventingBasicConsumer(model);
            consumer.Received += (o, e) =>
            {
                callback.Invoke(e.DeliveryTag, e.BasicProperties.Headers, e.Body);
                dTag = e.DeliveryTag;
                //TEST
                /*if (dTag == 2)
                {
                    Debug.WriteLine("ack");
                    model.BasicAck(dTag, false);
                }*/
            };
            model.BasicConsume(queue, false, consumer);
        }

        //Acknowleges a message
        public static void Ack(ulong deliveryTag)
        {
            model.BasicAck(deliveryTag, false);
        }

        public static void SendDirectMessage(string type, string operation, string scheme, string sender, string dest, byte[] message)
        {
            //byte[] messageByte = System.Text.Encoding.UTF8.GetBytes((char[]) message); //this casting may becomes problematic

            IBasicProperties props = model.CreateBasicProperties();
            //props.ContentType = "application/octet-stream";
            props.DeliveryMode = 2;
            props.Headers = new Dictionary<string, object>();
            props.Headers.Add("Type", type);
            props.Headers.Add("Operation", operation);
            props.Headers.Add("Scheme", scheme);
            props.Headers.Add("Sender", sender);

            model.BasicPublish(scheme, dest, props, message);
        }

        public static void SendFanoutMessages(string type, string operation, string scheme, string sender, byte[] message)
        {
            IBasicProperties props = model.CreateBasicProperties();
            //props.ContentType = "application/octet-stream";
            props.DeliveryMode = 2;
            props.Headers = new Dictionary<string, object>();
            props.Headers.Add("Type", type);
            props.Headers.Add("Operation", operation);
            props.Headers.Add("Scheme", scheme);
            props.Headers.Add("Sender", sender);

            model.BasicPublish(scheme, "#", props, message);
        }

        public static void DeleteExchange(string exchange)
        {
            //try
            //{
                model.ExchangeDelete(exchange);
            //}
            //catch (RabbitMQ.Client.Exceptions.)
        }
        /*public static void SendShareRequest(string scheme, string dealer, string player)
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
        }*/

        /*              GENERATE methods               */
        //Called by dealer to send share requests to all players
        public static void SendShareRequests(string scheme, string dealer, List<string> players)
        {
            model.ExchangeDeclare(scheme, ExchangeType.Topic, true, false);

            foreach(string player in players)
            {
                //Bind every player into exchange
                model.QueueBind(player, scheme, "#");
                model.QueueBind(player, scheme, player);
            }

            byte[] message = System.Text.Encoding.UTF8.GetBytes(""); //not important
            IBasicProperties props = model.CreateBasicProperties();
            //props.ContentType = "text/plain";
            props.DeliveryMode = 2;
            props.Headers = new Dictionary<string, object>();
            props.Headers.Add("Type", "REQUEST");
            props.Headers.Add("Operation", "Generate");
            props.Headers.Add("Scheme", scheme);
            props.Headers.Add("Sender", dealer);

            model.BasicPublish(scheme, "#", props, message);

        }

        public static void Close()
        {
            model.Close();
            conn.Close();
        }
    }
}
