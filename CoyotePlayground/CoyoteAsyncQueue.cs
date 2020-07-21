using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks.Dataflow;
using Microsoft.Coyote;
using Microsoft.Coyote.Random;
using Microsoft.Coyote.Runtime;
using Microsoft.Coyote.Specifications;
using Microsoft.Coyote.Tasks;
using static CoyotePlayground.InterestingEvent;

namespace CoyotePlayground
{
    public class CoyoteAsyncBuffer<T>
    {
        BufferBlock<T> _buffer;
        TextWriter _logger;

        public CoyoteAsyncBuffer(int capacity, TextWriter logger)
        {
            _buffer = new BufferBlock<T>(new DataflowBlockOptions { BoundedCapacity = capacity }); ;
            _logger = logger;
        }

        public void Enqueue(T item)
        {
            _buffer.Post(item);
        }

        public async Task<T> Dequeue()
        {
            var t = _buffer.ReceiveAsync();
            return await TaskExtensions.WrapInControlledTask(t);
        }
    }

    public class InterestingEvent 
    {
        public DateTime DateTime;
        public string TaskName;
        public StateEnum State;

        public enum StateEnum { 
            SemaphoreWait,
            SemaphoreEnter,
            SemaphoreReleased,
            MutexAttemptAcquire,
            MutexAcquired,
        }
    }

    public class InterestingEvents
    {
        List<InterestingEvent> interestingEvents = new List<InterestingEvent>();

        public void Mark(string taskName, StateEnum state)
        {
            interestingEvents.Add(
                new InterestingEvent
                {
                    DateTime = DateTime.UtcNow,
                    TaskName = taskName,
                    State = state
                });
        }

        public string SerializeToJson()
        {
            var json = JsonSerializer.Serialize(this.interestingEvents);
            return json;
        }
    }

    /// <summary>
    /// Warning: Produces deadlock!
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CoyoteAsyncQueue<T>
    {
        Queue<T> _buffer;
        AsyncLock _mutex;
        Semaphore _consumerSemaphore;
        Semaphore _producerSemaphore;
        InterestingEvents _logger;

        public CoyoteAsyncQueue(int capacity, InterestingEvents logger)
        {
            _logger = logger;
            _buffer = new Queue<T>();
            _mutex = AsyncLock.Create();
            _consumerSemaphore = Semaphore.Create(0, capacity);
            _producerSemaphore = Semaphore.Create(capacity, capacity);
        }

        public async Task Enqueue(T item, string taskName)
        {
            _logger.Mark(taskName, StateEnum.SemaphoreWait);
            await _producerSemaphore.WaitAsync();
            _logger.Mark(taskName, StateEnum.SemaphoreEnter);

            _logger.Mark(taskName, StateEnum.MutexAttemptAcquire);
            using (await _mutex.AcquireAsync())
            {
                _logger.Mark(taskName, StateEnum.MutexAcquired);
                _buffer.Enqueue(item);
            }

            // wakeup consumers            
            _consumerSemaphore.Release();
            _logger.Mark(taskName, StateEnum.SemaphoreReleased);
        }

        public async Task<T> Dequeue(string taskName)
        {
            T item;
            _logger.Mark(taskName, StateEnum.SemaphoreWait);
            await _consumerSemaphore.WaitAsync();
            _logger.Mark(taskName, StateEnum.SemaphoreEnter);

            _logger.Mark(taskName, StateEnum.MutexAttemptAcquire);
            using (await _mutex.AcquireAsync())
            {
                _logger.Mark(taskName, StateEnum.MutexAcquired);
                item = _buffer.Dequeue();
            }

            // wakeup producers            
            _producerSemaphore.Release();
            _logger.Mark(taskName, StateEnum.SemaphoreReleased);

            return item;
        }
    }

    public class TestCoyoteBlockingQueue
    {
        static IEnumerable<int> RandomStream(Generator generator, int streamLength)
        {
            return
                Enumerable
                .Repeat(0, streamLength)
                .Select(x => generator.NextInteger(100));
        }

        static IEnumerable<T> Slice<T>(IEnumerable<T> seq, int from, int to)
        {
            return seq.Take(to).Skip(from);
        }

