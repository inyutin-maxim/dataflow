using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace System.Contracts
{
    [DebuggerDisplay("LifetimeDef `{" + nameof(Name) + "}` ")]
    public class LifetimeDef : IDisposable
    {
        public Lifetime Lifetime { get; }
        public string Name { get; }

        private const string Noname = "Unnamed";
        
        public LifetimeDef(string name = null)
        {
            Name = name ?? Noname;
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