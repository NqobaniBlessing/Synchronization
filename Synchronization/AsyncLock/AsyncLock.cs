namespace Synchronization.AsyncLock
{
    public interface ILockHandle : IDisposable {}

    public class AsyncLock
    {
        private readonly AsyncSemaphore _semaphore = new(1, 1);

        public ILockHandle Lock()
        {
            var handle = new LockHandle(this);

            handle.Lock();

            return handle;
        }

        public async Task<ILockHandle> LockAsync()
        {
            var handle = new LockHandle(this);

            await handle.LockAsync();

            return handle;
        }

        public void Dispose()
        {
            _semaphore.Dispose();
        }

        private class LockHandle(AsyncLock parent) : ILockHandle
        {
            private readonly AsyncLock _parent = parent ?? throw new ArgumentNullException(nameof(parent));

            public void Lock()
            {
                _parent._semaphore.Wait();
            }

            public Task LockAsync()
            {
                return _parent._semaphore.WaitAsync();
            }

            public void Dispose()
            {
                _parent._semaphore.Release();
            }
        }
    }
}
