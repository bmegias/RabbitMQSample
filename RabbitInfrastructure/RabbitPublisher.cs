﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitInfrastructure
{
    public class RabbitPublisher
    {
        string _xchg;
        string _hostName;
        public RabbitPublisher()
            : this(RabbitCfg.XCHG, RabbitCfg.HOST)
        { }

        public RabbitPublisher(string xchg, string hostName) {
            _xchg = xchg;
            _hostName = hostName;
        }

        public void Publish<T>(T message) where T:MessageBase 
        {
            var factory = new ConnectionFactory()
            {
                Protocol = Protocols.FromEnvironment(),
                HostName = _hostName
            };
            using (var con = factory.CreateConnection())
            using (var mod = con.CreateModel())
            {
                mod.ExchangeDeclare(_xchg, ExchangeType.Direct, true);
                mod.Publish(_xchg, message);
            }
        }

        public void Response(object response, Type responseType, BasicDeliverEventArgs args)
        {
            var factory = new ConnectionFactory()
            {
                Protocol = Protocols.FromEnvironment(),
                HostName = _hostName
            };
            using (var con = factory.CreateConnection())
            using (var mod = con.CreateModel())
            {
                mod.ExchangeDeclare(_xchg, ExchangeType.Direct, true);
                mod.Response(_xchg, response, responseType, args.BasicProperties.ReplyTo, args.BasicProperties.CorrelationId);
            }
        }
    }
}
