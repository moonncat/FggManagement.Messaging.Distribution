using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace FggManagement.Messaging.Distribution.RabbitMq
{
    public class RabbitMqMessageManager : IMessageManager, IDisposable
    {
        public RabbitMqMessageManager(string host)
        {
            var factory = new ConnectionFactory();
            factory.Uri = new Uri(host);
            this.connection = factory.CreateConnection();
        }
        private readonly IConnection connection;
        public string Exchange { get; set; }
        public string GetMessage(string chanel)
        {
            var model = this.connection.CreateModel();
            var msg = model.BasicGet(chanel, false);
            string buff = Encoding.UTF8.GetString(msg.Body);
            model.BasicAck(msg.DeliveryTag, false);
            return buff;
        }

        public void SetMessage(string chanel, string message)
        {
            this.Publish(chanel, message);
        }

        public int Publish(string chanel, string message)
        {
            return this.Publish(this.Exchange, chanel, message);
        }

        public Task Subscribe(string chanel, Action<string> handler)
        {
            return Task.Run(() =>
            {
                while (true)
                {
                    try
                    {
                        var model = this.connection.CreateModel();
                        EventingBasicConsumer consumer = new EventingBasicConsumer(model);
                        consumer.Received += (ch, ea) =>
                         {
                             try
                             {
                                 string buff = Encoding.UTF8.GetString(ea.Body);
                                 handler?.Invoke(buff);
                                 model.BasicAck(ea.DeliveryTag, false);
                             }
                             catch
                             {
                                 model.BasicCancel(ea.ConsumerTag);
                             }
                         };
                        model.BasicConsume(chanel, false, consumer);
                        Thread.Sleep(Timeout.Infinite);
                    }
                    catch
                    {
                        Thread.Sleep(100);
                    }
                }
            });
        }

        public int Publish(string exchange, string routingKey, string message)
        {
            try
            {
                var model = this.connection.CreateModel();
                byte[] buff = Encoding.UTF8.GetBytes(message);
                IBasicProperties props = model.CreateBasicProperties();
                props.ContentType = "text/plain";
                props.DeliveryMode = 1;
                model.BasicPublish(exchange, routingKey, props, buff);
                return 0;
            }
            catch
            {
                return -1;
            }
        }

        public void Dispose()
        {
            if (this.connection != null && this.connection.IsOpen)
            {
                this.connection.Close();
            }
        }
    }
}
