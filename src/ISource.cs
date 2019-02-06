using System;

namespace CDC.EventCollector
{
    public interface ISource
    {
        Guid GetSourceId();
    }
}
