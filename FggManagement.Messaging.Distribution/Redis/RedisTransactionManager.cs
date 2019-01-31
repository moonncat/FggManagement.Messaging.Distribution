using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FggManagement.Messaging.Distribution.Redis
{
    public class RedisTransactionManager : TransactionManagerBase, ITransactionManager
    {

        public RedisTransactionManager(string host, string queue)
        {
            this.Queue = queue;
            this.MessageManager = new RedisMessageManager(host);
        }

        public string Queue { get; set; }

        public void Submit()
        {
            if (this._entity != null)
            {
                this.MessageManager.SetMessage(this._sessionID, JsonConvert.SerializeObject(this._entity));

                this.MessageManager.Publish(this.Queue, this._sessionID);
                this._entity = null;
            }
        }

        public void Rollback()
        {
            if (!string.IsNullOrEmpty(this._copy))
            {
                this._sessionID = _newID;
                this.MessageManager.SetMessage(this._sessionID, this._copy);

                this.MessageManager.Publish(this.Queue, this._sessionID);
                this._copy = null;
            }
        }
    }
}
