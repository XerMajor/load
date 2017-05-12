using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace Load
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Perform(args);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static void Perform(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("load timeout_ms CPU");
                Console.WriteLine("load timeout_ms RAM megabytes");
                return;
            }

            switch (args[1].ToLower())
            {
                case "cpu":
                    var threads = Environment.ProcessorCount*2;
                    int workerThreads;
                    int completionPortThreads;
                    ThreadPool.GetMinThreads(out workerThreads, out completionPortThreads);
                    ThreadPool.SetMinThreads(threads, completionPortThreads);

                    for (var i = 0; i < threads; i++)
                    {
                        Task.Factory.StartNew(() =>
                        {
                            while (true)
                            {
                            }
                        });
                    }

                    Thread.Sleep(int.Parse(args[0]));
                    break;
                case "ram":
                    var pointer = Marshal.AllocHGlobal(1024*1024*int.Parse(args[2]));
                    Thread.Sleep(int.Parse(args[0]));
                    Marshal.FreeHGlobal(pointer);
                    break;
            }
        }
    }
}
