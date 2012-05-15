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
        //string Exchange { get; }
        Type MessageType { get; }
        Type ResponseType { get; }
        object Handle(object msg);
    }
}
