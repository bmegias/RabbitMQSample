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
            mod.BasicPublish(xchg, typeof(T).FullName.ToLowerInvariant(), null, Encoding.UTF8.GetBytes(message.ToJson()));
        }

        public static void Response(this IModel mod, string xchg, object message, Type msgType, string routingKey, string correlationId)
        {
            var props = mod.CreateBasicProperties();
            props.CorrelationId = correlationId;
            var json = MessageBase.ToJson(message, msgType);
            mod.BasicPublish(xchg, routingKey, props, Encoding.UTF8.GetBytes(json));
        }
    }
}
