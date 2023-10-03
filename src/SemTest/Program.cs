using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

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
        private const string Lib = "libSystem.dylib";
        private const int OCREAT = 0x0200;  // create the semaphore if it does not exist

        private static readonly IntPtr SemFailed = new(-1);

        [DllImport(Lib, SetLastError = true)]
        private static extern IntPtr sem_open([MarshalAs(UnmanagedType.LPStr)] string name, int oflag, uint mode, uint value);

        [LibraryImport(Lib, SetLastError = true)]
        private static partial int sem_close(IntPtr handle);

        private static void CreateOrOpenSemaphore(string name, uint initialCount)
        {
            var handle = sem_open(name, OCREAT, (uint)PosixFilePermissions.ACCESSPERMS, initialCount);
            if (handle != SemFailed)
            {
                sem_close(handle);
                Console.WriteLine($"Success");
            }
            else
            {
                int error = Marshal.GetLastWin32Error();
                Console.WriteLine($"Error: {error}");
            }
        }

        public static void Main()
        {
            //CreateOrOpenSemaphore("/ct.ip.SpeciBuildSegme127940Q1", 0);
            CreateOrOpenSemaphore("/ct.ip.Spec", 0);
        }
    }
}
