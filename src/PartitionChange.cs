using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace CDC.EventCollector
{
    public class PartitionChange
    {
        public readonly Guid PartitionId;
        public readonly List<TransactionData> Transactions;

        public PartitionChange(Guid partitionId, List<TransactionData> transactions)
        {
            this.PartitionId = partitionId;
            this.Transactions = transactions;
        }
    }
}