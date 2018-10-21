using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace MultiThreading.Test
{
    /// <summary>
    /// Сводное описание для ConcurrentPriorityQueue
    /// </summary>
    [TestClass]
    public class ConcurrentPriorityQueueTest
    {
        private ConcurrentPriorityQueue<TaskJob> queue;

        [TestInitialize()]
        public void MyTestInitialize()
        {
            queue = new ConcurrentPriorityQueue<TaskJob>();
        }

        [TestMethod]
        public void SingleObjectQueueTest()
        {
            var t = new TaskJob(1, Priority.Normal);

            Assert.IsTrue(queue.IsEmpty());

            queue.Enqueue(t, t.Priority);

            Assert.IsFalse(queue.IsEmpty());
            Assert.IsTrue(queue.TryDequeue(out var res));
            Assert.AreEqual(t, res);
        }

        [TestMethod]
        public void MultiObjectQueueTest()
        {
            var list = new List<TaskJob>(100);
            foreach (var num in Enumerable.Range(1, 100))
                list.Add(new TaskJob(num, num > 10 ? num > 50 ? Priority.High : Priority.Normal : Priority.Low));

            Assert.IsTrue(queue.IsEmpty());

            list.ForEach(t => queue.Enqueue(t, t.Priority));

            Assert.IsTrue(queue.TryDequeue(out var res));
            Assert.IsFalse(queue.IsEmpty());
        }

        [TestMethod]
        public void QueueOrderShouldntAffectDequeueTest()
        {
            var list = new List<TaskJob>(10);
            foreach (var num in Enumerable.Range(1, 10))
                list.Add(new TaskJob(num, num > 3 ? num > 6 ? Priority.High : Priority.Normal : Priority.Low));
            list.ForEach(t => queue.Enqueue(t, t.Priority));

            var straightResultPriority = new List<Priority>();
            foreach (var num in Enumerable.Range(1, 10))
            {
                Assert.IsTrue(queue.TryDequeue(out var res));
                straightResultPriority.Add(res.Priority);
            }

            list.Reverse();
            list.ForEach(t => queue.Enqueue(t, t.Priority));
            var reverseResultPriority = new List<Priority>();
            foreach (var num in Enumerable.Range(1, 10))
            {
                Assert.IsTrue(queue.TryDequeue(out var res));
                reverseResultPriority.Add(res.Priority);
            }

            Assert.IsTrue(straightResultPriority.SequenceEqual(reverseResultPriority));
        }

        [TestMethod]
        public void PriorityPatternTest()
        {
            var list = new List<TaskJob>(30);
            foreach (var num in Enumerable.Range(0, 30))
                list.Add(new TaskJob(num, num > 1 ? num > 8 ? Priority.High : Priority.Normal : Priority.Low));
            list.ForEach(t => queue.Enqueue(t, t.Priority));

            var resultPriority = new List<Priority>();
            foreach (var num in Enumerable.Range(0, 30))
            {
                Assert.IsTrue(queue.TryDequeue(out var res));
                resultPriority.Add(res.Priority);
            }

            var expected = new Priority[] {
                Priority.High, Priority.High, Priority.High, Priority.High, Priority.High,
                Priority.Normal,
                Priority.High, Priority.High, Priority.High, Priority.High, Priority.High,
                Priority.Normal, Priority.Normal,
                Priority.High, Priority.High, Priority.High, Priority.High, Priority.High,
                Priority.Normal,
                Priority.High, Priority.High, Priority.High, Priority.High, Priority.High,
                Priority.Normal, Priority.Normal,
                Priority.High,
                Priority.Normal,
                Priority.Low, Priority.Low
            };

            Assert.IsTrue(resultPriority.SequenceEqual(expected));
        }

        [TestMethod]
        public void NormalPriorityPatternTest()
        {
            var list = new List<TaskJob>();
            foreach (var num in Enumerable.Range(0, 11))
                list.Add(new TaskJob(num, Priority.High));
            list.ForEach(t => queue.Enqueue(t, t.Priority));

            var resultPriority = new List<Priority>();
            foreach (var num in Enumerable.Range(0, 11))
            {
                Assert.IsTrue(queue.TryDequeue(out var res));
                resultPriority.Add(res.Priority);
            }

            list = new List<TaskJob>();
            foreach (var num in Enumerable.Range(0, 11))
                list.Add(new TaskJob(num, num > 0 ? num > 4 ? Priority.High : Priority.Normal : Priority.Low));
            list.ForEach(t => queue.Enqueue(t, t.Priority));

            foreach (var num in Enumerable.Range(0, 11))
            {
                Assert.IsTrue(queue.TryDequeue(out var res));
                resultPriority.Add(res.Priority);
            }

            var expected = new Priority[] {
                Priority.High, Priority.High, Priority.High, Priority.High, Priority.High,
                Priority.High, Priority.High, Priority.High, Priority.High, Priority.High,
                Priority.High,
                Priority.Normal, Priority.Normal,
                Priority.High, Priority.High, Priority.High, Priority.High, Priority.High,
                Priority.Normal,
                Priority.High,
                Priority.Normal,
                Priority.Low
            };

            Assert.IsTrue(resultPriority.SequenceEqual(expected));
        }
    }
}
