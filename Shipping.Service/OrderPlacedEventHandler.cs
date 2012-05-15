using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Orders.Messages.Events;
using RabbitInfrastructure;
using Shipping.Messages.Events;

namespace Shipping.Service
{
    class OrderPlacedEventHandler:MessageHandlerBase<OrderPlacedEvent>
    {
        public override object Handle(OrderPlacedEvent message)
        {
            Console.Write("Shipping order {0}... ", message.OrderId);
            // DO STUFF
            Thread.Sleep(TimeSpan.FromSeconds(2));
            publisher.Publish(new OrderShippedEvent()
            {
                OrderId = message.OrderId
            });
            Console.WriteLine("Done!");
            return null;
        }
    }
}
