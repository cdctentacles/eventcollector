using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using CDC.EventCollector;

namespace eventcollector.tests
{
    public class TestPersistentCollectorFailure : TestPersistentCollector
    {
        public TestPersistentCollectorFailure()
        {
            this.acceptChange = true;
        }

        override public Task PersistTransactions(PartitionChange change)
        {
            if (this.acceptChange)
            {
                return base.PersistTransactions(change);
            }
            return Task.FromException(new Exception("Network error"));
        }

        public void AcceptChangesNow()
        {
            this.acceptChange = true;
        }

        public void DontAcceptChangesNow()
        {
            this.acceptChange = false;
        }

        bool acceptChange;
    }
}