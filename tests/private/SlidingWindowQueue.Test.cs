using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using CDC.EventCollector;
using Xunit;
using Xunit.Abstractions;

namespace eventcollector.tests
{
    // Properties for SlidingWindowQueueTest :
    // 1. We can store transactions and get them all.
    // 2. If we store transaction with lsn-1, it will not come in query for lsn < lsn-1.
    // 3. If we store transaction with lsn-1, it will come in query for lsn == lsn-1.
    // 4. If we store transaction with lsn-1, it will come in query for lsn > lsn-1.
    // 5. Sliding the window till lsn-1 removes all transactions till lsn-1 including it.
    //    It should not be visible in query from then.
    // 6. All above points [1-5] should work in multi-threaded environment.
    //    Current requirement is to have one Add thread and one Slide thread concurrently.
    public class SlidingWindowQueueTest
    {
        public SlidingWindowQueueTest(ITestOutputHelper output)
        {
            // Enable console writing in test by un commenting below.
            var converter = new XunitConsoleWriter(output);
            Console.SetOut(converter);
        }

        [Fact]
        // case 1
        public void GetAllTransactionsInOrder()
        {
            const int numTransactions = 1000;
            var queue = this.GenerateQueueWithData(numTransactions);
            var transactions = queue.GetTransactions(numTransactions);
            Assert.Equal(numTransactions, transactions.Count);

            for (int i = 0; i < numTransactions; ++i)
            {
                Assert.Equal(i, transactions[i].Lsn);
                Assert.Equal(this.Data, transactions[i].Data);
            }
        }

        [Fact]
        // case 2,3,4,5
        public void GetTransactionsTillLsnInOrder()
        {
            const int numTransactions = 1000;
            var queue = this.GenerateQueueWithData(numTransactions);
            for (int i = 99; i <= numTransactions; i += 100)
            {
                var transactions = queue.GetTransactions(i);
                Assert.Equal(100, transactions.Count);
                for (int j = 0, lsn = i - 99; j < 100; ++j, ++lsn)
                {
                    Assert.Equal(lsn, transactions[j].Lsn);
                    Assert.Equal(this.Data, transactions[j].Data);
                }
                Assert.Equal(100, queue.SlideWindowTill(i));
            }
        }

        [Fact]
        // case 6
        public void OneAddAndSlideTaskConcurrently()
        {
            var queue = new SlidingWindowQueue();
            long numTotalTransactionsAdded = 0;
            long numTotalTransactionsRemoved = 0;
            const int MaxTimeToWaitInMs = 1000 * 20; // 20 seconds of test

            var addTask = Task.Run(async () => {
                var rand = new Random();
                var watch = Stopwatch.StartNew();
                while (watch.ElapsedMilliseconds < MaxTimeToWaitInMs)
                {
                    long numTransactionsAddedLocal = Interlocked.Read(ref numTotalTransactionsAdded);
                    int transactionsAdded = rand.Next(100, 1000);
                    for (int i = 1; i <= transactionsAdded; ++i)
                    {
                        var lsn = numTransactionsAddedLocal + i;
                        queue.Add(new TransactionData(lsn - 1, lsn, this.Data));
                        Interlocked.Increment(ref numTotalTransactionsAdded);
                    }

                    // Console.WriteLine($"Added total : {numTransactionsAddedLocal + transactionsAdded}");
                    await Task.Delay(rand.Next(0, 10));
                }
            });

            var slideTask = Task.Run(async () => {
                var rand = new Random();
                var watch = Stopwatch.StartNew();
                while (watch.ElapsedMilliseconds < MaxTimeToWaitInMs)
                {
                    if (addTask.IsCompleted)
                    {
                        break;
                    }

                    long numTransactionsAddedLocal = Interlocked.Read(ref numTotalTransactionsAdded);
                    var transactions = queue.GetTransactions(numTransactionsAddedLocal);
                    int numRemovedTransactions = queue.SlideWindowTill(numTransactionsAddedLocal);
                    numTotalTransactionsRemoved += numRemovedTransactions;

                    // Console.WriteLine($"Removed total : {numTotalTransactionsRemoved}");
                    Assert.True(transactions.Count == numRemovedTransactions,
                        $"{transactions.Count} == {numRemovedTransactions} : " +
                        $"No more transactions can be added with lsn less than {numTransactionsAddedLocal}");

                    long lastTransactionLsn = long.MinValue;
                    // all transactions are received in monotonically increasing lsn.
                    foreach (var transaction in transactions)
                    {
                        Assert.True(transaction.Lsn >= lastTransactionLsn);
                        lastTransactionLsn = transaction.Lsn;
                        Assert.Equal(this.Data, transaction.Data);
                    }

                    await Task.Delay(rand.Next(0, 10));
                }
            });

            Task.WaitAll(new Task[]{ addTask, slideTask });
            Console.WriteLine($"Added : {numTotalTransactionsAdded} Removed: {numTotalTransactionsRemoved}");
            Assert.True(numTotalTransactionsAdded >= numTotalTransactionsRemoved, "Not possible : Removed more transactions than added.");
            Assert.True(queue.SlideWindowTill(numTotalTransactionsAdded + 1) == (numTotalTransactionsAdded - numTotalTransactionsRemoved));
        }


