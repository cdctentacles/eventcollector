using System;
using System.Threading.Tasks;

namespace CDC.EventCollector
{
    class EventCollectorScheduler : IEventCollectorScheduler
    {
        public EventCollectorScheduler()
        {
            var source = new TaskCompletionSource<bool>();
            this.waitingTask = source.Task;
        }

        public event Func<object, Task> Ready;

        public async Task OnEvent()
        {
            var handlers = Ready;
            if (null == handlers)
            {
                return;
            }

            Delegate[] invocationList = handlers.GetInvocationList();
            Task[] handlerTasks = new Task[invocationList.Length];

            for (int i = 0; i < invocationList.Length; i++)
            {
                handlerTasks[i] = ((Func<object, EventArgs, Task>)invocationList[i])(this, EventArgs.Empty);
            }

            await Task.WhenAll(handlerTasks);
        }

        public Task NewEvent()
        {
            // return waiting task
            return Task.CompletedTask;
        }

        private Task waitingTask;
    }
}