        static List<List<T>> Chop<T>(IEnumerable<T> seq, int chunks)
        {
            var s = new List<List<T>>();
            var d = (decimal)seq.Count() / chunks;
            var chunk = (int)Math.Round(d, MidpointRounding.ToEven);
            for (int i = 0; i < chunks; i++)
            {
                s.Add(Slice(seq, i * chunk, (i * chunk) + chunk).ToList());
            }
            return s;
        }

        [Microsoft.Coyote.SystematicTesting.Test]
        public static async Task Execute_Buffer_TenReadersAndWriters(ICoyoteRuntime runtime)
        {
            Action<string> log = s => runtime.Logger.WriteLine(s);

            var generator = Generator.Create();
            var numConsumerProducer = generator.NextInteger(10) + 1;
            var numConsumers = numConsumerProducer;
            var numProducers = numConsumerProducer;

            log($"Testing Queue with {numConsumerProducer} consumers and producers");
            var queue = new CoyoteAsyncBuffer<int>(1000, runtime.Logger);

            var tasks =
                Chop(RandomStream(generator, 100), numProducers)
                .Select((x, i) => { var t = Task.Run(() => Writer(queue, x, $"Task{i}")); i++; return t; })
                .ToList();

            for (int i = 0; i < numProducers; i++)
            {
                tasks.Add(Task.Run(() => Reader(queue, "")));
            }

            await Task.WhenAll(tasks.ToArray());
        }

        [Microsoft.Coyote.SystematicTesting.Test]
        public static async Task Execute_TenReadersAndWriters(ICoyoteRuntime runtime)
        {
            Action<string> log = s => runtime.Logger.WriteLine(s);

            var generator = Generator.Create();
            var numConsumerProducer = generator.NextInteger(10) + 1;
            var numConsumers = numConsumerProducer;
            var numProducers = numConsumerProducer;

            log($"Testing Queue with {numConsumerProducer} consumers and producers");
            var queue = new CoyoteAsyncQueue<int>(numConsumerProducer, new InterestingEvents());

            var tasks =
                Chop(RandomStream(generator, 100), numProducers)
                .Select((x, i) => { var t = Task.Run(() => Writer(queue, x, $"Task{i}")); i++; return t; })
                .ToList();

            for (int i = 0; i < numProducers; i++)
            {
                tasks.Add(Task.Run(() => Reader(queue, "")));
            }

            await Task.WhenAll(tasks.ToArray());
        }

        [Microsoft.Coyote.SystematicTesting.Test]
        public static async Task Execute_TwoReadersAndWriters(ICoyoteRuntime runtime)
        {
            Action<string> log = s => runtime.Logger.WriteLine(s);

            var numConsumerProducer = 2;
            log($"Testing Queue with {numConsumerProducer} consumers and producers");
            var queue = new CoyoteAsyncQueue<int>(numConsumerProducer + 1, new InterestingEvents());

            var tasks = new List<Task>()
            {                
                Task.Run(() => Writer(queue, Enumerable.Range(1,10), "Task1")),
                Task.Run(() => Writer(queue, Enumerable.Range(10,20), "Task2")),
                Task.Run(() => Reader(queue, "Task3")),
                Task.Run(() => Reader(queue, "Task4")),
            };

            await Task.WhenAll(tasks.ToArray());
        }

        static async Task<T> Reader<T>(CoyoteAsyncQueue<T> queue, string taskname)
        {
            return await queue.Dequeue(taskname);
        }

        static async Task<T> Reader<T>(CoyoteAsyncBuffer<T> queue, string taskname)
        {
            return await queue.Dequeue();
        }

        static async Task Writer<T>(CoyoteAsyncQueue<T> queue, IEnumerable<T> items, string taskname)
        {
            foreach(var item in items)
            {
                await queue.Enqueue(item, taskname);
            }            
        }

        static void Writer<T>(CoyoteAsyncBuffer<T> queue, IEnumerable<T> items, string taskname)
        {
            foreach (var item in items)
            {
                queue.Enqueue(item);
            }
        }
    }
}
