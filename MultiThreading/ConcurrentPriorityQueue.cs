using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace MultiThreading
{
    /// <summary>
    /// Queue based on Priority class
    /// </summary>
    public class ConcurrentPriorityQueue<T>
    {
        private readonly Dictionary<Priority, ConcurrentQueue<T>> _queues = new Dictionary<Priority, ConcurrentQueue<T>>();
        private int _highPriorityCounter = 0;
        private int _normalPriorityCounter = 0;
        private object _dequeueLock = new object();

        /// <summary>
        /// Queue based on Priority class
        /// </summary>
        public ConcurrentPriorityQueue()
        {
            foreach (Priority priority in Enum.GetValues(typeof(Priority)))
                _queues.Add(priority, new ConcurrentQueue<T>());
        }

        /// <summary>
        /// Put item according priority
        /// </summary>
        public void Enqueue(T item, Priority priority)
        {
            _queues[priority].Enqueue(item);
        }

        /// <summary>
        /// Try get item
        /// </summary>
        public bool TryDequeue(out T item)
        {
            lock (_dequeueLock)
            {
                if (_highPriorityCounter % 5 == 0 && _normalPriorityCounter > 0 && _queues[Priority.Normal].TryDequeue(out item))
                {
                    _normalPriorityCounter--;
                    return true;
                }

                if (_queues[Priority.High].TryDequeue(out item))
                {
                    _highPriorityCounter = _highPriorityCounter == 10 ? _normalPriorityCounter == 0 ? 1 : _highPriorityCounter : _highPriorityCounter + 1;

                    if (_highPriorityCounter % 5 == 0 && _normalPriorityCounter < 2)
                        _normalPriorityCounter = _highPriorityCounter / 5;
                    
                    return true;
                }

                if (_queues[Priority.Normal].TryDequeue(out item))
                {
                    _normalPriorityCounter = _normalPriorityCounter > 0 ? _normalPriorityCounter - 1 : 0;
                    _highPriorityCounter = 0;
                    return true;
                }

                return _queues[Priority.Low].TryDequeue(out item);
            }
        }

        /// <summary>
        /// Empty check
        /// </summary>
        public bool IsEmpty()
        {
            lock (_dequeueLock)
            {
                return _queues.All(x => x.Value.IsEmpty);
            }
        }
    }
}
