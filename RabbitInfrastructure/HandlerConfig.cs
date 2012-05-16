using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitInfrastructure
{
    public class HandlerConfig<T> : IHandlerConfig 
        where T : MessageBase
    {
        public string RoutingKey { get; set; }
        public string Exchange { get; set; }
        public Type MessageType { get { return typeof(T); } }
        public IMessageHandler<T> Handler { get; set; }

        public virtual object Handle(object msg)
        {
            return this.Handler != null ? this.Handler.Handle(msg as T) : null;
        }

        public HandlerConfig(IMessageHandler<T> handler)
        {
            RoutingKey = typeof(T).FullName.ToLowerInvariant();
            Exchange = RabbitCfg.XCHG;
            Handler = handler;
        }
    }

    public class RPCHandlerConfig<T, TResponse> : IRPCHandlerConfig
        where T : MessageBase
        where TResponse : MessageBase
    {
        public string RoutingKey { get; set; }
        public string Exchange { get; set; }
        public Type MessageType { get { return typeof(T); } }
        public IMessageHandler<T> Handler { get; set; }
        public Type ResponseType { get { return typeof(TResponse); } }

        public RPCHandlerConfig(IMessageHandler<T> handler)
        {
            RoutingKey = typeof(T).FullName.ToLowerInvariant();
            Exchange = RabbitCfg.XCHG;
            Handler = handler;
        }

        public virtual object Handle(object msg)
        {
            return this.Handler != null ? this.Handler.Handle(msg as T) : null;
        }

    }
}
