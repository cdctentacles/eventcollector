using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace CDC.EventCollector
{
    public interface IPersistentCollector
    {
        Task PersistTransactions(List<PartitionChange> changes);
        Guid GetId();
    }
}