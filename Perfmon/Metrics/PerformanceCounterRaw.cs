using System.Diagnostics;
using System.Linq;

namespace Perfmon.Metrics
{
	public class PerformanceCounterRaw: IMetric
	{
	    private readonly object[] _arguments;

	    public PerformanceCounterRaw(params object[] arguments)
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
				var result = c.RawValue;
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
