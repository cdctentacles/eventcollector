namespace CDC.EventCollector
{
    public interface IEventCollector
    {
        void TransactionApplied(long lsn, byte [] transaction);
    }
}