using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitInfrastructure;
using Server.Messages;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("***** RPC Server *****");
            var serviceName = typeof(Program).Namespace.ToLowerInvariant();
            RabbitSubscriber.MessageLoop(serviceName, new[]
                    {
                    new RPCHandlerConfig<HelloRequest,HelloResponse>(new HelloRequestHandler())
                    }
                );
        }
    }
}
