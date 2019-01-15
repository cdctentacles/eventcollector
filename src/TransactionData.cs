using System.Collections.Generic;

namespace CDC.EventCollector
{
    public class TransactionData
    {
        public TransactionData(long lsn, byte[] data)
        {
            this.lsn = lsn;
            this.data = data;
        }

        public long Lsn
        {
            get { return this.lsn; }
        }

        public byte[] Data
        {
            get { return this.data; }
        }
        readonly long lsn;
        readonly byte[] data;
    }
}