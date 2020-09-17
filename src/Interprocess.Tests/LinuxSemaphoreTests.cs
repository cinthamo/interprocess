﻿using Cloudtoid.Interprocess.Semaphore.Linux;
using FluentAssertions;

namespace Cloudtoid.Interprocess.Tests
{
    public class LinuxSemaphoreTests
    {
        [Fact(Platforms = Platform.Linux | Platform.FreeBSD)]
        public void CanReleaseAndWait()
        {
            using var sem = new SemaphoreLinux("my-sem", deleteOnDispose: true);
            sem.Wait(10).Should().BeFalse();
            sem.Release();
            sem.Release();
            sem.Wait(-1).Should().BeTrue();
            sem.Wait(10).Should().BeTrue();
            sem.Wait(0).Should().BeFalse();
            sem.Wait(10).Should().BeFalse();
            sem.Release();
            sem.Wait(10).Should().BeTrue();
        }

        [Fact(Platforms = Platform.Linux | Platform.FreeBSD)]
        public void CanCreateMultipleSemaphoresWithSameName()
        {
            using var sem1 = new SemaphoreLinux("my-sem", deleteOnDispose: true);
            using var sem2 = new SemaphoreLinux("my-sem", deleteOnDispose: false);
            sem2.Release();
            sem1.Wait(10).Should().BeTrue();
            sem1.Wait(10).Should().BeFalse();
            sem2.Wait(10).Should().BeFalse();
        }

        [Fact(Platforms = Platform.Linux | Platform.FreeBSD)]
        public void CanReuseSameSemaphoreName()
        {
            using (var sem = new SemaphoreLinux("my-sem", deleteOnDispose: true))
            {
                sem.Wait(10).Should().BeFalse();
                sem.Release();
                sem.Wait(-1).Should().BeTrue();
                sem.Release();
            }

            using (var sem = new SemaphoreLinux("my-sem", deleteOnDispose: false))
            {
                sem.Wait(10).Should().BeFalse();
                sem.Release();
                sem.Wait(-1).Should().BeTrue();
                sem.Release();
            }

            using (var sem = new SemaphoreLinux("my-sem", deleteOnDispose: true))
            {
                sem.Wait(10).Should().BeTrue();
                sem.Release();
                sem.Wait(-1).Should().BeTrue();
                sem.Release();
            }
        }
    }
}
