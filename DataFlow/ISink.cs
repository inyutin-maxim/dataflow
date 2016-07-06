namespace DataFlow
{
    public interface ISink<in T>
    {
        void Fire(T value);
    }

    public interface IVoidSink
    {
        void Fire();      
    }
}