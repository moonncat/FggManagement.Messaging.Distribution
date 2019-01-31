using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FggManagement.Messaging.Distribution.RabbitMq
{
    public class RabbitMqTransactionManager : TransactionManagerBase, ITransactionManager
    {


        public RabbitMqTransactionManager(string host, string queue)
        {
            this.Queue = queue;
            this.MessageManager = new RabbitMqMessageManager(host);
        }

        public string Queue { get; set; }

        public void Submit()
        {
            if (this._entity != null)
            {
                this.MessageManager.Publish(this.Queue, JsonConvert.SerializeObject(this._entity));
                this._entity = null;
            }
        }

        public void Rollback()
        {
            if (!string.IsNullOrEmpty(this._copy))
            {
                this.MessageManager.Publish(this.Queue, this._copy);
                this._copy = null;
            }
        }
    }
}
