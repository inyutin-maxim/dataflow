using System;

namespace DataFlow
{
    public static class SignalEx
    {
        public static ISourceAdapter<T> Where<T>(this ITarget<T> self, Func<T, bool> predecate)
        {
            var lf = Lifetime.DefineDependent(self.Lifetime).Lifetime;
            var signal = new SourceAdapter<T>(lf);

            self.Subscribe(x =>
            {
                if (predecate(x))
                {
                    signal.Fire(x);
                }
            });

            return signal;
        }     

        public static ISourceAdapter<TRes> Select<T, TRes>(this ITarget<T> self, Func<T, TRes> selector)
        {
            var lf = Lifetime.DefineDependent(self.Lifetime).Lifetime;
            var signal = new SourceAdapter<TRes>(lf);

            self.Subscribe(x =>
            {
                signal.Fire(selector(x));
            });

            return signal;
        }                   

        public static ISourceAdapter<T> Union<T>(this ITarget<T> self, ITarget<T> other)
        {
            var lf = Lifetime.WhenAll(self.Lifetime, other.Lifetime);
            var signal = new SourceAdapter<T>(lf);

            self.Subscribe(x => { signal.Fire(x); });
            other.Subscribe(x => { signal.Fire(x); });

            return signal;
        }

        public static IVoidSourceAdapter Union(this IVoidTarget self, IVoidTarget other)
        {
            var lf = Lifetime.WhenAll(self.Lifetime, other.Lifetime);
            var signal = new VoidSourceAdapter(lf);

            self.Subscribe(() => { signal.Fire(); });
            other.Subscribe(() => { signal.Fire(); });

            return signal;
        }

        public static IVoidSourceAdapter AsVoid<T>(this ITarget<T> self)
        {
            var lf = Lifetime.DefineDependent(self.Lifetime).Lifetime;
            var signal = new VoidSourceAdapter(lf);  
            return signal;
        }

        public static void ReportTo<T>(this ITarget<T> self, IGate<T> other)
        {
            other.AddParentSource(self);
        }
    }
}