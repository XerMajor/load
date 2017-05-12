using System;
using System.Diagnostics;
using System.Threading;
using Shamsullin.Common.Extensions;

namespace Perfmon.Metrics
{
	public class Counter : ICounter
	{
	    private PerformanceCounter _counter;

	    private readonly Func<PerformanceCounter> _creator;

        public int Interval = 1000;

        public string Machine = "localhost";

        public Counter(params object[] arguments)
	    {
            _creator = () => new PerformanceCounter(arguments[0].ToStr(), arguments[1].ToStr(), arguments[2].ToStr(), Machine);
        }

	    public double? GetValue()
        {
            if (_counter == null) _counter = _creator();
            var result = _counter.NextValue();
            Thread.Sleep(Interval);
            return result;
        }
	}
}
