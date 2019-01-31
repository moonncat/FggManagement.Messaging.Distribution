using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace FggManagement.Messaging.Distribution
{
    public class TransactionManagerBase
    {
        public IMessageManager MessageManager { get; set; }

        protected string _copy;
        protected string _sessionID;
        protected object _entity;
        protected string _newID => Guid.NewGuid().ToString();

        public void BeginTransaction(object _entity)
        {
            this._sessionID = _newID;
            if (_entity == null)
            {
                throw new ArgumentNullException("_entity");
            }
            this._copy = JsonConvert.SerializeObject(_entity);
            this._entity = _entity;
        }

    }
}
