using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitInfrastructure
{
    public class HandlerConfig<T> : IHandlerConfig where T : MessageBase
    {
        public string RoutingKey { get; set; }
        public string Exchange { get; set; }
        public Type MessageType { get { return typeof(T); } }
        public Type ResponseType { get; set; }
        public IMessageHandler<T> Handler { get; set; }

        public object Handle(object msg)
        {
            return this.Handler.Handle((T)msg);
        }

        public HandlerConfig(IMessageHandler<T> handler)
        {
            RoutingKey = typeof(T).FullName.ToLowerInvariant();
            Exchange = RabbitCfg.XCHG;
            Handler = handler;
        }
    }
    
}
