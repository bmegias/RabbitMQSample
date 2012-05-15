using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Billing.Messages.Events;
using Orders.Messages.Commands;
using Orders.Messages.Events;
using RabbitInfrastructure;
using Shipping.Messages.Events;

namespace Web.Handlers
{
    public class WSEventHandler:MessageHandlerBase<OrderPlacedEvent>,IMessageHandler<OrderBilledEvent>,IMessageHandler<OrderShippedEvent>
    {
        public override object Handle(OrderPlacedEvent message)
        {
            return broadcastMessage(message);
        }

        public object Handle(OrderBilledEvent message)
        {
            return broadcastMessage(message);
        }

        public object Handle(OrderShippedEvent message)
        {
            return broadcastMessage(message);
        }
        object broadcastMessage(MessageBase msg) 
        {
            TestWebSocketHandler.Broadcast(msg);
            return null;
        }

    }
}
