using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiThreading
{
    class Program
    {
        static void Main(string[] args)
        {
            var ftp = new FixedTaskPool(20);
            var list = new List<TaskJob>(100);
            foreach (var num in Enumerable.Range(1, 100))
                list.Add(new TaskJob(num, num > 10 ? num > 50 ? Priority.High : Priority.Normal : Priority.Low));
            foreach (var t in list)
                ftp.Execute(t, t.Priority);
            ftp.Stop();
            Console.WriteLine("finished");
        }
    }
}
