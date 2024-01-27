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

        private SemaphoreWindows(SysSemaphore handle)
        {
            this.handle = handle;
        }

        public static bool TryOpenExisting(string name, out SemaphoreWindows? semaphoreWindows)
        {
#pragma warning disable CA1416
            if (SysSemaphore.TryOpenExisting(HandleNamePrefix + name, out SysSemaphore? semaphore))
#pragma warning restore CA1416
            {
                semaphoreWindows = new SemaphoreWindows(semaphore);
                return true;
            }
            else
            {
                semaphoreWindows = null;
                return false;
            }
        }

        public void Dispose()
            => handle.Dispose();

        public void Release()
            => handle.Release();

        public bool Wait(int millisecondsTimeout)
            => handle.WaitOne(millisecondsTimeout);
    }
}