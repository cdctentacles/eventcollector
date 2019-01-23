using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using CDC.EventCollector;
using Xunit;

namespace eventcollector.tests
{
    // Properties of EventCollectorScheduler
    // 1. Should not fire another event when one event has not completed.
    // 2. If event for lsn L1 is fired and
    //    a. it returns success, should not fire it again for L1.
    //    b. it returns failure, should fire it again for L >= L1.
    // 3. Should reject new event with lsn less than already seen.
    // 4. If task t1 is returned for l1, then if event for l1 returns success, then t1 should complete.
    //    (b) If task t2 =/= t1 is returned for l2, then on success of l1, t2 should not complete.
    // 4. This all should work during multi threading.
    public class EventCollectorSchedulerTest
    {
        [Fact]
        public void ShouldNotFireMoreThanOneEventAtATime()
        {
            var invariantTester = new SchedulerInVariantTester();
            var scheduler = new EventCollectorScheduler(invariantTester.OnSchedule);

            long MaxTimeToWaitInMs = 200;
            var watch = Stopwatch.StartNew();
            long lsn = 0;

            while (watch.ElapsedMilliseconds < MaxTimeToWaitInMs)
            {
                scheduler.NewEvent(lsn);
                lsn += 1;
            }
        }

        [Fact]
        public void ShouldNotFireEventAgainForSuccessLsn()
        {
            var invariantTester = new SchedulerInVariantTester();
            var scheduler = new EventCollectorScheduler(invariantTester.OnScheduleWithLsn);

            long MaxTimeToWaitInMs = 200;
            var watch = Stopwatch.StartNew();
            long lsn = 0;

            while (watch.ElapsedMilliseconds < MaxTimeToWaitInMs)
            {
                scheduler.NewEvent(lsn);
                lsn += 1;
            }
        }

        [Fact]
        public void ShouldFireEventAgainForFailureLsn()
        {
            var invariantTester = new SchedulerInVariantTester();
            var scheduler = new EventCollectorScheduler(invariantTester.OnScheduleWithSomeFailure);

            long MaxTimeToWaitInMs = 200;
            var watch = Stopwatch.StartNew();
            long lsn = 0;

            while (watch.ElapsedMilliseconds < MaxTimeToWaitInMs)
            {
                scheduler.NewEvent(lsn);
                lsn += 1;
            }

            Assert.True(invariantTester.HasReceivedEventAfterFailedEvents());
        }
    }

    class SchedulerInVariantTester
    {
        public async Task OnSchedule(long persistTillLsn)
        {
            Interlocked.Increment(ref this.numConcurrentCalls);
            Assert.True(1 == Interlocked.Read(ref this.numConcurrentCalls), "More than one events scheduled at a time.");
            Interlocked.Decrement(ref this.numConcurrentCalls);
            Assert.True(0 == Interlocked.Read(ref this.numConcurrentCalls), "More than one events scheduled at a time.");
            await Task.CompletedTask;
        }

        public async Task OnScheduleWithLsn(long lsn)
        {
            Assert.True(this.persistTillLsn < lsn,
                $"Seen event again for a lsn which is persisted : {this.persistTillLsn} >= {lsn}");
            this.persistTillLsn = lsn;
            await this.OnSchedule(lsn);
        }

        public async Task OnScheduleWithSomeFailure(long lsn)
        {
            // 30 % chance of failure.
            if (this.rand.Next(1, 10) >= 3)
            {
                this.lsnSucceeded.Add(lsn);
                await this.OnScheduleWithLsn(lsn);
            }
            else
            {
                this.lsnFailed.Add(lsn);
                await Task.FromException(new Exception("some random exception"));
            }
        }

        public bool HasReceivedEventAfterFailedEvents()
        {
            if (this.lsnFailed.Count == 0 || this.lsnSucceeded.Count == 0)
            {
                return true;
            }

            // anyLsnInSuccessWithSmallerLsnInFailed
            return this.lsnSucceeded.Any(ls => this.lsnFailed.Any(lf => lf <= ls));
        }

        long numConcurrentCalls = 0;
        long persistTillLsn = long.MinValue;
        List<long> lsnFailed = new List<long>();
        List<long> lsnSucceeded = new List<long>();
        Random rand = new Random();
    }
}
