using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Orders.Messages.Commands;
using Orders.Messages.Events;
using RabbitInfrastructure;

namespace Orders.Service
{
    public class PlaceOrderCommandHandler : MessageHandlerBase<PlaceOrderCommand>
    {
        static int orderCounter = 0;
        public override object Handle(PlaceOrderCommand message)
        {
            Console.Write("Processing order {0} from customer {1}... ", message.OrderReference, message.CustomerId);
            // DO STUFF
            Thread.Sleep(TimeSpan.FromSeconds(3));
            publisher.Publish(new OrderPlacedEvent()
            {
                Amount = 10,
                CustomerId = message.CustomerId,
                OrderId = orderCounter++,
                ShippingAddress = "Address of customer " + message.CustomerId
            });
            Console.WriteLine("Done!");
            return null;
        }
    }
}
