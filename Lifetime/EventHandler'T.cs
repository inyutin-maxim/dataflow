using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace System.Contracts
{
    public class EventHandler<T>
    {
        private readonly TaskScheduler _scheduler;
        private readonly LifetimeDef _lfd;
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        private readonly Dictionary<object, LifetimeDef> _actions = new Dictionary<object, LifetimeDef>();

        protected EventHandler(OuterLifetime outer, TaskScheduler scheduler)
        {
            _scheduler = scheduler;
            _lfd = Lifetime.DefineDependent(outer, typeof(EventHandler<T>).FullName);
        }

        public static EventHandler<T> Create(OuterLifetime outer, TaskScheduler context = null)
        {
            return new EventHandler<T>(outer, context ?? TaskScheduler.Current);
        }

        public void Subscribe(T context)
        {
            var subscriptionLfd = Lifetime.DefineDependent(_lfd, "Event handler subsciption Lfd");
            subscriptionLfd.Lifetime.Add(() => Unsubscribe(context));

            _lock.EnterWriteLock();
            _actions.Add(context, subscriptionLfd);
            _lock.ExitWriteLock();
        }

        public void Unsubscribe(T context)
        {
            _lock.EnterWriteLock();
            if (_actions.TryGetValue(context, out var lfd))
            {
                _actions.Remove(context);
                lfd.Terminate();
            }
            _lock.ExitWriteLock();
        }

        public async Task InvokeAsync(Action<T> func)
        {
            var errors = new List<Exception>();
            
            await Task.Factory.StartNew(() =>
            {
                _lock.EnterReadLock();
                var subscribers = _actions.Keys.ToList();
                _lock.ExitReadLock();

                foreach (var subscriber in subscribers)
                {
                    try
                    {
                        func((T) subscriber);
                    }
                    catch (Exception any)
                    {
                        errors.Add(any);
                    }
                }
            }, CancellationToken.None, TaskCreationOptions.None, _scheduler);

            if (errors.Count > 0)
            {
                throw new AggregateException("Failed to call some of event handlers", errors);
            }
        }
    }
}
