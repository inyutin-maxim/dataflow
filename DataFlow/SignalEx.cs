using System;

namespace DataFlow
{
    public static class SignalEx
    {
        public static ISignal<T> Where<T>(this ISource<T> self, Func<T, bool> predecate)
        {
            var lf = Lifetime.Define(self.Lifetime).Lifetime;
            var signal = new Signal<T>(lf);

            self.Subscribe(x => { if (predecate(x)) { signal.Fire(x); }});

            return signal;
        }     

        public static ISignal<TRes> Select<T, TRes>(this ISource<T> self, Func<T, TRes> selector)
        {
            var lf = Lifetime.Define(self.Lifetime).Lifetime;
            var signal = new Signal<TRes>(lf);

            self.Subscribe(x => { signal.Fire(selector(x)); } );

            return signal;
        }                   

        public static ISignal<T> Union<T>(this ISource<T> self, ISource<T> other)
        {
            var lf = Lifetime.Define(self.Lifetime.Intersect(other.Lifetime)).Lifetime;
            var signal = new Signal<T>(lf);

            self.Subscribe(x => { signal.Fire(x); });
            other.Subscribe(x => { signal.Fire(x); });

            return signal;
        }

        public static IVoidSignal Union(this IVoidSource self, IVoidSource other)
        {
            var lf = Lifetime.Define(self.Lifetime.Intersect(other.Lifetime)).Lifetime;
            var signal = new VoidSignal(lf);

            self.Subscribe(() => { signal.Fire(); });
            other.Subscribe(() => { signal.Fire(); });

            return signal;
        }

        public static IVoidSignal AsVoid<T>(this ISource<T> self)
        {
            var lf = Lifetime.Define(self.Lifetime).Lifetime;
            var signal = new VoidSignal(lf);  
            return signal;
        }
    }
}