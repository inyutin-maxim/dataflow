﻿using System;
using System.Collections.Generic;
using System.Linq;

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

        /// <summary>
        /// Creates new instance of Lifetime which terminates only when last 
        /// dependent lifetime is terminated
        /// </summary>
        public static Lifetime WhenAll(params Lifetime[] lifetimes)
        {
            var def = Define();

            Action subscription = null;
            var act = new Action(() =>
            {
                if (!def.Lifetime.IsTerminated && lifetimes.All(x => x.IsTerminated))
                {
                    def.Terminate();
                    foreach (var lifetime in lifetimes)
                    {
                        // ReSharper disable once AccessToModifiedClosure
                        lifetime.Actions.Remove(subscription);
                    }
                }
            });
            subscription = act;

            foreach (var lifetime in lifetimes)
            {
                lifetime.Actions.Insert(0, subscription);
            }

            return def.Lifetime;
        }

        public static Lifetime WhenAny(params Lifetime[] lifetimes)
        {
            var def = Define();
            var lifetimesCopy = (Lifetime[])lifetimes.Clone();

            Action subscription = null;
            var act = new Action(() =>
            {
                if (!def.Lifetime.IsTerminated)
                {
                    def.Terminate();
                    foreach (var lifetime in lifetimes)
                    {
                        // ReSharper disable once AccessToModifiedClosure
                        lifetime.Actions.Remove(subscription);
                    }
                }
            });
            subscription = act;

            foreach (var lifetime in lifetimesCopy)
            {
                lifetime.Actions.Insert(0, subscription);
            }

            return def.Lifetime;
        }
    }
}