using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RabbitInfrastructure;
using Server.Messages;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {

            var names = new[] 
            { 
                "Brau", "Raul", "Oriol", "Xavi", "Rocio", "Juanjo" 
            };

            Console.WriteLine("***** RPC Client *****");
            var tot = 0;
            var serviceName = typeof(Program).Namespace.ToLowerInvariant();

            var cfg = new[] { new HandlerConfig<HelloResponse>(null) };
            var sub = new RabbitSubscriber(RabbitCfg.XCHG, serviceName, RabbitCfg.HOST, cfg);
            var pub = new RabbitPublisher(RabbitCfg.XCHG, RabbitCfg.HOST, sub);

            sub.StartAsyncMessageLoop();

            names
                .AsParallel()
                .ForAll(name =>
            {
                var corId = Guid.NewGuid().ToString();

                pub.PublishRequest<HelloRequest, HelloResponse>(
                    corId
                    , new HelloRequest() { Name = name }
                    , response =>
                    {
                        Console.WriteLine("Received CorrelationId: {0} Message: {1}"
                                               , corId
                                               , MessageBase.ToJson(response));
                    });
            });
            
        }
    }
}
