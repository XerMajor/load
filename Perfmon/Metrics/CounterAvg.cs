using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Perfmon.Metrics
{
	public class CounterAvg : ICounter
    {
	    private readonly object[] _arguments;

	    public int Interval = 1000;

        public string Machine = "localhost";

        public CounterAvg(params object[] arguments)
	    {
	        _arguments = arguments;
	    }

	    public double? GetValue()
        {
            var arguments = _arguments.Cast<string>().ToArray();
            PerformanceCounter c1 = null;
            PerformanceCounter based = null;
			try
			{
				c1 = new PerformanceCounter(arguments[0], arguments[1], arguments[2], Machine);
				based = new PerformanceCounter(arguments[0], arguments[3], arguments[2], Machine);
			    var result = c1.RawValue/based.RawValue;
                Thread.Sleep(Interval);
                return result;
			}
			finally
			{
				if (c1 != null) c1.Dispose();
				if (based != null) based.Dispose();
				//PerformanceCounter.CloseSharedResources();
			}
		}
	}
}
