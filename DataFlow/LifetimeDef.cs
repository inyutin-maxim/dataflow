using System;
using System.Runtime.CompilerServices;

namespace DataFlow
{
    public class LifetimeDef : IDisposable
    {
        public Lifetime Lifetime { get; private set; }

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