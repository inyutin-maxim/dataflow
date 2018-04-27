using System;
using System.Contracts;

namespace TestApp2
{
    using OnIntegerComesHandler = System.Contracts.EventHandler<Action<int>>;
 
    class DataSource : IDisposable
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

        public void Trigger(int val)
        {
            OnIntegerComes(val);
        }

        protected virtual void OnIntegerComes(int obj)
        {
            _handler.InvokeAsync(act => act(obj)).Wait();
        }

        public void Dispose()
        {
            _lfd.Terminate();
        }
    }
}