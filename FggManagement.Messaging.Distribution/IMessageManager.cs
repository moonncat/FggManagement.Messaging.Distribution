using System;
using System.Threading.Tasks;

namespace FggManagement.Messaging.Distribution
{
    public interface IMessageManager
    {
        int Publish(string exchange, string routingKey, string message);
        int Publish(string chanel, string message);
        Task Subscribe(string chanel, Action<string> handler);
        void SetMessage(string chanel,string message);
        string GetMessage(string chanel);
    }
}
