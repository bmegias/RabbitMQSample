using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitInfrastructure
{
    public interface IHandlerConfig
    {
        string RoutingKey { get; }
        Type MessageType { get; }
        object Handle(object msg);
    }

    public interface IRPCHandlerConfig : IHandlerConfig
    {
        Type ResponseType { get; }
    }
}
