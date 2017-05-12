using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Perfmon.Metrics
{
	public class PerformanceCounterPerSecond : IMetric
    {
	    private readonly object[] _arguments;

	    public PerformanceCounterPerSecond(params object[] arguments)
	    {
	        _arguments = arguments;
	    }

	    public double? GetValue()
        {
            var arguments = _arguments.Cast<string>().ToArray();
            PerformanceCounter c = null;
			try
			{
				c = new PerformanceCounter(arguments[0], arguments[1], arguments[2], arguments[3]);
				c.NextValue();
				Thread.Sleep(1000);
				return c.NextValue();
			}
			finally
			{
				if (c != null) c.Dispose();
				//PerformanceCounter.CloseSharedResources();
			}
		}
	}
}
