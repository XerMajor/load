using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Perfmon.Metrics
{
	public class CounterRaw: ICounter
	{
	    private readonly object[] _arguments;

        public int Interval = 1000;

        public string Machine = "localhost";

        public CounterRaw(params object[] arguments)
	    {
	        _arguments = arguments;
	    }

	    public double? GetValue()
		{
			var arguments = _arguments.Cast<string>().ToArray();
            PerformanceCounter c = null;
			try
			{
				c = new PerformanceCounter(arguments[0], arguments[1], arguments[2], Machine);
				var result = c.RawValue;
                Thread.Sleep(Interval);
                return result;
			}
			finally
			{
				if (c != null) c.Dispose();
				//PerformanceCounter.CloseSharedResources();
			}
		}
	}
}
