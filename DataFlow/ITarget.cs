using System;
using System.Contracts;

namespace DataFlow
{
    /// <summary>
    /// Data source which can be subscribed
    /// </summary>
    public interface ITarget<out T>
    {
        Lifetime Lifetime { get; }

        void Subscribe(Action<T> handler);

        void Subscribe(Action<T> handler, Lifetime lf);
    }

    /// <summary>
    /// Void events source which can be subscribed
    /// </summary>
    public interface IVoidTarget
    {
        Lifetime Lifetime { get; }

        void Subscribe(Action handler);
    }

    /// <summary>
    /// Multiple sources aggregator
    /// </summary>
    public interface IGate<T> : ITarget<T>
    {
        void AddParentSource(ITarget<T> target);
    }
}