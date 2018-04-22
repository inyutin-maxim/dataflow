using System;
using System.Collections.Generic;
using DataFlow;

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

    public class EventContainer<T>
    {
        private readonly LifetimeDef _lfd;
        private readonly List<T> _list = new List<T>();
        private readonly Dictionary<object, LifetimeDef> _actions = new Dictionary<object, LifetimeDef>();

        public EventContainer(OuterLifetime outer)
        {
            _lfd = Lifetime.DefineDependent(outer);
        }

        public void Subscribe(T context)
        {                        
            _list.Add(context);

            var localLifetime = Lifetime.DefineDependent(_lfd);
            localLifetime.Lifetime.Add(() => _list.Remove(context));

            _actions.Add(context, localLifetime);
        }

        public void Unsubscribe(T context)
        {
            var lfd = _actions[context];
            _actions.Remove(context);
            lfd.Terminate();
        }

        public void Invoke(Action<T> func)
        {
            foreach (var subscriber in _list)
            {
                func(subscriber);
            }
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
