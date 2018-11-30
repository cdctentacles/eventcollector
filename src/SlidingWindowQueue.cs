namespace CDC.EventCollector
{
    class SlidingWindowQueue
    {
        public SlidingWindowQueue()
        {
            this.lsnInWaiting = 0;
        }

        public void Add(long lsn, byte [] data)
        {
            if (lsn == this.lsnInWaiting)
            {
                this.lsnInWaiting += 1;
            }
        }

        public int Length()
        {
            return 0;
        }

        public void SlidWindowTill(long lsn)
        {

        }

        private long lsnInWaiting;
    }
}