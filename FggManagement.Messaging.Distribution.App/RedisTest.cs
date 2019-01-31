using FggManagement.Messaging.Distribution.Redis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FggManagement.Messaging.Distribution.App
{
    class RedisTest
    {
        public Task Subscribe(string host)
        {
            return Task.Run(() =>
            {
                var client = new RedisMessageManager(host);
                client.Subscribe("redis-test", (s) =>
                {
                    string message = client.GetMessage(s);
                    Console.WriteLine($"Consumer {s} recieved: " + message);
                });


                Thread.Sleep(Timeout.Infinite);
            });
        }
        public Task Publish(string host)
        {
            return Task.Run(() =>
            {
                var a = new Model();

                for (int i = 0; i < 9; i++)
                {
                    a.Name = "aaa" + i;
                    var publisher = new RedisTransactionManager(host, "redis-test");
                    publisher.BeginTransaction(a);
                    publisher.Submit();
                }
                for (int i = 0; i < 9; i++)
                {
                    var client = new RedisTransactionManager(host, "redis-test");
                    a.Name = "aaa" + i;
                    client.BeginTransaction(new[] { a });
                    a.Name = "rollback" + i;
                    client.Submit();
                    client.Rollback();
                }
            });
        }
    }
}
