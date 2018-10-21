using System;
using System.Threading.Tasks;

namespace MultiThreading
{
    /// <summary>
    /// For testing purposes only
    /// </summary>
    public class TaskJob : ITask
    {
        private int _num;
        private Priority _priority;
        private bool _isComplete = false;

        public TaskJob(int num, Priority priority)
        {
            _num = num;
            _priority = priority;
        }

        public void Execute()
        {
            //Task.Delay(_num * 100).Wait();
            Console.WriteLine(_num + " " + Priority.ToString());
            _isComplete = true;
        }

        public Priority Priority
        {
            get
            {
                return _priority;
            }
        }

        public bool IsComplete
        {
            get
            {
                return _isComplete;
            }
        }
    }
}
