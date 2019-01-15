using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using CDC.EventCollector;

namespace eventcollector.tests
{
    public class TestPersistentCollector : IPersistentCollector
    {
        public TestPersistentCollector()
        {
            this.Id = new Guid();
            this.changes = new List<PartitionChange>();
        }

        public Task PersistTransactions(List<PartitionChange> changes)
        {
            this.changes.AddRange(changes);
            return Task.CompletedTask;
        }

        public Guid GetId()
        {
            return this.Id;
        }

        public List<PartitionChange> Changes
        {
            get { return new List<PartitionChange>(this.changes); }
        }

        List<PartitionChange> changes;
        Guid Id;
    }
}