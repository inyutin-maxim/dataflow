using System;
using System.Contracts;
using DataFlow;

namespace TestApp1
{
    class Program
    {
        static void Main()
        {
            DataSource dataSource;

            using (var applife = Lifetime.Define())
            {
                dataSource = new DataSource(applife.Lifetime);
                dataSource.X.Changed.Select(x => $"X: {x}").Subscribe(Console.WriteLine); 
                dataSource.Y.Changed.Select(x => $"Y: {x}").Subscribe(Console.WriteLine);

                // где-то в другом месте
                dataSource.X.Value = 34;  // должно напечатать X: 34
                dataSource.Y.Value = 12;  // должно напечатать Y: 12
            }

            dataSource.X.Value = 666;  // не должно ничего напечатать
            dataSource.Y.Value = 999;  // не должно ничего напечатать
            Console.ReadKey();
        }
    }

    class DataSource
    {
        public DataSource(OuterLifetime outerLifetime)
        {
            var lf = Lifetime.DefineDependent(outerLifetime).Lifetime;
            X = Property<decimal>.Create(lf);
            Y = Property<decimal>.Create(lf);
        }

        public Property<decimal> X { get; }

        public Property<decimal> Y { get; }
    }
}
