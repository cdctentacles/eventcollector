using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CDC.EventCollector
{
    class EventCollector : IEventCollector
    {
        public EventCollector()
        {
            this.queue = new SlidingWindowQueue();
            this.persistentCollector = new List<IPersistentCollector>();
            this.lockObj = new Object();
            this.scheduler = new EventCollectorScheduler();
        }

        public Task TransactionApplied(Guid partitionId, long lsn, byte [] transaction)
        {
            queue.Add(lsn, transaction);
            return Task.CompletedTask; // todo
        }

        public void AddPersistentCollectors(IList<IPersistentCollector> newCollectors)
        {
            lock(lockObj)
            {
                var unregisteredCollectors = newCollectors.Where(nc => !IsRegisteredCollector(nc));
                foreach (var collector in unregisteredCollectors)
                {
                    this.persistentCollector.Add(collector);
                }
            }
        }

        private bool IsRegisteredCollector(IPersistentCollector collector)
        {
            return this.persistentCollector.Any(c => c.GetId() == collector.GetId());
        }

        private SlidingWindowQueue queue;
        private IList<IPersistentCollector> persistentCollector;
        private IEventCollectorScheduler scheduler;
        private Object lockObj;
    }
}