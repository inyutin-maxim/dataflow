using System;

namespace DataFlow
{
    public static class SignalEx
    {
        public static IProxy<T> Where<T>(this ISource<T> self, Func<T, bool> predecate)
        {
            var lf = Lifetime.DefineDependent(self.Lifetime).Lifetime;
            var signal = new Proxy<T>(lf);

            self.Subscribe(x =>
            {
                if (predecate(x))
                {
                    signal.Fire(x);
                }
            });

            return signal;
        }     

        public static IProxy<TRes> Select<T, TRes>(this ISource<T> self, Func<T, TRes> selector)
        {
            var lf = Lifetime.DefineDependent(self.Lifetime).Lifetime;
            var signal = new Proxy<TRes>(lf);

            self.Subscribe(x =>
            {
                signal.Fire(selector(x));
            });

            return signal;
        }                   

        public static IProxy<T> Union<T>(this ISource<T> self, ISource<T> other)
        {
            var lf = Lifetime.WhenBoth(self.Lifetime, other.Lifetime);
            var signal = new Proxy<T>(lf);

            self.Subscribe(x => { signal.Fire(x); });
            other.Subscribe(x => { signal.Fire(x); });

            return signal;
        }

        public static IVoidProxy Union(this IVoidSource self, IVoidSource other)
        {
            var lf = Lifetime.WhenBoth(self.Lifetime, other.Lifetime);
            var signal = new VoidProxy(lf);

            self.Subscribe(() => { signal.Fire(); });
            other.Subscribe(() => { signal.Fire(); });

            return signal;
        }

        public static IVoidProxy AsVoid<T>(this ISource<T> self)
        {
            var lf = Lifetime.DefineDependent(self.Lifetime).Lifetime;
            var signal = new VoidProxy(lf);  
            return signal;
        }
    }
}