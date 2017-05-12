using System.Diagnostics;
using System.Linq;

namespace Perfmon.Metrics
{
	public class PerformanceCounterAvg : IMetric
    {
	    private readonly object[] _arguments;

	    public PerformanceCounterAvg(params object[] arguments)
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
				c1 = new PerformanceCounter(arguments[0], arguments[1], arguments[2], arguments[3]);
				based = new PerformanceCounter(arguments[0], arguments[4], arguments[2], arguments[3]);
				var result = c1.RawValue/ based.RawValue;
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
