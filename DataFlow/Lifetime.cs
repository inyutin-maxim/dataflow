using System;
using System.Collections.Generic;

namespace DataFlow
{
    public class Lifetime
    {
        internal List<Action> Actions { get; } = new List<Action>();

        public static Lifetime Etheral = Define().Lifetime;

        public bool IsTerminated { get; internal set; }

        public Lifetime()
        {
            Add(() => IsTerminated = true);
        }

        public static LifetimeDef Define()
        {
            return new LifetimeDef();
        }

        public static LifetimeDef DefineDependent(Lifetime parent)
        {
            var def = new LifetimeDef();
            parent.Add(() => def.Terminate());
            return def;
        }

        public void Add(Action action)
        {
            Actions.Add(action);
        }

        public static Lifetime WhenBoth(Lifetime first, Lifetime other)
        {
            var def = Lifetime.Define();

            other.Actions.Insert(0, () =>
            {
                if (first.IsTerminated) def.Terminate();
            });

            first.Actions.Insert(0, () =>
            {
                if (other.IsTerminated) def.Terminate();
            });
            
            return def.Lifetime;
        }

        public static Lifetime WhenAny(Lifetime first, Lifetime second)
        {
            var def = Define();

            Action action = () =>
            {
                def.Terminate();
            };

            first.Actions.Insert(0, () =>
            {
                action();
                second.Actions.Remove(action);
            });

            second.Actions.Insert(0, () =>
            {
                action();
                first.Actions.Remove(action);
            });

            return def.Lifetime;
        }
    }
}