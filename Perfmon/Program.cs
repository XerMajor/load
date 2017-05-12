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
        static readonly Storage.Storage Db = new Storage.Storage();

        static void Main(string[] args)
        {
/*
select 
    (select min(Timestamp) from Cpu) [From],
    (select max(Timestamp) from Cpu) [To],
    (select avg(Value) from Cpu) Cpu,
    (select avg(Value) from Hdd) Hdd,
    (select avg(Value) from Free_RAM) Free_RAM,
    (select avg(Value) from Switches) Switches,
    (select sum(Value) from SQL_Locks) SQL_Locks,
    (select sum(Value) from SQL_Locks_Avg) SQL_Locks_Avg,
    (select sum(Value) from SQL_Deadlocks) SQL_Deadlocks,
    (select sum(Value) from ASP_Execution) ASP_Execution,
    (select sum(Value) from ASP_Wait) ASP_Wait,
    (select sum(Value) from ASP_Queued) ASP_Queued,
    (select sum(Value) from ASP_Errors) ASP_Errors
*/

            var counters = new Dictionary<string, ICounter>
            {
                {"Cpu", new Counter("Processor", "% Processor Time", "_Total")},
                {"Hdd", new Counter("PhysicalDisk", "Avg. Disk Queue Length", "_Total")},
                {"SQL_Locks_Avg", new CounterAvg("SQLServer:Locks", "Average Wait Time (ms)", "_Total", "Average Wait Time Base")},
                {"SQL_Locks", new Counter("SQLServer:Locks", "Lock Waits/Sec", "_Total", "Average Wait Time Base")},
                {"SQL_Deadlocks", new Counter("SQLServer:Locks", "Number of Deadlocks/sec", "_Total")},
                {"ASP_Execution", new Counter("ASP.NET", "Request Execution Time", null)},
                {"ASP_Wait", new Counter("ASP.NET", "Request Wait Time", null)},
                {"ASP_Queued", new Counter("ASP.NET", "Requests Queued", null)},
                {"ASP_Errors", new Counter("ASP.NET Applications", "Errors Total", "__Total__")},
                {"Free_RAM", new Counter("Memory", "Available MBytes", null)},
                {"Switches", new Counter("System", "Context Switches/sec", null)},
            };

            foreach (var module in ConfigurationManager.AppSettings["Modules"].ToStr().Split(','))
            {
                if (counters.ContainsKey(module))
                {
                    ThreadPool.QueueUserWorkItem(delegate
                    {
                        while (true)
                        {
                            try
                            {
                                var value = counters[module].GetValue();
                                Log.Instance.Info($"{module}: {value}");
                                Db.Set(DateTimeOffset.Now, module, value);
                            }
                            catch (Exception ex)
                            {
                                Log.Instance.Error($"{module} failed", ex);
                            }
                        }
                    });
                }
            }

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }
    }
}