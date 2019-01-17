using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CDC.EventCollector
{
    public class PartitionChange
    {
        public readonly Guid PartitionId;
        public readonly ReadOnlyCollection<TransactionData> Transactions;

        public PartitionChange(Guid partitionId, ReadOnlyCollection<TransactionData> transactions)
        {
            this.PartitionId = partitionId;
            this.Transactions = transactions;
        }
    }
}