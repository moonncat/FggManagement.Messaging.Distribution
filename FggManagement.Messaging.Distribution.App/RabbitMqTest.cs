using FggManagement.Messaging.Distribution.RabbitMq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FggManagement.Messaging.Distribution.App
{
    class RabbitMqTest
    {
        public Task Subscribe()
        {
            return Task.Run(() =>
            {
                var client = new RabbitMqMessageManager("amqp://guest:guest@localhost:5672/");
                client.Subscribe("queue", (s) =>
                {
                    Console.WriteLine("Consumer queue recieved: " + s);

                });

                Thread.Sleep(Timeout.Infinite);
            });
        }
        public Task Publish()
        {
            var client = new RabbitMqMessageManager("amqp://guest:guest@localhost:5672/");
            return Task.Run(() =>
            {
                for(int i = 0; i < 10; i++)
                {
                    client.Publish("exchange", "routingkey", $"message{i}");
                    Console.WriteLine("Published: " + i);
                }
            });
        }
    }
}
