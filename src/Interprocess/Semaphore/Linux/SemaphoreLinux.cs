using System;

namespace Cloudtoid.Interprocess.Semaphore.Linux
{
    public class SemaphoreLinux : IInterprocessSemaphoreWaiter, IInterprocessSemaphoreReleaser
    {
        private const string HandleNamePrefix = "/ct.ip.";
        private readonly string name;
        private readonly bool deleteOnDispose;
        private readonly IntPtr handle;

        internal SemaphoreLinux(string name, bool deleteOnDispose = false)
            : this(name, 0, deleteOnDispose) { }

        public SemaphoreLinux(string name, uint initialCount, bool deleteOnDispose = false)
        {
            this.name = name = HandleNamePrefix + name;
            this.deleteOnDispose = deleteOnDispose;
            handle = Interop.CreateOrOpenSemaphore(name, initialCount);
        }

        ~SemaphoreLinux()
            => Dispose(false);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            Interop.Close(handle);

            if (deleteOnDispose)
                Interop.Unlink(name);
        }

        public void Release()
            => Interop.Release(handle);

        public bool Wait(int millisecondsTimeout)
            => Interop.Wait(handle, millisecondsTimeout);
    }
}