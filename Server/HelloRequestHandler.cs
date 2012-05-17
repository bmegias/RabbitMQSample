using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RabbitInfrastructure;
using Server.Messages;

namespace Server
{
    public class HelloRequestHandler : MessageHandlerBase<HelloRequest>
    {
        public override object Handle(HelloRequest message)
        {
            Console.WriteLine("Received: {0}", message.ToJson());
            Thread.Sleep(TimeSpan.FromSeconds(10));
            return new HelloResponse()
            {
                HelloMessage = string.Format("Hello {0}", message.Name)
            };
        }
    }
}
