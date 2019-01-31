using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FggManagement.Messaging.Distribution
{
    public interface ITransactionManager
    {
        IMessageManager MessageManager { get; set; }
        void BeginTransaction(object entity);
        void Rollback();
        void Submit();
    }
}
