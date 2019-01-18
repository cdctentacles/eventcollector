using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CDC.EventCollector
{
    internal class EventCollector : IEventCollector
    {
        public EventCollector(List<IPersistentCollector> persistentCollectors)
        {
            this.queue = new SlidingWindowQueue();
            this.persistentCollectors = persistentCollectors;
            this.lockObj = new Object();
            this.scheduler = new EventCollectorScheduler();
            this.scheduler.Ready += this.PersistEvents;
        }

        public Task TransactionApplied(Guid partitionId, long lsn, byte [] transaction)
        {
            this.queue.Add(lsn, transaction);
            return this.scheduler.NewEvent(lsn);
        }

        public async Task PersistEvents(long persistTillLsn)
        {
            var transactions = this.queue.GetTransactions(persistTillLsn);
            var partitionChange = new PartitionChange(new Guid(), transactions);

            foreach (var persistentCollector in this.persistentCollectors)
            {
                await persistentCollector.PersistTransactions(partitionChange);
            }

            this.queue.SlideWindowTill(persistTillLsn);
        }

        private int AddPersistentCollectors(IList<IPersistentCollector> newCollectors)
        {
            var unregisteredCollectors = newCollectors.Where(nc => !IsRegisteredCollector(nc));
            foreach (var collector in unregisteredCollectors)
            {
                this.persistentCollectors.Add(collector);
            }
            return unregisteredCollectors.Count();
        }

        internal bool IsRegisteredCollector(IPersistentCollector collector)
        {
            return this.persistentCollectors.Any(c => c.GetId() == collector.GetId());
        }

        private SlidingWindowQueue queue;
        private IList<IPersistentCollector> persistentCollectors;
        private EventCollectorScheduler scheduler;
        private Object lockObj;
    }
}