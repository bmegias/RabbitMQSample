using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using RabbitInfrastructure;

namespace Orders.Messages.Events
{
    public class OrderPlacedEvent:MessageBase
    {
        [DataMember]
        public int CustomerId { get; set; }
        [DataMember]
        public int OrderId { get; set; }
        [DataMember]
        public double Amount { get; set; }
        [DataMember]
        public string ShippingAddress { get; set; }
    }
}
