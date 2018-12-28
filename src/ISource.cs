using System;

namespace CDC.EventCollector
{
    public interface ISource
    {
        ITransactionalLog GetTransactionalLog();
        Guid GetSourceId();
    }
}
