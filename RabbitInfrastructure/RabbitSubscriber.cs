﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitInfrastructure
{



    public class RabbitSubscriber
    {
        string _exchangeName;
        string _queueName;
        string _hostName;
        IEnumerable<IHandlerConfig> _cfg;

        RabbitPublisher _pub;

        public RabbitSubscriber(string exchangeName, string queueName, string hostName, IEnumerable<IHandlerConfig> cfg) 
        {
            _exchangeName=exchangeName;
            _queueName = queueName;
            _hostName = hostName;
            _cfg = cfg;
            _pub = new RabbitPublisher(_exchangeName, hostName);
        }

        public static void MessageLoop(string serviceName, IEnumerable<IHandlerConfig> cfg)
        {
            Console.WriteLine("*** {0} ***", serviceName);
            var subscriber = new RabbitSubscriber(RabbitCfg.XCHG, serviceName, RabbitCfg.HOST, cfg);
            subscriber.MessageLoop();
        }

        public void MessageLoop() 
        {
            var factory = new ConnectionFactory()
            {
                Protocol = Protocols.FromEnvironment(),
                HostName = _hostName
            };
            using (var con = factory.CreateConnection())
            using (var mod = con.CreateModel())
            {
                mod.QueueDeclare(_queueName, true, false, false, null);
                foreach (var h in _cfg)
                {
                    var xchg = _exchangeName.ToLowerInvariant();
                    var rk = h.MessageType.FullName.ToLowerInvariant();
                    Console.Write("Binding {0}:{1}:{2}... ", xchg, rk, _queueName);
                    mod.ExchangeDeclare(xchg, ExchangeType.Direct, true);
                    mod.QueueBind(_queueName, xchg, rk);
                    Console.WriteLine("Done!");
                }

                var consumer = new QueueingBasicConsumer(mod);
                mod.BasicConsume(_queueName, false, consumer);
                Console.WriteLine("Waiting for messages...");
                while (true)
                {
                    var e = (BasicDeliverEventArgs)consumer.Queue.Dequeue();
                    mod.BasicAck(e.DeliveryTag, false);
                    IBasicProperties props = e.BasicProperties;
                    var handler = _cfg.Where(c => c.RoutingKey.Equals(e.RoutingKey, StringComparison.InvariantCultureIgnoreCase));
                    if (!handler.Any())
                    {
                        Console.WriteLine("Unknown type: {0}", e.RoutingKey);
                        continue;
                    }
                    byte[] body = e.Body;
                    Console.WriteLine("Received message: " + Encoding.UTF8.GetString(body));
                    var obj = MessageBase.FromJson(body, handler.First().MessageType);
                    handler
                        .ToList()
                        .ForEach(h =>
                        {
                            var response = h.Handle(obj);

                            var replyTo = e.BasicProperties.ReplyTo;
                            var correlationId = e.BasicProperties.CorrelationId;
                            if (
                                !string.IsNullOrEmpty(replyTo) && !string.IsNullOrEmpty(correlationId))
                            {
                                _pub.Response(response, h.ResponseType, e);
                            }
                        });
                }
            }
        }
    }
}
