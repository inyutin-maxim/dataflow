using System;
using DataFlow;

namespace TestApp2
{
    class Program
    {
        static void Main()
        {
            DataSource dataSource;

            using (var applf = Lifetime.Define())
            {
                dataSource = new DataSource(applf.Lifetime);
                dataSource.IntegerComes += Console.WriteLine;

                dataSource.Trigger(12);  // Выведет в консоль 
            }

            dataSource.Trigger(666); // Пусто
        }
    }

    class DataSource : IDisposable
    {
        private readonly LifetimeDef _lfd;
        private Action<int> _integerComes;

        public DataSource(OuterLifetime outerLifetime)
        {
            _lfd = Lifetime.DefineDependent(outerLifetime);
        }

        public event Action<int> IntegerComes
        {
            add
            {
                _lfd.Lifetime.AddBracket(() => _integerComes += value, () => _integerComes -= value);
            }
            remove { _integerComes -= value; }
        }

        public void Trigger(int val)
        {
            OnIntegerComes(val);
        }

        protected virtual void OnIntegerComes(int obj)
        {
            _integerComes?.Invoke(obj);
        }

        public void Dispose()
        {
            _lfd.Terminate();
        }
    }
}
