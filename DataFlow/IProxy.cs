namespace DataFlow
{
    public interface IProxy<T> : ITarget<T>, ISource<T>
    {
    }

    public interface IVoidProxy : IVoidTerget, IVoidSource
    {
    }
}