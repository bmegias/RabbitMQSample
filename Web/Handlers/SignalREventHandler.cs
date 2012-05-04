using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Billing.Messages.Events;
using Orders.Messages.Commands;
using Orders.Messages.Events;
using RabbitInfrastructure;
using Shipping.Messages.Events;
using SignalR;
using SignalR.Hosting.AspNet;
using SignalR.Infrastructure;
using Web.Hubs;

namespace Web.Handlers
{
    public class SignalREventHandler : MessageHandlerBase<OrderPlacedEvent>, IMessageHandler<OrderBilledEvent>, IMessageHandler<OrderShippedEvent>
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
            var conMgr = AspNetHost.DependencyResolver.Resolve<IConnectionManager>();
            var clients = conMgr.GetClients<Notifications>();
            clients.addMessage(string.Format("{0} : {1}", msg.GetType().FullName, msg.ToJson()));
        }

    }
}