        [Fact]
        // We don't support multiple producer or multiple consumer.
        public void OneProducerMultiConsumerFail()
        {
            var queue = new SlidingWindowQueue();
            long numTotalTransactionsAdded = 0;
            long numTotalTransactionsRemoved = 0;
            const int MaxTimeToWaitInMs = 1000 * 20; // 20 seconds of test
            var rand = new Random();

            Func<Task> runProducer = async () => {
                var watch = Stopwatch.StartNew();
                while (watch.ElapsedMilliseconds < MaxTimeToWaitInMs)
                {
                    var numTransactions = Interlocked.Read(ref numTotalTransactionsAdded);
                    int transactionsToAdd = rand.Next(100, 1000);
                    for (int i = 1; i <= transactionsToAdd; ++i)
                    {
                        var lsn = numTransactions + i;
                        queue.Add(new TransactionData(lsn - 1, lsn, this.Data));
                    }
                    Interlocked.Exchange(ref numTotalTransactionsAdded, numTransactions + transactionsToAdd);
                    await Task.Delay(rand.Next(0, 2));
                }
            };

            Func<int, Task> runConsumer = async (int index) => {
                var watch = Stopwatch.StartNew();
                while (watch.ElapsedMilliseconds < MaxTimeToWaitInMs)
                {
                    var numTransactions = Interlocked.Read(ref numTotalTransactionsAdded);
                    var transactions = queue.GetTransactions(numTransactions);
                    var numRemoved = queue.SlideWindowTill(numTransactions);
                    Assert.True(transactions.Count >= numRemoved);
                    while (true)
                    {
                        var totalRemoved = Interlocked.Read(ref numTotalTransactionsRemoved);
                        if (totalRemoved == Interlocked.CompareExchange(ref numTotalTransactionsRemoved, totalRemoved + numRemoved, totalRemoved))
                        {
                            break;
                        }
                    }
                    await Task.Delay(rand.Next(0, 10));
                }
            };

            const int numConsumers = 10;
            var allTasks = new List<Task>(1 + numConsumers);

            allTasks.Add(runProducer());
            for (int i = 0; i < numConsumers; ++i)
            {
                allTasks.Add(runConsumer(i));
            }

            try
            {
                Task.WaitAll(allTasks.ToArray());
            }
            catch (AggregateException ex)
            {
                foreach (var e in ex.Flatten().InnerExceptions)
                {
                    if (!e.Message.Contains("SlidingWindowQueue :"))
                    {
                        throw;
                    }
                }
            }
        }

        [Fact]
        public void QueueSemantics()
        {
            var queue = new SlidingWindowQueue();
            queue.Add(new TransactionData(-1, 0, this.Data));
            Assert.ThrowsAny<ArgumentException>(() => queue.Add(new TransactionData(-1, 0, this.Data)));
            Assert.ThrowsAny<ArgumentException>(() => queue.Add(new TransactionData(-2, -1, this.Data)));
            queue.Add(new TransactionData(0, 1, this.Data));
            Assert.ThrowsAny<ArgumentException>(() => queue.Add(new TransactionData(-1, 0, this.Data)));
        }

        private SlidingWindowQueue GenerateQueueWithData(int numTransactions)
        {
            var queue = new SlidingWindowQueue();
            for (int i = 0; i < numTransactions; ++i)
            {
                queue.Add(new TransactionData(i-1, i, this.Data));
            }
            return queue;
        }

        readonly byte[] Data = Encoding.ASCII.GetBytes("data");
    }
}