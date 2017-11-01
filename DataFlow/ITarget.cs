namespace DataFlow
{
    public interface ITarget<in T>
    {
        void Fire(T value);
    }

    public interface IVoidTerget
    {
        void Fire();      
    }
}