namespace System.Contracts
{
    public class LifetimeDef : IDisposable
    {
        public Lifetime Lifetime { get; }

        public LifetimeDef()
        {
            Lifetime = new Lifetime();
        }

        public void Terminate()
        {
            Lifetime.Terminate();
        }

        public void Dispose()
        {
            Terminate();
        }
    }
}