namespace Synchronization.AsyncLock
{
    public class AsyncSemaphore : IDisposable
    {
        private int _currentCount; // Current number of available slots
        private readonly int _maxCount;
        private readonly Queue<TaskCompletionSource<bool>> _waitQueue = new();
        private readonly object _lock = new();

        public AsyncSemaphore(int initialCount, int maxCount)
        {
            if (initialCount < 0 || maxCount <= 0 || initialCount > maxCount)
                throw new ArgumentOutOfRangeException(nameof(initialCount));

            _currentCount = initialCount;
            _maxCount = maxCount;
        }

        public void Wait()
        {
            lock (_lock)
            {
                while (_currentCount == 0)
                {
                    Monitor.Wait(_lock); // Block the thread until a slot is available
                }

                _currentCount--; // Take a slot
            }
        }

        public Task WaitAsync()
        {
            lock (_lock)
            {
                if (_currentCount > 0)
                {
                    _currentCount--;
                    return Task.CompletedTask;
                }

                var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
                _waitQueue.Enqueue(tcs);
                return tcs.Task; // Task that will complete when a slot is available
            }
        }

        public void Release()
        {
            lock (_lock)
            {
                if (_currentCount == _maxCount)
                    throw new SemaphoreFullException("Cannot release beyond max count");

                if (_waitQueue.Count > 0)
                {
                    var tcs = _waitQueue.Dequeue();
                    tcs.SetResult(true); // awake an asynchronous waiter
                }
                else
                {
                    _currentCount++;
                    Monitor.Pulse(_lock); // awake a synchronous waiter
                }
            }

            Console.WriteLine($"Task releasing...");
        }

        public void Dispose()
        {
            
        }
    }
}
