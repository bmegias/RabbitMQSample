using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using RabbitInfrastructure;
using Server.Messages;

namespace AsyncWeb
{
    public static class RabbitUtils
    {
        static RabbitPublisher _publisher;
        static RabbitSubscriber _subscriber;
        public static RabbitPublisher Publisher { get { return _publisher; } }
        public static RabbitSubscriber Subscriber { get { return _subscriber; } }

        static RabbitUtils()
        {
            var serviceName = typeof(RabbitUtils).FullName;
            var cfg = new[] { new HandlerConfig<HelloResponse>(null) };
            _subscriber = new RabbitSubscriber(RabbitCfg.XCHG, serviceName, RabbitCfg.HOST, cfg);
            _publisher = new RabbitPublisher(RabbitCfg.XCHG, RabbitCfg.HOST, _subscriber);
        }
    }


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
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            BundleTable.Bundles.RegisterTemplateBundles();

            RabbitUtils.Subscriber.StartAsyncMessageLoop();
        }
    }
}
