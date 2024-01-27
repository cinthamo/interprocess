using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Cloudtoid.Interprocess.Semaphore.MacOS;

namespace SemTest
{
    [Flags]
    internal enum PosixFilePermissions : uint
    {
        S_ISUID = 0x0800, // Set user ID on execution
        S_ISGID = 0x0400, // Set group ID on execution
        S_ISVTX = 0x0200, // Save swapped text after use (sticky).
        S_IRUSR = 0x0100, // Read by owner
        S_IWUSR = 0x0080, // Write by owner
        S_IXUSR = 0x0040, // Execute by owner
        S_IRGRP = 0x0020, // Read by group
        S_IWGRP = 0x0010, // Write by group
        S_IXGRP = 0x0008, // Execute by group
        S_IROTH = 0x0004, // Read by other
        S_IWOTH = 0x0002, // Write by other
        S_IXOTH = 0x0001, // Execute by other

        S_IRWXG = S_IRGRP | S_IWGRP | S_IXGRP,
        S_IRWXU = S_IRUSR | S_IWUSR | S_IXUSR,
        S_IRWXO = S_IROTH | S_IWOTH | S_IXOTH,
        ACCESSPERMS = S_IRWXU | S_IRWXG | S_IRWXO, // 0777
        ALLPERMS = S_ISUID | S_ISGID | S_ISVTX | S_IRWXU | S_IRWXG | S_IRWXO, // 07777
        DEFFILEMODE = S_IRUSR | S_IWUSR | S_IRGRP | S_IWGRP | S_IROTH | S_IWOTH, // 0666
    }

    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:Element should begin with upper-case letter", Justification = "Matching the exact names in Linux/MacOS")]
    public static partial class Program
    {
        private const string Lib = "/Users/cristian/Documents/GX/dev/interprocess/src/Interprocess/bin/Cloudtoid.Interprocess.Native.dylib";
        //private const int OCREAT = 0x0200;  // create the semaphore if it does not exist
        //private const int OEXCL = 0x0400;

        private static readonly IntPtr SemFailed = new(-1);

        [DllImport(Lib, SetLastError = true)]
        private static extern IntPtr gx_sem_open_or_create([MarshalAs(UnmanagedType.LPStr)] string name, uint value);

        [LibraryImport(Lib, SetLastError = true)]
        private static partial int gx_sem_close(IntPtr handle);

        [DllImport(Lib, SetLastError = true)]
        private static extern int gx_sem_trywait(IntPtr handle);

        [DllImport(Lib, SetLastError = true)]
        private static extern int gx_sem_unlink([MarshalAs(UnmanagedType.LPStr)] string name);

        [DllImport(Lib, SetLastError = true)]
        private static extern int gx_sem_post(IntPtr handle);

        public static void Main()
        {
            var x = 1;
            if (x == 0)
                DirectCall();
            else
                ClassCall();
        }

        private static void ClassCall()
        {
            gx_sem_unlink("/gx/name3");
            var s = new SemaphoreMacOS("/name3", 0, true);
            s.Wait(5000);
            Console.WriteLine("Success");
        }

        private static void DirectCall()
        {
            string name = "/gx/name3"; //"/gxW586311";
            gx_sem_unlink(name);
            var handle = gx_sem_open_or_create(name, 0);
            //var handle = gx_sem_open(name, OCREAT | OEXCL, (uint)PosixFilePermissions.ACCESSPERMS, 0);
            if (handle != SemFailed)
            {
                int x = gx_sem_trywait(handle);
                int err = Marshal.GetLastWin32Error();
                //sem_post(handle);
                //sem_wait(handle);
                gx_sem_close(handle);
                Console.WriteLine($"Success {x} {err}");
            }
            else
            {
                int error = Marshal.GetLastWin32Error();
                Console.WriteLine($"Error open: {error}");
            }
        }
    }
}
