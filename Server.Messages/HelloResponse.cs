using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using RabbitInfrastructure;

namespace Server.Messages
{
    public class HelloResponse : MessageBase
    {
        [DataMember]
        public string HelloMessage { get; set; }
    }
}
