using System.Collections.Generic;

namespace CDC.EventCollector
{
    public static class CDCCollector
    {
        public static void AddConfiguration(Configuration configuration)
        {
            foreach (var sourceFactory in configuration.SourceFactories)
            {

            }
        }

        static EventCollector collector = new EventCollector();
    }
}