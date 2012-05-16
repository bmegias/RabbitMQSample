using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitInfrastructure;
using Server.Messages;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("***** RPC Client *****");
            var tot = 0;
            var pub = new RabbitPublisher();
            var serviceName = typeof(Program).Namespace.ToLowerInvariant();

            var cfg = new[] { new HandlerConfig<HelloResponse>(null) };
            var sub = new RabbitSubscriber(RabbitCfg.XCHG, serviceName, RabbitCfg.HOST, cfg);

            sub.MessageReceived += (sender, msg) =>
                {
                    Console.WriteLine("Received: {0}", MessageBase.ToJson(msg.Message));
                };

            sub.StartAsyncMessageLoop();

            while (true)
            {
                var name = string.Empty;
                while (string.IsNullOrWhiteSpace(name))
                {
                    Console.WriteLine("What's your name?");
                    name = Console.ReadLine();
                }
                var msg = new HelloRequest() { Name = name };
                pub.PublishRequest<HelloRequest, HelloResponse>(msg);
                Console.WriteLine("Request sent: {0}",msg.ToJson());
            }
            //while (true)
            //{
            //    Console.WriteLine("Press enter to send 10 requests");
            //    Console.ReadLine();
            //    for (var i = tot; i < 10; i++, tot++)
            //    {
            //        pub.PublishRequest(new HelloRequest() { Name = "Name" + i.ToString("000") });
            //    }
            //    Console.WriteLine("10 requests sent");
            //}
        }
    }
}
