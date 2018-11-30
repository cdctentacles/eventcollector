namespace CDC.EventCollector
{
    public abstract class Source : ISource
    {
        public Source(IEventCollector eventCollector, IHealthStore healthStore, ITransactionalLog transactionalLog)
        // : base(collector, healthStore, transactionalLog)
        {
            this.eventCollector = eventCollector;
            this.healthStore = healthStore;
            this.transactionalLog = transactionalLog;
        }

        // return error type
        public void OnTransactionApplied(long lsn, byte [] transaction)
        {
            this.eventCollector.TransactionApplied(lsn, transaction);
        }

        private IEventCollector eventCollector;
        private IHealthStore healthStore;
        private ITransactionalLog transactionalLog;
    }
}