using System;
using System.Contracts;

namespace TestApp2
{
    class Program
    {
        static void Main()
        {
            DataSource dataSource;

            using (var applf = Lifetime.Define("Root"))
            {
                dataSource = new DataSource(applf.Lifetime);
                dataSource.IntegerComes += Console.WriteLine;
                dataSource.Trigger(12);  // Выведет в консоль 
            }

            dataSource.Trigger(666); // Пусто
            Console.ReadKey();
        }
    }
}
