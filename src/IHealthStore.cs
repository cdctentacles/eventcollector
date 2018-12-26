using System;

namespace CDC.EventCollector
{
    public interface IHealthStore
    {
        void WriteError(string msg);
        void WriteWarning(string msg);
        void WriteInfo(string msg);
        void WriteNoise(string msg);
    }
}