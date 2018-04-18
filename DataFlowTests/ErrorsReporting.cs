using System;
using DataFlow;

namespace DataFlowTests
{
    public class ErrorsReporting : IDisposable
    {
        private readonly LifetimeDef _lfd;
        private readonly Gate<int> _weigths, _heights, _salaries;

        public ErrorsReporting(OuterLifetime lf)
        {
            _lfd = Lifetime.DefineDependent(lf);
            _weigths = new Gate<int>(_lfd.Lifetime);
            _heights = new Gate<int>(_lfd.Lifetime);
            _salaries = new Gate<int>(_lfd.Lifetime);

            var wrongHeight = _weigths.Where(x => x > 250 || x < 0).Select(x => $"Wrong Height: {x}");
            var wrongWeight = _heights.Where(x => x > 250 || x < 0).Select(x => $"Wrong Weigth: {x}");
            var wrongSalary = _salaries.Where(x => x < 3000).Select(x => $"Wrong Salary: {x}");

            wrongSalary
                .Union(wrongHeight)
                .Union(wrongWeight)
                .Subscribe(x => Console.WriteLine($"  Message: {x}"));
        }

        public IGate<int> Weights => _weigths;

        public IGate<int> Heigths => _heights;

        public IGate<int> Salaries => _salaries;

        public void Dispose()
        {
            _lfd?.Dispose();
        }
    }
}