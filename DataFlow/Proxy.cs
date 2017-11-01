using System;
using System.Collections.Generic;

namespace DataFlow
{
    public class Proxy<T> : IProxy<T>
    {
        private readonly List<Action<T>> _handlers = new List<Action<T>>();
        
        public Lifetime Lifetime { get; private set; }

        public Proxy(Lifetime lf)
        {
            Lifetime = lf;
        }

        public void Subscribe(Action<T> handler)
        {
            Subscribe(handler, Lifetime);
        }

        public void Subscribe(Action<T> handler, Lifetime lf)
        {
            _handlers.Add(handler);
            lf.Add(() => _handlers.Remove(handler));
        }

        public void Fire(T value)
        {
            foreach (var handler in _handlers)
            {
                handler(value);
            }
        }
    }

    public class VoidProxy : IVoidProxy
    {
        private readonly List<Action> _handlers = new List<Action>();

        public Lifetime Lifetime { get; private set; }

        public VoidProxy(Lifetime lf)
        {
            Lifetime = lf;
        }

        public void Subscribe(Action handler)
        {
            _handlers.Add(handler);
            Lifetime.Add(() => _handlers.Remove(handler));
        }

        public void Fire()
        {
            foreach (var handler in _handlers)
            {
                handler();
            }
        }
    }
}