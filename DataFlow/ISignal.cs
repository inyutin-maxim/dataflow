namespace DataFlow
{
    public interface ISignal<T> : ISink<T>, ISource<T>
    {
    }

    public interface IVoidSignal : IVoidSink, IVoidSource
    {
    }
}