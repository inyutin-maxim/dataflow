using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace System.Contracts
{
    [DebuggerDisplay("Lifetime of {Actions.Count} instances")]
    public class Lifetime
    {
        private List<Action> Actions { get; } = new List<Action>();

        public static Lifetime Eternal = Define("Eternal").Lifetime;

        public bool IsTerminated { get; internal set; }

        public Lifetime()
        {
            Add(() => IsTerminated = true);
        }

        public void Add(Action action)
        {
            lock (Actions)
            {
                Actions.Add(action);
            }
        }

        public void AddDisposable(IDisposable disposable)
        {
            Add(disposable.Dispose);
        }

        public void AddBracket(Action subscribe, Action unsubscribe)
        {
            subscribe();
            Add(unsubscribe);
        }

        public void AddRef(object obj)
        {
            lock (Actions)
            {
                Actions.Add(() => GC.KeepAlive(obj));
            }
        }

        internal void Terminate()
        {
            lock (Actions)
            {
                if (IsTerminated) return;
                IsTerminated = true;

                for(var i = Actions.Count-1; i >= 0; i--)
                {
                    Actions[i]();
                }

                Actions.Clear();
            }
        }

        public static LifetimeDef Define(string name)
        {
            return new LifetimeDef(name);
        }

        public static LifetimeDef DefineDependent(OuterLifetime parent, string name = null)
        {
            var def = new LifetimeDef(name);
            parent.Lifetime.Add(() => def.Terminate());
            return def;
        }                             

        /// <summary>
        /// Creates new instance of Lifetime which terminates only when last 
        /// dependent lifetime is terminated
        /// </summary>
        public static Lifetime WhenAll(params OuterLifetime[] lifetimes)
        {
            var def = Define($"WhenAll of ({lifetimes.Length})");

            Action subscription = null;
            var act = new Action(() =>
            {
                if (!def.Lifetime.IsTerminated && lifetimes.All(x => x.IsTerminated))
                {
                    def.Terminate();
                    foreach (var lifetime in lifetimes)
                    {
                        // ReSharper disable once AccessToModifiedClosure
                        lifetime.Lifetime.Actions.Remove(subscription);
                    }
                }
            });
            subscription = act;

            foreach (var outerLifetime in lifetimes)
            {
                outerLifetime.Lifetime.Actions.Insert(0, subscription);
            }

            return def.Lifetime;
        }

        public static Lifetime WhenAny(params OuterLifetime[] lifetimes)
        {
            var def = Define($"WhenAny of {lifetimes.Length}");
            var lifetimesCopy = (OuterLifetime[])lifetimes.Clone();

            Action subscription = null;
            var act = new Action(() =>
            {
                if (!def.Lifetime.IsTerminated)
                {
                    def.Terminate();
                    foreach (var lifetime in lifetimes)
                    {
                        // ReSharper disable once AccessToModifiedClosure
                        lifetime.Lifetime.Actions.Remove(subscription);
                    }
                }
            });
            subscription = act;

            foreach (var outerLifetime in lifetimesCopy)
            {
                outerLifetime.Lifetime.Actions.Insert(0, subscription);
            }

            return def.Lifetime;
        }
    }
}