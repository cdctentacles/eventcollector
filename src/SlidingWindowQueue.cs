using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace CDC.EventCollector
{
    internal class SlidingWindowQueue
    {
        public SlidingWindowQueue()
        {
            this.queue = new ConcurrentQueue<TransactionData>();
            this.lsnSeen = long.MinValue;
        }

        public void Add(TransactionData transaction)
        {
            if (this.lsnSeen >= transaction.Lsn)
            {
                throw new ArgumentException($"LSN not in order. Received {transaction.Lsn} when we have seen {this.lsnSeen}");
            }
            this.queue.Enqueue(transaction);
            this.lsnSeen = transaction.Lsn;
        }

        public int SlideWindowTill(long lsn)
        {
            int totalTansactions = 0;

            while (!this.queue.IsEmpty)
            {
                TransactionData peekTransaction = null;
                if (this.queue.TryPeek(out peekTransaction))
                {
                    if (peekTransaction.Lsn > lsn)
                    {
                        break;
                    }

                    TransactionData dequeueTransaction = null;
                    if (this.queue.TryDequeue(out dequeueTransaction))
                    {
                        if (!dequeueTransaction.Equals(peekTransaction))
                        {
                            throw new Exception("SlidingWindowQueue : Fatal exception : Removed transaction from queue other than peeked.");
                        }
                        else
                        {
                            totalTansactions += 1;
                        }
                    }
                    else
                    {
                        throw new Exception("SlidingWindowQueue : Unexpected exception : Another thread removed from queue while peeked different transaction.");
                    }
                }
                else
                {
                    throw new Exception("SlidingWindowQueue : Unexpected exception : Another thread removed only element from queue after empty check.");
                }
            }

            return totalTansactions;
        }

        public IReadOnlyList<TransactionData> GetTransactions(long lsn)
        {
            return this.queue.TakeWhile(transaction => transaction.Lsn <= lsn).ToList().AsReadOnly();
        }

        // no lock for lsnSeen, as this is read/written by only one function
        // called on single thread.
        private long lsnSeen;
        ConcurrentQueue<TransactionData> queue;
    }
}
