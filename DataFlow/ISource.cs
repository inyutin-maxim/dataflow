using System;

namespace DataFlow
{
    public interface ISource<out T>
    {
        Lifetime Lifetime { get; }
        void Subscribe(Action<T> handler);
        void Subscribe(Action<T> handler, Lifetime lf);
    }

    public interface IVoidSource
    {
        Lifetime Lifetime { get; }
        void Subscribe(Action handler);
    }

    public interface IMultipleParentSource<in T>
    {
        void AddParentSource(ISource<T> source);
    }
}