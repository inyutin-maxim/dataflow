using System;

namespace DataFlow
{
    public interface ISource<out T>
    {
        Lifetime Lifetime { get; }
        void Subscribe(Action<T> handler);
    }

    public interface IVoidSource
    {
        Lifetime Lifetime { get; }
        void Subscribe(Action handler);
    }
}