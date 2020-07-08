using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ReviseApp
{
    public class BlockingQueue<T>
    {
        Queue<T> _queue;
        Mutex _mutex;
        Semaphore _consumerSemaphore;
        Semaphore _producerSemaphore;

        public BlockingQueue(int capacity)
        {
            _queue = new Queue<T>();
            _mutex = new Mutex();
            _consumerSemaphore = new Semaphore(0, capacity);
            _producerSemaphore = new Semaphore(capacity, capacity);
        }

        public void Enqueue(T item)
        {
            _producerSemaphore.WaitOne();

            _mutex.WaitOne();
            try
            {
                _queue.Enqueue(item);
            }
            finally
            {
                _mutex.ReleaseMutex();
            }

            // wakeup consumers
            _consumerSemaphore.Release();
        }

        public T Dequeue()
        {
            T item;
            _consumerSemaphore.WaitOne();

            _mutex.WaitOne();
            try
            {
                item = _queue.Dequeue();
            }
            finally
            {
                _mutex.ReleaseMutex();
            }

            // wakeup producers
            _producerSemaphore.Release();

            return item;
        }
    }

    [TestClass]
    public class Revise_MutexSemaphore
    {
        IEnumerable<int> Slice(IEnumerable<int> seq, int from, int to)
        {
            return seq.Take(to).Skip(from);
        }

        List<List<int>> Chop(IEnumerable<int> seq, int chunks)
        {
            var s = new List<List<int>>();
            var d = (decimal)seq.Count() / chunks;
            var chunk = (int) Math.Round(d, MidpointRounding.ToEven);
            for (int i = 0; i < chunks; i++)
            {
                s.Add(Slice(seq, i * chunk, (i * chunk) + chunk).ToList());
            }
            return s;
        }

        void Insert<T>(BlockingQueue<T> queue, IEnumerable<T> items)
        {
            foreach(var item in items)
            {
                queue.Enqueue(item);
            }
        }

        [TestMethod]
        public void SliceTest()
        {
            var range = Enumerable.Range(1, 30);
            var s1 = Slice(range, 0, 10);
            var s2 = Slice(range, 10, 20);
            var s3 = Slice(range, 20, 30);

            CollectionAssert.AreEqual(Enumerable.Range(1, 10).ToArray(), s1.ToArray());
            CollectionAssert.AreEqual(Enumerable.Range(11, 10).ToArray(), s2.ToArray());
            CollectionAssert.AreEqual(Enumerable.Range(21, 10).ToArray(), s3.ToArray());
        }

        [TestMethod]
        public void Chop_30Into3Chunks_Test()
        {
            var range = Enumerable.Range(1, 30);
            var chopped = Chop(range, 3);

            CollectionAssert.AreEqual(Enumerable.Range(1, 10).ToArray(), chopped[0].ToArray());
            CollectionAssert.AreEqual(Enumerable.Range(11, 10).ToArray(), chopped[1].ToArray());
            CollectionAssert.AreEqual(Enumerable.Range(21, 10).ToArray(), chopped[2].ToArray());
        }

        [TestMethod]
        public void Chop_29Into3Chunks_Test()
        {
            var range = Enumerable.Range(1, 29);
            var chopped = Chop(range, 3);

            CollectionAssert.AreEqual(Enumerable.Range(1, 10).ToArray(), chopped[0].ToArray());
            CollectionAssert.AreEqual(Enumerable.Range(11, 10).ToArray(), chopped[1].ToArray());
            CollectionAssert.AreEqual(Enumerable.Range(21, 9).ToArray(), chopped[2].ToArray());
        }

        IEnumerable<int> yieldIt(Func<int> f, int numItems)
        {
            for(int i = 0; i < numItems; i++)
            {
                yield return f();
            };
        }

        [TestMethod]
        public async Task TestBlockingQueue()
        {
            var queue = new BlockingQueue<int>(100);
            var seq = Enumerable.Range(1,99).ToList();

            var tasks =
                Chop(seq, 5)
                .Select(s =>
                {
                    var t = new TaskFactory().StartNew(() => Insert(queue, s));
                    return t;
                })
                .ToArray();

            await Task.WhenAll(tasks);

            var rt =
                new TaskFactory()
                .StartNew(() => yieldIt(queue.Dequeue, seq.Count));
            var result = (await rt).OrderBy(x => x).ToList();

            CollectionAssert.AreEqual(seq, result, 
                $"Expected {String.Join(",", seq)} Got {String.Join(",", result)}");
        }
    }
}
