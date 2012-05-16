using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace RabbitInfrastructure
{
    public static class ModelExtensions
    {
        public static void Publish<T>(this IModel mod, string xchg, T message) where T : MessageBase
        {
            var rk = typeof(T).FullName.ToLowerInvariant();
            var json = message.ToJson();
            mod.BasicPublish(xchg, rk, null, Encoding.UTF8.GetBytes(json));
        }

        public static void PublishRequest<T,TResp>(this IModel mod, string xchg, T message, string correlationId) 
            where T : MessageBase
            where TResp : MessageBase
        {
            var json = message.ToJson();
            var rk = typeof(T).FullName.ToLowerInvariant();
            var rkReply = typeof(TResp).FullName.ToLowerInvariant();
            var props = mod.CreateBasicProperties();
            props.CorrelationId = correlationId;
            props.ReplyTo = rkReply;

            mod.BasicPublish(xchg, rk, props, Encoding.UTF8.GetBytes(message.ToJson()));
        }

        public static void Response(this IModel mod, string xchg, object message, string routingKey, string correlationId)
        {
            var json = MessageBase.ToJson(message);
            var props = mod.CreateBasicProperties();
            props.CorrelationId = correlationId;
            mod.BasicPublish(xchg, routingKey, props, Encoding.UTF8.GetBytes(json));
        }
    }
}
