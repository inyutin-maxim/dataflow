using System;              

namespace DataFlow
{
    public class Gate<T> : IProxy<T>, IMultipleParentSource<T>
    {
        private readonly Lifetime _lifetime;  
        private readonly Proxy<T> _pureProxy;

        public Gate(Lifetime lifetime)
        {
            _lifetime = lifetime;              
            _pureProxy = new Proxy<T>(_lifetime);
        }

        public Lifetime Lifetime => _lifetime;

        public void Subscribe(Action<T> handler)
        {
            _pureProxy.Subscribe(handler);
        }

        public void Subscribe(Action<T> handler, Lifetime lf)
        {
            _pureProxy.Subscribe(handler, lf);
        }

        public void Fire(T value)
        {
            _pureProxy.Fire(value);
        }

        public void AddParentSource(ISource<T> source)
        {
            source.Subscribe(Fire, Lifetime.WhenAny(source.Lifetime, Lifetime));
        }
    }
}
