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

        private SemaphoreLinux(string name, IntPtr handle)
        {
            this.name = name;
            deleteOnDispose = false;
            this.handle = handle;
        }

        ~SemaphoreLinux()
            => Dispose(false);

        public static bool TryOpenExisting(string name, out SemaphoreLinux? semaphoreLinux)
        {
            name = HandleNamePrefix + name;
            var handle = Interop.OpenExistingSemaphore(name);
            semaphoreLinux = handle == IntPtr.Zero ? null : new SemaphoreLinux(name, handle);
            return semaphoreLinux != null;
        }

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