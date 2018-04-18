using System;
using DataFlow;

namespace TestApp0
{
    class Program
    {
        static void Main(string[] args)
        {
            // Глобальная область
            using (var lfd = Lifetime.Define())
            {
                var dataLayer = new ObjectsLayer(lfd);
                // ...
            }
        }
    }

    // Область контроля данных. Если предполагается что объект может быть 
    // разрушен отдельно от своей зависимости, вводим IDisposable
    class ObjectsLayer : IDisposable
    {
        private readonly LifetimeDef _lfd;

        public ObjectsLayer(OuterLifetime dependant)
        {
            _lfd = Lifetime.DefineDependent(dependant);
        }

        public void Dispose()
        {
            _lfd?.Dispose();
        }
    }
}
