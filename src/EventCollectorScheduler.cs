using System;
using System.Threading;
using System.Threading.Tasks;

namespace CDC.EventCollector
{
    class EventCollectorScheduler : IEventCollectorScheduler
    {
        public EventCollectorScheduler()
        {
            this.source = new TaskCompletionSource<bool>();
            this.waitingTillId = 0;
            this.timer = new Timer(this.OnEvent, null, TimeSpan.Zero, Timeout.InfiniteTimeSpan);
        }

        public Func<long, Task> Ready;

        public async void OnEvent(object state)
        {
            // todo: handle multi threaded ness of changing waitingTillId.
            await Ready(this.waitingTillId);

            this.source.SetResult(true);
            this.source = new TaskCompletionSource<bool>();

            // call the OnEvent again.
            this.timer.Change(TimeSpan.FromMilliseconds(20), Timeout.InfiniteTimeSpan);
        }

        public Task NewEvent(long id)
        {
            this.waitingTillId = id;
            return this.source.Task;
        }

        TaskCompletionSource<bool> source;
        private long waitingTillId;
        Timer timer;
    }
}