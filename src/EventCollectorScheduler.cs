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

        public event Func<long, Task> Ready;

        public async void OnEvent(object state)
        {
            var handlers = Ready;
            if (null == handlers)
            {
                return;
            }

            Delegate[] invocationList = handlers.GetInvocationList();
            Task[] handlerTasks = new Task[invocationList.Length];

            // todo: handle multi threaded ness of changing waitingTillId.
            for (int i = 0; i < invocationList.Length; i++)
            {
                // todo : don't cast here . just a typedef
                handlerTasks[i] = ((Func<long, Task>)invocationList[i])(this.waitingTillId);
            }

            await Task.WhenAll(handlerTasks);
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