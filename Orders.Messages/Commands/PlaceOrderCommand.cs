using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using RabbitInfrastructure;

namespace Orders.Messages.Commands
{
    public class PlaceOrderCommand : MessageBase
    {
        [DataMember]
        public int CustomerId { get; set; }
        [DataMember]
        public string OrderReference { get; set; }
    }
}
