using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MultiThreading.Test
{
    [TestClass]
    public class FixedTaskPoolTest
    {
        private FixedTaskPool taskPool;

        [TestInitialize()]
        public void MyTestInitialize()
        {
            taskPool = new FixedTaskPool(10);
        }

        [TestMethod]
        public async Task AllTasksShouldExecute()
        {
            var list = new List<TaskJob>(100);
            foreach (var num in Enumerable.Range(1, 100))
                list.Add(new TaskJob(num, num > 10 ? num > 50 ? Priority.High : Priority.Normal : Priority.Low));

            Assert.IsTrue(list.All(x => !x.IsComplete));

            foreach (var t in list)
                Assert.IsTrue(await taskPool.Execute(t, t.Priority));
            taskPool.Stop();

            Assert.IsTrue(list.All(x => x.IsComplete));
        }

        [TestMethod]
        public async Task CantExecuteAfterPoolStop()
        {
            var list = new List<TaskJob>(100);
            foreach (var num in Enumerable.Range(1, 100))
                list.Add(new TaskJob(num, num > 10 ? num > 50 ? Priority.High : Priority.Normal : Priority.Low));
            foreach (var t in list)
                Assert.IsTrue(await taskPool.Execute(t, t.Priority));
            taskPool.Stop();

            var anotherTask = new TaskJob(777, Priority.High);
            Assert.IsFalse(await taskPool.Execute(anotherTask, anotherTask.Priority));
            Assert.IsFalse(anotherTask.IsComplete);
        }

        [TestMethod]
        public void CantCreateEmptyPool()
        {
            Assert.ThrowsException<ArgumentException>(() => new FixedTaskPool(0));
        }
    }
}
