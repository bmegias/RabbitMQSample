using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using RabbitInfrastructure;

namespace Billing.Messages.Events
{
    public class OrderBilledEvent : MessageBase
    {
        [DataMember]
        public int OrderId { get; set; }
    }
}
