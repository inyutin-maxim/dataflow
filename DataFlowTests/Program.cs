using System;
using DataFlow;

namespace DataFlowTests
{
    class Program
    {
        static void Main()
        {    
            /*
            #region lifetimeCheck
            var define1 = Lifetime.Define();
            var define2 = Lifetime.Define();

            var life1 = define1.Lifetime;
            var life2 = define2.Lifetime;

            life1.Add(() => Console.WriteLine("Hello! 1"));
            life2.Add(() => Console.WriteLine("Hello! 2"));

            var inters = life1.Intersect(life2);
            inters.Add(() => Console.WriteLine("All done"));

            ISignal<int> signal1 = new Signal<int>(life1);
            ISignal<int> signal2 = new Signal<int>(life2);

            var signal1Filter = signal1.Where(x => x > 1 && x < 4);
            var signal2Filter = signal2.Where(x => x > 1 && x < 4);

            var zipped = signal1Filter.Zip(signal2Filter).Select(x => $"in: {x}");

            zipped.Subscribe(Console.WriteLine);

            signal1.Fire(1);
            signal1.Fire(2);
            signal1.Fire(3);
            signal1.Fire(4);

            define1.Terminate();

            signal2.Fire(1);
            signal2.Fire(2);
            signal2.Fire(3);
            signal1.Fire(2);   // should be ignored
            signal1.Fire(3);   // should be ignored
            signal2.Fire(4);

            define2.Terminate();
            #endregion
            */

            var personLifetime = Lifetime.Define().Lifetime;

            var person = new Person(personLifetime);
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