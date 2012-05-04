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
        public override void Handle(OrderPlacedEvent message)
        {
            broadcastMessage(message);
        }

        public void Handle(OrderBilledEvent message)
        {
            broadcastMessage(message);
        }

        public void Handle(OrderShippedEvent message)
        {
            broadcastMessage(message);
        }
        void broadcastMessage(MessageBase msg) 
        {
            TestWebSocketHandler.Broadcast(msg);
        }

    }
}
