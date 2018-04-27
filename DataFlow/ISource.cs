namespace DataFlow
{
    public interface ISource<in T>
    {
        void Fire(T value);
    }

    public interface IVoidSource
    {
        void Fire();
    }
}