using System;
using System.Collections.Generic;

namespace DataFlow
{
    public class Signal<T> : ISignal<T>
    {
        private readonly List<Action<T>> _handlers = new List<Action<T>>();
        
        public Lifetime Lifetime { get; private set; }

        public Signal(Lifetime lf)
        {
            Lifetime = lf;
        }

        public void Subscribe(Action<T> handler)
        {
            _handlers.Add(handler);
            Lifetime.Add(() => _handlers.Remove(handler));
        }

        public void Fire(T value)
        {
            foreach (var handler in _handlers)
            {
                handler(value);
            }
        }
    }
    public class VoidSignal : IVoidSignal
    {
        private readonly List<Action> _handlers = new List<Action>();

        public Lifetime Lifetime { get; private set; }

        public VoidSignal(Lifetime lf)
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