using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Orders.Messages.Commands;
using RabbitInfrastructure;
using RabbitMQ.Client;

namespace PublishTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var tot = 0;
            var pub = new RabbitInfrastructure.RabbitPublisher();
            while (true)
            {
                Console.WriteLine("Press enter to place 10 orders");
                Console.ReadLine();
                for (var i = tot; i < 10; i++, tot++)
                {
                    pub.Publish(new PlaceOrderCommand() { CustomerId = tot, OrderReference = Guid.NewGuid().ToString() });
                }
                Console.WriteLine("10 orders were placed");
            }
        }
    }
}
