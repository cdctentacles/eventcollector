using System;
using System.Threading;
using System.Threading.Tasks;

namespace CDC.EventCollector
{
    class EventCollectorScheduler
    {
        public EventCollectorScheduler(Func<long, Task> onSchedule)
        {
            this.lockScheduler = new object();
            this.source = new TaskCompletionSource<bool>();
            this.waitingTillId = long.MinValue;
            this.triedTillId = this.waitingTillId;
            this.Ready = onSchedule;
            this.timer = new Timer(this.OnEvent, null, TimeSpan.Zero, Timeout.InfiniteTimeSpan);
        }

        private Func<long, Task> Ready;

        public async void OnEvent(object state)
        {
            long waitingTillIdLocal = this.waitingTillId;
            var sourceLocal = this.source;

            try
            {
                lock(lockScheduler)
                {
                    waitingTillIdLocal = this.waitingTillId;
                    sourceLocal = this.source;

                    if (this.triedTillId == waitingTillIdLocal)
                    {
                        return;
                    }

                    this.source = new TaskCompletionSource<bool>();
                }

                // todo: handle multi threaded ness of changing waitingTillIdLocal.
                await Ready(waitingTillIdLocal);
                sourceLocal.SetResult(true);
            }
            catch (Exception ex)
            {
                // also report on IHealthStore.
                sourceLocal.SetException(ex);
            }
            finally
            {
                this.triedTillId = waitingTillIdLocal;
                // call the OnEvent again.
                this.timer.Change(TimeSpan.FromMilliseconds(20), Timeout.InfiniteTimeSpan);
            }
        }

        public Task NewEvent(long id)
        {
            lock(lockScheduler)
            {
                if (id <= this.waitingTillId)
                {
                    throw new InvalidOperationException("Event ids are monotonically increasing LSN values.");
                }

                this.waitingTillId = id;
                return this.source.Task;
            }
        }


        private object lockScheduler;
        private TaskCompletionSource<bool> source; // read/written concurrently
        private long waitingTillId; // read/written concurrently.
        private long triedTillId; // read/written by single thread : OnEvent
        private readonly Timer timer;
    }
}