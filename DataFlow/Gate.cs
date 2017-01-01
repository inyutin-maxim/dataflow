using System;              

namespace DataFlow
{
    public class Gate<T> : ISignal<T>, IMultipleParentSource<T>
    {
        private readonly Lifetime _lifetime;  
        private readonly Signal<T> _pureSignal;

        public Gate(Lifetime lifetime)
        {
            _lifetime = lifetime;              
            _pureSignal = new Signal<T>(_lifetime);
        }

        public Lifetime Lifetime => _lifetime;

        public void Subscribe(Action<T> handler)
        {
            _pureSignal.Subscribe(handler);
        }

        public void Subscribe(Action<T> handler, Lifetime lf)
        {
            _pureSignal.Subscribe(handler, lf);
        }

        public void Fire(T value)
        {
            _pureSignal.Fire(value);
        }

        public void AddParentSource(ISource<T> source)
        {
            source.Subscribe(Fire, Lifetime.WhenAny(source.Lifetime, Lifetime));
        }
    }
}
