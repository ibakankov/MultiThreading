using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MultiThreading
{
    /// <summary>
    /// Limited count task executor
    /// </summary>
    public class FixedTaskPool : ITaskExecutor
    {
        private int _maxTaskCount;
        private ConcurrentPriorityQueue<ITask> _tasksQueue = new ConcurrentPriorityQueue<ITask>();
        private List<Task> _taskWorkers;
        private bool _isStopping = false;

        /// <summary>
        /// Limited count task executor
        /// </summary>
        public FixedTaskPool(int maxTaskCount)
        {
            if (maxTaskCount < 1)
                throw new ArgumentException(nameof(maxTaskCount));

            _maxTaskCount = maxTaskCount;
            _taskWorkers = new List<Task>(_maxTaskCount);
            for (int i = 0; i < maxTaskCount; i++)
                _taskWorkers.Add(Task.Run(() => TaskWorker()));
        }

        private async Task TaskWorker()
        {
            while (!_isStopping || !_tasksQueue.IsEmpty())
            {
                if (_tasksQueue.TryDequeue(out ITask nextTask))
                {
                    await Task.Run(() => nextTask.Execute());
                }
            }
        }

        /// <summary>
        /// Start execute tasks
        /// </summary>
        /// <returns>True - if task will be executed</returns>
        public Task<bool> Execute(ITask task, Priority priority)
        {
            if (_isStopping)
                return Task.FromResult(false);
            _tasksQueue.Enqueue(task, priority);
            return Task.FromResult(true);
        }

        /// <summary>
        /// Stop pool and wait all tasks to complete
        /// </summary>
        public void Stop()
        {
            _isStopping = true;
            Task.WhenAll(_taskWorkers).Wait();
        }        
    }
}
