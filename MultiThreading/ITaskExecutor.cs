using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiThreading
{
    public interface ITaskExecutor
    {
        Task<bool> Execute(ITask task, Priority priority);

        void Stop();
    }
}
