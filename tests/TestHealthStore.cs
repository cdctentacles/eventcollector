using System;

using CDC.EventCollector;

namespace eventcollector.tests
{
    public class TestHealthStore : IHealthStore
    {
        public void WriteError(string msg)
        {
            ToConsole(msg);
        }

        public void WriteWarning(string msg)
        {
            ToConsole(msg);
        }

        public void WriteInfo(string msg)
        {
            ToConsole(msg);
        }

        public void WriteNoise(string msg)
        {
            ToConsole(msg);
        }

        void ToConsole(string msg)
        {
            Console.WriteLine($"TestHealthStore: {msg}");
        }
    }
}
