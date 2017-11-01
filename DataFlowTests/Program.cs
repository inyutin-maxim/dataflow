using System;
using System.Collections.Generic;
using DataFlow;

namespace DataFlowTests
{
    class Program
    {
        static void Main()
        {
            var persons = new List<Person>();
            var personsLifetimes = new List<LifetimeDef>();

            var gatesLifetime = Lifetime.Define().Lifetime;

            var weightsGate = new Gate<int>(gatesLifetime);
            var heightsGate = new Gate<int>(gatesLifetime);
            var salariesGate = new Gate<int>(gatesLifetime);

            for (int i = 0; i < 10; i++)
            {
                var def = Lifetime.Define();
                var lifetime = def.Lifetime;
                var person = new Person(lifetime);

                weightsGate.AddParentSource(person.Weigth.Changed);
                heightsGate.AddParentSource(person.Height.Changed);
                salariesGate.AddParentSource(person.Salary.Changed);

                persons.Add(person);
                personsLifetimes.Add(def);
            }                                       

            var wrongHeight = weightsGate.Where(x => x > 250 || x < 0).Select(x => $"Wrong Height: {x}");
            var wrongWeight = heightsGate.Where(x => x > 250 || x < 0).Select(x => $"Wrong Weigth: {x}");
            var wrongSalary = salariesGate.Where(x => x < 3000).Select(x => $"Wrong Salary: {x}");

            wrongSalary
                .Union(wrongHeight)
                .Union(wrongWeight)
                .Subscribe(x => Console.WriteLine($"Message: {x}"));
                        
            Console.WriteLine("=================");
            PushChanges(persons);

            personsLifetimes[2].Terminate();
            personsLifetimes[5].Terminate();
            personsLifetimes[7].Terminate();

            Console.WriteLine("=================");
            PushChanges(persons);

            Console.ReadKey();
        }

        private static void PushChanges(List<Person> persons)
        {
            foreach (var i in new[] {0, 2, 5, 7, 9})
            {
                Console.WriteLine($"Changing {i} person");
                persons[i].Height.Value = 0;
                persons[i].Height.Value = 20;
                persons[i].Height.Value = 200;
                persons[i].Height.Value = 2000;

                persons[i].Weigth.Value = 0;
                persons[i].Weigth.Value = 20;
                persons[i].Weigth.Value = 200;
                persons[i].Weigth.Value = 2000;

                persons[i].Salary.Value = 0;
                persons[i].Salary.Value = 20;
                persons[i].Salary.Value = 200;
                persons[i].Salary.Value = 4000;
            }
        }

        class Person : IDisposable
        {
            private readonly LifetimeDef _lfd;

            public Person(Lifetime lf)
            {
                _lfd = Lifetime.DefineDependent(lf);
                Height = new Property<int>(_lfd.Lifetime);
                Weigth = new Property<int>(_lfd.Lifetime);
                Salary = new Property<int>(_lfd.Lifetime);
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
}