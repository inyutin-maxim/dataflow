using System;
using DataFlow;

namespace DataFlowTests
{
    class Person : IDisposable
    {
        private readonly LifetimeDef _lfd;

        public Person(OuterLifetime lf)
        {
            _lfd = Lifetime.DefineDependent(lf);
            Height = Property<int>.Create(_lfd.Lifetime);
            Weigth = Property<int>.Create(_lfd.Lifetime);
            Salary = Property<int>.Create(_lfd.Lifetime);
        }

        public Property<int> Height { get; }

        public Property<int> Weigth { get; }

        public Property<int> Salary { get; }

        public void Dispose()
        {
            _lfd.Terminate();
        }
    }
}