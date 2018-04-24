using System;
using System.Contracts;

namespace TestApp2
{
    class Program
    {
        static void Main()
        {
            DataSource dataSource;

            using (var applf = Lifetime.Define())
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
        private readonly EventContainer<Action<int>> _container;

        public DataSource(OuterLifetime outerLifetime)
        {
            _lfd = Lifetime.DefineDependent(outerLifetime);
            _container = new EventContainer<Action<int>>(_lfd.Lifetime);
        }

        public event Action<int> IntegerComes
        {
            add { _container.Subscribe(value); }
            remove { _container.Unsubscribe(value); }
        }

        public void Trigger(int val)
        {
            OnIntegerComes(val);
        }

        protected virtual void OnIntegerComes(int obj)
        {
            _container.Invoke(act => act(obj));
        }

        public void Dispose()
        {
            _lfd.Terminate();
        }
    }
}
