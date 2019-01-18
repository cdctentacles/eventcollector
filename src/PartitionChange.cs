using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CDC.EventCollector
{
    public class PartitionChange
    {
        public readonly Guid PartitionId;
        public readonly IReadOnlyList<TransactionData> Transactions;

        public PartitionChange(Guid partitionId, IReadOnlyList<TransactionData> transactions)
        {
            this.PartitionId = partitionId;
            this.Transactions = transactions;
        }
    }
}