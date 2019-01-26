using System;
using System.Threading;
using System.Threading.Tasks;

namespace CDC.EventCollector
{
    class EventCollectorScheduler : IEventCollectorScheduler
    {
        public EventCollectorScheduler(Func<long, Task> onSchedule)
        {
            this.source = new TaskCompletionSource<bool>();
            this.waitingTillId = long.MinValue;
            this.Ready = onSchedule;
            this.timer = new Timer(this.OnEvent, null, TimeSpan.Zero, Timeout.InfiniteTimeSpan);
        }

        private Func<long, Task> Ready;

        public async void OnEvent(object state)
        {
            try
            {
                if (this.triedTillId == this.waitingTillId)
                {
                    return;
                }

                // todo: handle multi threaded ness of changing waitingTillId.
                await Ready(this.waitingTillId);
                this.source.SetResult(true);
            }
            catch (Exception ex)
            {
                // also report on IHealthStore.
                this.source.SetException(ex);
            }
            finally
            {
                this.triedTillId = this.waitingTillId;
                this.source = new TaskCompletionSource<bool>();
                // call the OnEvent again.
                this.timer.Change(TimeSpan.FromMilliseconds(20), Timeout.InfiniteTimeSpan);
            }
        }

        public Task NewEvent(long id)
        {
            if (id <= this.waitingTillId)
            {
                throw new InvalidOperationException("Event ids are monotonically increasing LSN values.");
            }

            this.waitingTillId = id;
            return this.source.Task;
        }


        TaskCompletionSource<bool> source;
        private long waitingTillId;
        private long triedTillId;
        Timer timer;
    }
}