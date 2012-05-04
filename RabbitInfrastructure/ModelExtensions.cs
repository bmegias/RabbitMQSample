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
    }
}
