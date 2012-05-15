using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace RabbitInfrastructure
{
    [DataContract]
    public abstract class MessageBase
    {
        public static string ToJson(object obj,Type t)
        {
            using (var stream1 = new MemoryStream())
            {
                var ser = new DataContractJsonSerializer(t);
                ser.WriteObject(stream1, obj);
                stream1.Position = 0;
                var sr = new StreamReader(stream1);
                return sr.ReadToEnd();
            }
        }

        public string ToJson()
        {
            return ToJson(this,this.GetType());
        }

        public static object FromJson(byte[] str, Type type) 
        {
            using (var stream1 = new MemoryStream(str))
            {
                var ser = new DataContractJsonSerializer(type);
                return ser.ReadObject(stream1);
            } 
        }
    }
}
