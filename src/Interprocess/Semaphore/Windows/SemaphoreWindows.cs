using SysSemaphore = System.Threading.Semaphore;

namespace Cloudtoid.Interprocess.Semaphore.Windows
{
    // just a wrapper over the Windows named semaphore
    public sealed class SemaphoreWindows : IInterprocessSemaphoreWaiter, IInterprocessSemaphoreReleaser
    {
        private const string HandleNamePrefix = @"Global\CT.IP.";
        private readonly SysSemaphore handle;

        public SemaphoreWindows(string name, int initialCount = 0, int maximumCount = int.MaxValue)
        {
            handle = new SysSemaphore(initialCount, maximumCount, HandleNamePrefix + name);
        }

        public void Dispose()
            => handle.Dispose();

        public void Release()
            => handle.Release();

        public bool Wait(int millisecondsTimeout)
            => handle.WaitOne(millisecondsTimeout);
    }
}