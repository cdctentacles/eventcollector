using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CDC.EventCollector
{
    public interface IPersistentCollector
    {
        Task PersistTransactions(PartitionChange change);
        Guid GetId();
    }
}