using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Billing.Messages.Events;
using Orders.Messages.Commands;
using Orders.Messages.Events;
using RabbitInfrastructure;
using Shipping.Messages.Events;
using Web.Handlers;

namespace Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );

        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            // Use LocalDB for Entity Framework by default
            Database.DefaultConnectionFactory = new SqlConnectionFactory("Data Source=(localdb)\v11.0; Integrated Security=True; MultipleActiveResultSets=True");

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            // http://haacked.com/archive/2011/10/16/the-dangers-of-implementing-recurring-background-tasks-in-asp-net.aspx
            var wsHandler = new WSEventHandler();
            var signalrHandler = new SignalREventHandler();

            ThreadPool.QueueUserWorkItem(
                (o) =>
                {
                    RabbitSubscriber.MessageLoop("web",
                        new IHandlerConfig[]{
                            new HandlerConfig<OrderPlacedEvent>(wsHandler),
                            new HandlerConfig<OrderBilledEvent>(wsHandler),
                            new HandlerConfig<OrderShippedEvent>(wsHandler),

                            new HandlerConfig<OrderPlacedEvent>(signalrHandler),
                            new HandlerConfig<OrderBilledEvent>(signalrHandler),
                            new HandlerConfig<OrderShippedEvent>(signalrHandler),
                        });
                });
        }
    }
}
