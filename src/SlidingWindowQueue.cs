using System.Collections.Generic;

namespace CDC.EventCollector
{
    internal class SlidingWindowQueue
    {
        public SlidingWindowQueue()
        {
            this.queue = new List<TransactionData>();
            this.lsnSeen = 0;
        }

        public void Add(long lsn, byte [] data)
        {
            this.queue.Add(new TransactionData(lsn, data));
        }

        public int Length()
        {
            return this.queue.Count;
        }

        public void SlidWindowTill(long lsn)
        {
            this.lsnSeen = lsn;
        }

        public List<TransactionData> GetTransactions(long lsn)
        {
            return new List<TransactionData>(this.queue);
        }

        private long lsnSeen;
        List<TransactionData> queue;
    }
}