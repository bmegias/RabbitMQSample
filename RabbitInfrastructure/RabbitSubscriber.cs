using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitInfrastructure
{
    public class MessageReceivedEventArgs : EventArgs
    {
        public MessageBase Message { get; set; }
        public string CorrelationId { get; set; }
        public string ReplyTo { get; set; }
    }

    public class RabbitSubscriber
    {
        interface IResponseHandler
        {
            void Handle(object msg);
        }

        class ResponseHandler<T>:IResponseHandler
            where T : MessageBase
        {
            public Action<T> Handler { get; set; }

            public virtual void Handle(object msg)
            {
                this.Handler(msg as T);
            }

            public ResponseHandler(Action<T> handler)
            {
                Handler = handler;
            }
        }


        Dictionary<string, IResponseHandler> _responseHandlers;
        object _syncResponseHandlers;

        string _exchangeName;
        string _queueName;
        string _hostName;
        IEnumerable<IHandlerConfig> _cfg;

        RabbitPublisher _pub;

        public delegate void MessageReceivedHandler(object sender, MessageReceivedEventArgs e);
        public event MessageReceivedHandler MessageReceived;

        protected virtual void RaiseMessageReceived(MessageReceivedEventArgs args)
        {
            if (MessageReceived != null)
            {
                MessageReceived(this, args);
            }
        }

        public RabbitSubscriber(string exchangeName, string queueName, string hostName, IEnumerable<IHandlerConfig> cfg) 
        {
            _responseHandlers = new Dictionary<string, IResponseHandler>();
            _syncResponseHandlers = new object();

            _exchangeName=exchangeName;
            _queueName = queueName;
            _hostName = hostName;
            _cfg = cfg;
            _pub = new RabbitPublisher(_exchangeName, hostName, this);
        }

        public static void MessageLoop(string serviceName, IEnumerable<IHandlerConfig> cfg)
        {
            Console.WriteLine("*** {0} ***", serviceName);
            var subscriber = new RabbitSubscriber(RabbitCfg.XCHG, serviceName, RabbitCfg.HOST, cfg);
            subscriber.MessageLoop();
        }

        public void MessageLoop() 
        {
            var factory = new ConnectionFactory()
            {
                Protocol = Protocols.FromEnvironment(),
                HostName = _hostName
            };
            using (var con = factory.CreateConnection())
            using (var mod = con.CreateModel())
            {
                mod.QueueDeclare(_queueName, true, false, false, null);
                foreach (var h in _cfg)
                {
                    var xchg = _exchangeName.ToLowerInvariant();
                    var rk = h.MessageType.FullName.ToLowerInvariant();
                    Console.Write("Binding {0}:{1}:{2}... ", xchg, rk, _queueName);
                    mod.ExchangeDeclare(xchg, ExchangeType.Direct, true);
                    mod.QueueBind(_queueName, xchg, rk);
                    Console.WriteLine("Done!");
                }

                var consumer = new QueueingBasicConsumer(mod);
                mod.BasicConsume(_queueName, false, consumer);
                Console.WriteLine("Waiting for messages...");
                while (true)
                {
                    var e = (BasicDeliverEventArgs)consumer.Queue.Dequeue();
                    mod.BasicAck(e.DeliveryTag, false);

                    var tsk = Task.Run(
                        () =>
                        {
                            var props = e.BasicProperties;
                            var handler = _cfg.Where(c => c.RoutingKey.Equals(e.RoutingKey, StringComparison.InvariantCultureIgnoreCase));

                            byte[] body = e.Body;

                            Console.WriteLine("Received message: " + Encoding.UTF8.GetString(body));
                            var obj = MessageBase.FromJson(body, handler.First().MessageType);
                            var correlationId = e.BasicProperties.CorrelationId;
                            var replyTo = e.BasicProperties.ReplyTo;

                            var evtArgs = new MessageReceivedEventArgs()
                            {
                                Message = obj as MessageBase,
                                CorrelationId = correlationId,
                                ReplyTo = replyTo
                            };

                            RaiseMessageReceived(evtArgs);

                            handler
                                .AsParallel()
                                .ForAll(h =>
                                {
                                    var response = h.Handle(obj);
                                    var hRPC = h as IRPCHandlerConfig;
                                    var mustReply =
                                        hRPC != null
                                        && !string.IsNullOrEmpty(replyTo)
                                        && !string.IsNullOrEmpty(correlationId);
                                    var isAReply = !string.IsNullOrEmpty(correlationId);
                                    if (mustReply)
                                    {
                                        Console.WriteLine(
                                            "Replying RK: {0} CorrelationId: {1} Response: {2}"
                                            , e.BasicProperties.ReplyTo
                                            , e.BasicProperties.CorrelationId
                                            , MessageBase.ToJson(response));
                                        _pub.Response(response, hRPC.ResponseType, e);
                                    }
                                    if (isAReply)
                                    {
                                        var han = getHandler(correlationId);
                                        if (han != null)
                                        {
                                            han.Handle(obj);
                                            UnSubscribeResponse(correlationId);
                                        }
                                    }
                                });
                        });
                }
            }
        }

        public void StartAsyncMessageLoop()
        {
            Task.Run(() => this.MessageLoop());
        }

        IResponseHandler getHandler(string corId)
        {
            IResponseHandler handler = null;
            _responseHandlers.TryGetValue(corId, out handler);
            return handler;
        }

        public void SubscribeResponse<T>(string corId, Action<T> action) where T : MessageBase
        {
            lock (_syncResponseHandlers)
            {
                _responseHandlers.Add(corId, new ResponseHandler<T>(action));
            }
        }

        void UnSubscribeResponse(string corId)
        {
            lock (_syncResponseHandlers)
            {
                _responseHandlers.Remove(corId);
            }
        }
    }
}
