using System;

namespace Cloudtoid.Interprocess.Semaphore.MacOS
{
    public class SemaphoreMacOS : IInterprocessSemaphoreWaiter, IInterprocessSemaphoreReleaser
    {
        private const string HandleNamePrefix = "/gx";
        private readonly string name;
        private readonly bool deleteOnDispose;
        private readonly IntPtr handle;

        internal SemaphoreMacOS(string name, bool deleteOnDispose = false)
            : this(name, 0, deleteOnDispose) { }

        public SemaphoreMacOS(string name, uint initialCount, bool deleteOnDispose = false)
        {
            this.name = name = HandleNamePrefix + name;
            this.deleteOnDispose = deleteOnDispose;
            handle = Interop.CreateOrOpenSemaphore(name, initialCount);
        }

        private SemaphoreMacOS(string name, IntPtr handle)
        {
            this.name = name;
            deleteOnDispose = false;
            this.handle = handle;
        }

        ~SemaphoreMacOS()
            => Dispose(false);

        public static bool TryOpenExisting(string name, out SemaphoreMacOS? semaphoreMacOs)
        {
            name = HandleNamePrefix + name;
            var handle = Interop.OpenExistingSemaphore(name);
            semaphoreMacOs = handle == IntPtr.Zero ? null : new SemaphoreMacOS(name, handle);
            return semaphoreMacOs != null;
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