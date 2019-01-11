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
            this.scheduler.Ready += EventCollector.PersistEvents;
        }

        public Task TransactionApplied(Guid partitionId, long lsn, byte [] transaction)
        {
            this.queue.Add(lsn, transaction);
            return this.scheduler.NewEvent();
        }

        static public async Task PersistEvents(object eventCollector)
        {
            await ((EventCollector)eventCollector).PersistEvents();
        }

        public async Task PersistEvents()
        {
            await Task.CompletedTask;
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
        private EventCollectorScheduler scheduler;
        private Object lockObj;
    }
}