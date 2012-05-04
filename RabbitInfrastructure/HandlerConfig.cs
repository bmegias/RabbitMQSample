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
        public IMessageHandler<T> Handler { get; set; }
        public void Handle(object msg)
        {
            this.Handler.Handle((T)msg);
        }

        public HandlerConfig(IMessageHandler<T> handler)
        {
            RoutingKey = typeof(T).FullName.ToLowerInvariant();
            Exchange = RabbitCfg.XCHG;
            Handler = handler;
        }
    }
    
}
