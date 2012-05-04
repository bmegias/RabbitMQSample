using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orders.Messages.Events;
using RabbitInfrastructure;

namespace Shipping.Service
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceName = typeof(Program).Namespace.ToLowerInvariant();
            RabbitSubscriber.MessageLoop(
                    serviceName, new IHandlerConfig[]
                    {
                    new HandlerConfig<OrderPlacedEvent>(new OrderPlacedEventHandler())
                    }
                );
        }
    }
}
