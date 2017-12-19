using System;
using System.Collections.Generic;
using DataFlow;

namespace DataFlowTests
{
    class Program
    {
        static void Main()
        {
            using (var applf = Lifetime.Define())
            {
                var persons = new List<Person>();
                var personsLifetimes = new List<LifetimeDef>();
                var gatesLifetime = Lifetime.DefineDependent(applf).Lifetime;

                // Создаем шлюзы для групповых нотификаций
                var weightsGate = new Gate<int>(gatesLifetime);
                var heightsGate = new Gate<int>(gatesLifetime);
                var salariesGate = new Gate<int>(gatesLifetime);

                // Создаем 10 персон с выделением каждой своего отрезка жизни
                for (int i = 0; i < 10; i++)
                {
                    var person = new Person(applf);

                    weightsGate.AddParentSource(person.Weigth.Changed);
                    heightsGate.AddParentSource(person.Height.Changed);
                    salariesGate.AddParentSource(person.Salary.Changed);

                    persons.Add(person);
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

                Console.WriteLine("Killing: 2th, 5th, 7th");
                persons[2].Dispose();
                persons[5].Dispose();
                persons[7].Dispose();

                Console.WriteLine("=================");
                PushChanges(persons);

                Console.ReadKey();
            }
        }

        private static void PushChanges(List<Person> persons)
        {
            foreach (var i in new[] {0, 2, 5, 7, 9})
            {
                Console.WriteLine($"Changing {i} person");
                persons[i].Height.Value = i;
                persons[i].Height.Value = i*10;
                persons[i].Height.Value = i*100;
                persons[i].Height.Value = i*1000;

                persons[i].Weigth.Value = i;
                persons[i].Weigth.Value = i*10;
                persons[i].Weigth.Value = i*100;
                persons[i].Weigth.Value = i*1000;

                persons[i].Salary.Value = i;
                persons[i].Salary.Value = i*10;
                persons[i].Salary.Value = i*100;
                persons[i].Salary.Value = i*1000 + 1;
            }
        }                                           

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
}