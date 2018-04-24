using System;
using System.Collections.Generic;
using System.Contracts;

namespace TestApp2
{
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
            LifetimeDef lfd;
            if (_actions.TryGetValue(context, out lfd))
            {
                _actions.Remove(context);
                lfd.Terminate();
            }
        }

        public void Invoke(Action<T> func)
        {
            foreach (var subscriber in _list)
            {
                func(subscriber);
            }
        }
    }
}