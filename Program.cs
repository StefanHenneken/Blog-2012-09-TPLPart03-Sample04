using System;
using System.Threading;
using System.Threading.Tasks;

namespace Sample04
{
    class Program
    {
        static void Main(string[] args)
        {
            new Program().Run();
        }
        public void Run()
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            CancellationToken ct = cts.Token;
            ct.Register(() => { Console.WriteLine("Token is canceled"); });

            Task task1 = Task.Factory.StartNew((ct1) =>
            {
                Console.Write("Task1 Start");
                CancellationToken ct1Local = (CancellationToken)ct1;
                int x = 0;
                try
                {
                    while (x++ < 5)
                    {
                        Console.Write(".");
                        Thread.Sleep(500);
                        ct1Local.ThrowIfCancellationRequested();
                    }
                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine("Task1 OperationCanceledException");
                    throw;
                }
                Console.WriteLine("Task1 End");
            }, ct, ct);

            Task task2 = task1.ContinueWith((firstTask, ct2) =>
            {
                Console.Write("Task2 Start");
                CancellationToken ct2Local = (CancellationToken)ct2;
                int x = 0;
                try
                {
                    while (x++ < 5)
                    {
                        Console.Write(".");
                        Thread.Sleep(500);
                        ct2Local.ThrowIfCancellationRequested();
                    }
                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine("Task2 OperationCanceledException");
                    throw;
                }
                Console.WriteLine("Task2 End");
            }, ct, ct);

            Console.ReadLine();
            cts.Cancel();
            Thread.Sleep(1000);
            Console.WriteLine("Status task1: {0}", task1.Status);
            Console.WriteLine("Status task2: {0}", task2.Status);
            Console.ReadLine();
        }
    }
}
