using System.Collections.Generic;

namespace CDC.EventCollector
{
    public class TransactionData
    {
        public TransactionData(long previousLsn, long lsn, byte[] data)
        {
            this.previousLsn = previousLsn;
            this.lsn = lsn;
            this.data = data;
        }

        public long PreviousLsn
        {
            get { return this.previousLsn; }
        }

        public long Lsn
        {
            get { return this.lsn; }
        }

        public byte[] Data
        {
            get { return this.data; }
        }

        readonly long previousLsn;
        readonly long lsn;
        readonly byte[] data;
    }
}