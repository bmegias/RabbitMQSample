using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitInfrastructure
{
    public interface IMessageHandler<T> where T : MessageBase
    {
        void Handle(T message);
    }
    public abstract class MessageHandlerBase<T> : IMessageHandler<T> where T : MessageBase
    {
        protected RabbitPublisher publisher = new RabbitPublisher();
        public abstract void Handle(T message);
    }
}
