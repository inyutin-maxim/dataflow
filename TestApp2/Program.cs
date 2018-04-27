using System;
using System.Contracts;

namespace TestApp2
{
    class Program
    {
        static void Main()
        {
            DataSource dataSource;

            using (var applf = Lifetime.Define("Root"))
            {
                dataSource = new DataSource(applf.Lifetime);
                dataSource.IntegerComes += Console.WriteLine;
                dataSource.Trigger(12);  // Выведет в консоль 
            }

            dataSource.Trigger(666); // Пусто
            Console.ReadKey();
        }
    }

    class DataSource : IDisposable
    {
        private readonly LifetimeDef _lfd;
        private readonly EventHandler<Action<int>> _handler;

        public DataSource(OuterLifetime outerLifetime)
        {
            _lfd = Lifetime.DefineDependent(outerLifetime, "DataSource");
            _handler = EventHandler<Action<int>>.Create(_lfd.Lifetime);
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
