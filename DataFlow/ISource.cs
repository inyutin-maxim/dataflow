using System;

namespace DataFlow
{
    /// <summary>
    /// Data source which can be subscribed
    /// </summary>
    public interface ISource<out T>
    {
        Lifetime Lifetime { get; }

        void Subscribe(Action<T> handler);

        void Subscribe(Action<T> handler, Lifetime lf);
    }

    /// <summary>
    /// Void events source which can be subscribed
    /// </summary>
    public interface IVoidSource
    {
        Lifetime Lifetime { get; }

        void Subscribe(Action handler);
    }

    /// <summary>
    /// Multiple sources aggregator
    /// </summary>
    public interface IMultipleParentSource<T> : ISource<T>
    {
        void AddParentSource(ISource<T> source);
    }
}