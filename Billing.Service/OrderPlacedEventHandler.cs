using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Billing.Messages.Events;
using Orders.Messages.Events;
using RabbitInfrastructure;

namespace Billing.Service
{
    class OrderPlacedEventHandler:MessageHandlerBase<OrderPlacedEvent>
    {
        public override void Handle(OrderPlacedEvent message)
        {
            Console.Write("Billing order {0}... ", message.OrderId);
            // DO STUFF
            Thread.Sleep(TimeSpan.FromSeconds(5));
            publisher.Publish(new OrderBilledEvent()
            {
                OrderId = message.OrderId
            });
            Console.WriteLine("Done!");
        }
    }
}
