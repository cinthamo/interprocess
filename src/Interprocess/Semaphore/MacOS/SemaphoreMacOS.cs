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

        ~SemaphoreMacOS()
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