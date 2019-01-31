using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FggManagement.Messaging.Distribution.Redis
{
    public class RedisMessageManager : IMessageManager
    {
        public RedisMessageManager(string host)
        {
            this.connection = ConnectionMultiplexer.Connect(host);
        }

        private readonly ConnectionMultiplexer connection;
        public int Expiry = 120;

        public Task Subscribe(string chanel, Action<string> handler)
        {
            return Task.Run(() =>
            {
                while (true)
                {
                    try
                    {
                        var subscriber = connection.GetSubscriber();
                        var msgQueue = subscriber.Subscribe(new RedisChannel(chanel, RedisChannel.PatternMode.Pattern));
                        msgQueue.OnMessage((cm) => handler?.Invoke(cm.Message.ToString()));
                        Thread.Sleep(Timeout.Infinite);
                    }
                    catch
                    {
                        Thread.Sleep(100);
                    }
                }
            });
        }

        public int Publish(string chanel, string message)
        {
            try
            {
                var db = connection.GetDatabase();
                db.Publish(new RedisChannel(chanel, RedisChannel.PatternMode.Pattern), message);
                return 0;
            }
            catch
            {
                return -1;
            }
        }

        public void SetMessage(string chanel, string message)
        {
            connection.GetDatabase().StringSet(chanel, message, new TimeSpan(0, 0, Expiry));
        }

        public string GetMessage(string chanel)
        {
            return connection.GetDatabase().StringGet(chanel).ToString();
        }

        public int Publish(string exchange, string routingKey, string message)
        {
            return this.Publish(routingKey, message);
        }

    }
}
