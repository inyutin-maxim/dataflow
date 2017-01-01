using System;

namespace DataFlow
{
    /// <summary>
    /// Источник данных, на который можно подписаться
    /// </summary>
    public interface ISource<out T>
    {
        Lifetime Lifetime { get; }

        void Subscribe(Action<T> handler);

        void Subscribe(Action<T> handler, Lifetime lf);
    }

    /// <summary>
    /// Источник ping'ов, на который можно подписаться
    /// </summary>
    public interface IVoidSource
    {
        Lifetime Lifetime { get; }
        void Subscribe(Action handler);
    }

    /// <summary>
    ///
    /// </summary>
    public interface IMultipleParentSource<T> : ISource<T>
    {
        void AddParentSource(ISource<T> source);
    }
}