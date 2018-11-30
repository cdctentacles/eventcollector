namespace CDC.EventCollector
{
    public interface IHealthStore
    {
        void WriteError();
        void WriteWarning();
        void WriteInfo();
        void WriteNoise();
    }
}