using System.Collections.Generic;

namespace CDC.EventCollector
{
    class EventCollector
    {
        EventCollector()
        {
            this.queue = new SlidingWindowQueue();
            this.targets = new List<ITarget>();
        }

        public void TransactionApplied(long lsn, byte [] transaction)
        {
            queue.Add(lsn, transaction);
        }

        private SlidingWindowQueue queue;
        private List<ITarget> targets;
    }
}