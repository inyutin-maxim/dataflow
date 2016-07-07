using System;
using System.Collections.Generic;

namespace DataFlow
{
    public class Lifetime
    {
        private readonly List<Action> _actions = new List<Action>();

        internal List<Action> Actions
        {
            get { return _actions; }
        }

        public static Lifetime Etheral = Define().Lifetime;

        public bool IsTerminated { get; internal set; }

        public Lifetime()
        {
            Add(() => IsTerminated = true);
        }

        public static LifetimeDef Define(Lifetime parent = null)
        {
            var def = new LifetimeDef();
            parent?.Add(() => def.Terminate());
            return def;
        }

        public void Add(Action action)
        {
            Actions.Add(action);
        }

        public Lifetime Intersect(Lifetime other)
        {
            var def = Lifetime.Define();

            other.Actions.Insert(0, () =>
            {
                if (this.IsTerminated) def.Terminate();
            });

            Actions.Insert(0, () =>
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