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

    public class PartitionChange
    {
        public readonly Guid PartitionId;
        public readonly List<Tuple<long, byte[]>> Transactions;

        PartitionChange(Guid partitionId, List<Tuple<long, byte[]>> transactions)
        {
            this.PartitionId = partitionId;
            this.Transactions = transactions;
        }
    }
}