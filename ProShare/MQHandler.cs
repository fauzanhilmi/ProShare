﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System.Windows.Forms;
using System.Diagnostics;

namespace ProShare
{
    public static class MQHandler
    {
        private static string localServer = "localhost";
        private static string localUid = "guest";
        private static string localPwd = "guest";
        private static string remoteServer = "128.199.217.88";
        private static string remoteUid = "fauzan";
        private static string remotePwd = "fauzan";

        //not needed anymore?
        /*private static string generateExchange = "generate";
        private static string reconstructExchange = "reconstruct";
        private static string updateExchange = "update";*/

        private static IConnection conn;
        private static IModel model;

        public static string Fanout = "Players";

        public static void Connect()
        {
            try
            {
                ConnectionFactory CF = new ConnectionFactory();

                string env = "remote"; //GANTI ENVIRONMENT DISINI
                if (env == "remote")
                {
                    CF.HostName = remoteServer;
                    CF.UserName = remoteUid;
                    CF.Password = remotePwd;
                }
                else if (env == "local")
                {
                    CF.HostName = localServer;
                    CF.UserName = localUid;
                    CF.Password = localPwd;
                }
                //CF.uri = new Uri("amqps://fauzan:fauzan@128.199.217.88:5671/");

                //TEST SSL
                //CF.Ssl.ServerName = "localhost";
                CF.Ssl.Enabled = false;
                CF.Ssl.ServerName = System.Net.Dns.GetHostName();
                //CF.Port = 5671;
                //CF.Ssl.Version = System.Security.Authentication.SslProtocols.Tls12;
                CF.Ssl.CertPath = "keycert.p12";
                CF.Ssl.CertPassphrase = "Seiryukan7";
                //END OF TEST SSL

                conn = CF.CreateConnection();
                model = conn.CreateModel();
            }
            catch (BrokerUnreachableException bex)
           {
                Exception ex = bex;
                while (ex != null)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine("inner:");
                    ex = ex.InnerException;
                }
            }
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
            EventingBasicConsumer consumer = new EventingBasicConsumer(model);
            consumer.Received += (o, e) =>
            {
                callback.Invoke(e.DeliveryTag, e.BasicProperties.Headers, e.Body);
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

        public static void SendDirectMessage(string operation, string type, string scheme, string sender, string dest, byte[] message)
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
            Debug.WriteLine("Sending direct " + type + " message from "+ sender + " to " + dest);
        }

        public static void SendFanoutMessages(string operation, string type, string scheme, string sender, byte[] message)
        {
            IBasicProperties props = model.CreateBasicProperties();
            //props.ContentType = "application/octet-stream";
            props.DeliveryMode = 2;
            props.Headers = new Dictionary<string, object>();
            props.Headers.Add("Type", type);
            props.Headers.Add("Operation", operation);
            props.Headers.Add("Scheme", scheme);
            props.Headers.Add("Sender", sender);
            Debug.WriteLine("Sending fanout " + type + "message from " + sender);
            model.BasicPublish(scheme, Fanout, props, message);
            
        }

        public static void DeleteExchange(string exchange)
        {
            model.ExchangeDelete(exchange);
        }
        /*              GENERATE methods               */
        //Called by dealer to send share requests to all players
        public static void SendShareRequests(string scheme, string dealer, List<string> players)
        {
            //TEST
            //model.ExchangeDeclare(scheme, ExchangeType.Topic, true, false);
            model.ExchangeDeclare(scheme, ExchangeType.Direct, true, false);

            //Bind dealer
            model.QueueBind(dealer, scheme, dealer);
            foreach(string player in players)
            {
                //Bind every player into exchange
                model.QueueBind(player, scheme, Fanout);
                model.QueueBind(player, scheme, player);
            }

            byte[] message = System.Text.Encoding.UTF8.GetBytes(""); //not important
            SendFanoutMessages("Generate", "Request", scheme, dealer, message);

            
            /*IBasicProperties props = model.CreateBasicProperties();
            //props.ContentType = "text/plain";
            props.DeliveryMode = 2;
            props.Headers = new Dictionary<string, object>();
            props.Headers.Add("Operation", "Generate");
            props.Headers.Add("Type", "Request");
            props.Headers.Add("Scheme", scheme);
            props.Headers.Add("Sender", dealer);

            model.BasicPublish(scheme, "#", props, message);*/
        }

        public static void Close()
        {
            model.Close();
            conn.Close();
        }
    }
}
