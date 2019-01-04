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
        }

        public Task PersistTransactions(List<PartitionChange> changes)
        {
            return Task.CompletedTask;
        }

        public Guid GetId()
        {
            return this.Id;
        }

        Guid Id;
    }
}