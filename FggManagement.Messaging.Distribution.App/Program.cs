using FggManagement.Messaging.Distribution.Redis;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FggManagement.Messaging.Distribution.App
{
    class Program
    {
        static void Main(string[] args)
        {
            string host = ("localhost:6379");
            var redis = new RedisTest();
            redis.Subscribe(host);
            redis.Publish(host);

            //var rabbit = new RabbitMqTest();
            //rabbit.Subscribe();
            //rabbit.Publish();

            Thread.Sleep(Timeout.Infinite);
        }

    }
}
