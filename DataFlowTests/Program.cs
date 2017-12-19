using System;
using System.Collections.Generic;
using System.Configuration;
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
                var errReporting = new ErrorsReporting(applf);
                
                // Создаем 10 персон с выделением каждой своего отрезка жизни
                for (int i = 0; i < 10; i++)
                {
                    var person = new Person(applf);

                    person.Weigth.Changed.ReportTo(errReporting.Weights);
                    person.Height.Changed.ReportTo(errReporting.Heigths);
                    person.Salary.Changed.ReportTo(errReporting.Salaries);

                    persons.Add(person);
                }

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
                persons[i].Salary.Value = i*1000;
                Console.WriteLine($"Stopped changing {i} person");
            }
        }
    }
}