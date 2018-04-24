using System;
using System.Contracts;

namespace DataFlow
{
    public class Gate<T> : IGate<T>
    {
        private readonly Lifetime _lifetime;  

        private readonly SourceAdapter<T> _pureSourceAdapter;

        public Gate(Lifetime lifetime)
        {
            _lifetime = lifetime;              
            _pureSourceAdapter = new SourceAdapter<T>(_lifetime);
        }

        public Lifetime Lifetime => _lifetime;

        public void Subscribe(Action<T> handler)
        {
            _pureSourceAdapter.Subscribe(handler);
        }

        public void Subscribe(Action<T> handler, Lifetime lf)
        {
            _pureSourceAdapter.Subscribe(handler, lf);
        }

        public void AddParentSource(ITarget<T> target)
        {
            target.Subscribe(Fire, Lifetime.WhenAny(target.Lifetime, Lifetime));
        }
        
        private void Fire(T value)
        {
            _pureSourceAdapter.Fire(value);
        }
    }
}
