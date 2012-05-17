using System;
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
        RabbitSubscriber _sub;

        public RabbitPublisher()
            : this(RabbitCfg.XCHG, RabbitCfg.HOST, null)
        { }

        public RabbitPublisher(string xchg, string hostName, RabbitSubscriber subs) {
            _xchg = xchg;
            _hostName = hostName;
            _sub = subs;
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

        public string PublishRequest<T, Tresp>(T message, Action<Tresp> responseHandler)
            where T : MessageBase
            where Tresp : MessageBase
        {
            var corId = Guid.NewGuid().ToString();
            return PublishRequest(corId, message, responseHandler);
        }

        public string PublishRequest<T, Tresp>(string correlationId, T message, Action<Tresp> responseHandler)
            where T : MessageBase
            where Tresp : MessageBase
        {
            if (_sub == null)
                throw new ArgumentException("A subscriber must be provided to perform RPC calls");

            var factory = new ConnectionFactory()
            {
                Protocol = Protocols.FromEnvironment(),
                HostName = _hostName
            };

            var corrId = string.IsNullOrWhiteSpace(correlationId) ? Guid.NewGuid().ToString() : correlationId;

            _sub.SubscribeResponse(correlationId, responseHandler);

            using (var con = factory.CreateConnection())
            using (var mod = con.CreateModel())
            {
                mod.ExchangeDeclare(_xchg, ExchangeType.Direct, true);
                mod.PublishRequest<T, Tresp>(_xchg, message, corrId);
            }
            return corrId;
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
                mod.Response(_xchg, response, args.BasicProperties.ReplyTo, args.BasicProperties.CorrelationId);
            }
        }
    }
}
