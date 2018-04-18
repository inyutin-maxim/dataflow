namespace DataFlow
{
    public interface ISourceAdapter<T> : ISource<T>, ITarget<T>
    {
    }

    public interface IVoidSourceAdapter : IVoidSource, IVoidTarget
    {
    }
}