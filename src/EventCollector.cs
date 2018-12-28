using System.Collections.Generic;

namespace CDC.EventCollector
{
    class EventCollector
    {
        public EventCollector()
        {
            this.queue = new SlidingWindowQueue();
            this.targets = new List<IPersistentCollector>();
        }

        public void TransactionApplied(long lsn, byte [] transaction)
        {
            queue.Add(lsn, transaction);
        }

        private SlidingWindowQueue queue;
        private List<IPersistentCollector> targets;
    }
}