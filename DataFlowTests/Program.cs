using System;
using DataFlow;

namespace DataFlowTests
{
    class Program
    {
        static void Main()
        {        
            var lifetime = Lifetime.Define().Lifetime;  

            var person = new Person(lifetime);
                                                                         
                         
            var wrongHeight = person.Height.Changed.Where(x => x > 250 || x < 0).Select(x => $"Wrong Height: {x}");
            var wrongWeight = person.Weigth.Changed.Where(x => x > 250 || x < 0).Select(x => $"Wrong Weigth: {x}");
            var wrongSalary = person.Salary.Changed.Where(x => x < 3000).Select(x => $"Wrong Salary: {x}");

            wrongSalary.Zip(wrongHeight).Zip(wrongWeight).Subscribe(x => Console.WriteLine($"Message: {x}"));
          

            person.Height.Value = 0;
            person.Height.Value = 20;
            person.Height.Value = 200;
            person.Height.Value = 2000;

            person.Weigth.Value = 0;
            person.Weigth.Value = 20;
            person.Weigth.Value = 200;
            person.Weigth.Value = 2000;

            person.Salary.Value = 0;
            person.Salary.Value = 20;
            person.Salary.Value = 200;
            person.Salary.Value = 4000;

            Console.ReadKey();
        }

        class Person
        {
            private Lifetime _lf;

            public Person(Lifetime lf)
            {
                _lf = lf;
                Height = new Property<int>(lf);
                Weigth = new Property<int>(lf);
                Salary = new Property<int>(lf);
            }

            public Property<int> Height { get; }
            public Property<int> Weigth { get; }
            public Property<int> Salary { get; } 
        }
    }
}