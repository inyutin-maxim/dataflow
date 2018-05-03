using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace System.Contracts.Tests
{
    using OnIntegerComesHandler = EventHandler<Action<int>>;

    [TestFixture]
    public class EventHandlerTests
    {
        [Test]
        public void EventHandlerShouldProduceUpdates()
        {
            var calledCount = new List<int>();
            void Subscription(int number) => calledCount.Add(number);

            var dataSource = new DataSource(Lifetime.Eternal);
            dataSource.IntegerComes += Subscription;
            dataSource.TriggerItPlease(12); 
            dataSource.TriggerItPlease(666);

            Assert.AreEqual(2, calledCount.Count, "Calls count");
            Assert.AreEqual(12, calledCount.First(), "Pushed value");
            Assert.AreEqual(666, calledCount.Last(), "Pushed value");
        }
        
        [Test]
        public void EventHandlerShouldFailAfterTermination()
        {
            var dataSource = new DataSource(Lifetime.Eternal);
            dataSource.IntegerComes += x => { };
            dataSource.Dispose();

            Assert.Throws<AlreadyTerminatedException>(() =>
            {
                dataSource.IntegerComes += x => { };
            });
        }

        [Test]
        public void EventHandlerShouldBeAutomaticallyUnsubscribedByExternalLifetime()
        {
            DataSource dataSource;
            var calledCount = new List<int>();
            void Subscription(int number) => calledCount.Add(number);

            using (var applf = Lifetime.Define("Root"))
            {
                dataSource = new DataSource(applf.Lifetime);
                dataSource.IntegerComes += Subscription;
                dataSource.TriggerItPlease(12); 
            }

            dataSource.TriggerItPlease(666);

            Assert.AreEqual(1, calledCount.Count, "Calls count");
            Assert.AreEqual(12, calledCount.First(), "Pushed value");
        }

        [Test]
        public void EventHandlerShouldTerminateInRightWay()
        {
            using (var applf = Lifetime.Define("Root"))
            {
                var dataSource = new DataSource(applf.Lifetime);
                dataSource.IntegerComes += x => { };
                dataSource.TriggerItPlease(12); 
                dataSource.Dispose();

                Assert.AreEqual(false, applf.Lifetime.IsTerminated, $"Is {applf.Name} lifetime terminated");
            }
        }

        [Test]
        public void EventHandlerShouldBeAutomaticallyUnsubscribedManually()
        {
            DataSource dataSource;
            var calledCount = new List<int>();
            void Subscription(int number) => calledCount.Add(number);

            using (var applf = Lifetime.Define("Root"))
            {
                dataSource = new DataSource(applf.Lifetime);
                dataSource.IntegerComes += Subscription;
                dataSource.TriggerItPlease(12); 
                dataSource.IntegerComes -= Subscription;
                dataSource.TriggerItPlease(24); 
            }

            dataSource.TriggerItPlease(666);

            Assert.AreEqual(1, calledCount.Count, "Calls count");
            Assert.AreEqual(12, calledCount.First(), "Pushed value");
        }

        sealed class DataSource : IDisposable
        {
    
            private readonly LifetimeDef _lfd;
            private readonly OnIntegerComesHandler _handler;

            public DataSource(OuterLifetime outerLifetime)
            {
                _lfd = Lifetime.DefineDependent(outerLifetime, "DataSource");
                _handler = OnIntegerComesHandler.Create(_lfd.Lifetime);
            }

            public event Action<int> IntegerComes
            {
                add => _handler.Subscribe(value);
                remove => _handler.Unsubscribe(value);
            }

            public void TriggerItPlease(int val)
            {
                OnIntegerComes(val);
            }

            private void OnIntegerComes(int obj)
            {
                _handler.InvokeAsync(act => act(obj)).Wait();
            }

            public void Dispose()
            {
                _lfd.Terminate();
            }
        }
    }
}
