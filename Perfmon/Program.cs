using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading;
using Perfmon.Metrics;
using Shamsullin.Common;
using Shamsullin.Common.Extensions;

namespace Perfmon
{
    class Program
    {
        static readonly Database Db = new Database();

        static void Main(string[] args)
        {
            var interval = args.Length > 1 ? args[1].To(1000) : 1000;
            var machine = args.Length > 2 ? args[2].To("localhost") : "localhost";

            var counters = new Dictionary<string, IMetric>
            {
                {"Cpu", new PerformanceCounterPerSecond("Processor", "% Processor Time", "_Total", machine)},
                {"Hdd", new PerformanceCounterPerSecond("PhysicalDisk", "Avg. Disk Queue Length", "_Total", machine)},
                {"SQL_Locks", new PerformanceCounterAvg("SQLServer:Locks", "Average Wait Time (ms)", "_Total", machine, "Average Wait Time Base")},
                {"SQL_Deadlocks", new PerformanceCounterPerSecond("SQLServer:Locks", "Number of Deadlocks/sec", "_Total", machine)},
            };

            foreach (var module in ConfigurationManager.AppSettings["Modules"].ToStr().Split(','))
            {
                new AsyncManager(delegate
                {
                    while (true)
                    {
                        try
                        {
                            var value = counters[module].GetValue();
                            Log.Instance.Info($"{module}: {value}");
                            Db.Set(DateTimeOffset.Now, module, value);
                            Thread.Sleep(interval);
                        }
                        catch (Exception ex)
                        {
                            Log.Instance.Error($"{module} failed", ex);
                        }
                    }
                }).ExecuteAsync();
            }

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }
    }
}