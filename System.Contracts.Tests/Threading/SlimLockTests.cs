using System.Contracts.Threading;
using System.Threading;
using NUnit.Framework;

namespace System.Contracts.Tests.Threading
{
    [TestFixture]
    internal class SlimLockTests
    {
        [Test]
        public void ShouldEnterAndLeaveInSingleThread()
        {
            var @lock = new SlimLock();
            @lock.Enter();
            @lock.Leave();
        }

        [Test]
        public void ShouldEnterAndLeaveInSingleThreadWithTry()
        {
            var @lock = new SlimLock();
            Assert.IsTrue(@lock.TryEnter());
            @lock.Leave();
        }

        [Test]
        public void ShouldEnterAndWaitInSeparateThread()
        {
            var @ref = new LockRef();
            var manualEvent = new ManualResetEvent(false);
            var success = false;
            @ref.slimLock.Enter();

            ThreadPool.QueueUserWorkItem(x =>
            {
                success = !@ref.slimLock.TryEnter();
                manualEvent.Set();
            });
            manualEvent.WaitOne();
            @ref.slimLock.Leave();

            Assert.IsTrue(success, "Shouldn't enter in separate thread");
        }


        [Test]
        public void ShouldEnterAndWaitAndAcqLockInSeparateThread()
        {
            var @ref = new LockRef();
            using (var @event = new Barrier(2))
            {
                var firstSuccess = false;
                var secondSuccess = false;

                @ref.slimLock.Enter();

                ThreadPool.QueueUserWorkItem(x =>
                {
                    firstSuccess = !@ref.slimLock.TryEnter();

                    @event.SignalAndWait();
                    @event.SignalAndWait();

                    secondSuccess = @ref.slimLock.TryEnter();
                });

                @event.SignalAndWait();
                @ref.slimLock.Leave();
                @event.SignalAndWait();

                Assert.IsTrue(firstSuccess, "First thread blocked by first");
                Assert.IsTrue(secondSuccess, "Second thread blocked by first");
            }
        }

        [Test]
        public void ShouldFailToFreeInSeparateThread()
        {
            var @ref = new LockRef();
            var manualEvent = new ManualResetEvent(false);
            var success = false;

            @ref.slimLock.Enter();

            ThreadPool.QueueUserWorkItem(x =>
            {
                try
                {
                    @ref.slimLock.Leave();
                }
                catch
                {
                    success = true;
                }

                manualEvent.Set();
            });
            manualEvent.WaitOne();
            @ref.slimLock.Leave();

            Assert.IsTrue(success, "Should throw for locked in another thread");
        }

        [Test]
        public void ShouldFailForDoubleLeave()
        {
            var @lock = new SlimLock();
            @lock.Enter();
            @lock.Leave();
            Assert.Catch<Exception>(() => @lock.Leave(), "Enter/Leave should have balance");
        }

        class LockRef
        {
            public SlimLock slimLock;
        }
    }
}
