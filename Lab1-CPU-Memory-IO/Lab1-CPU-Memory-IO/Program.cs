using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Lab1_CPU_Memory_IO
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            int threadsCount = 100;
            long executionTimeMs1 = CalculateExecutionTimeMs(() =>
            {
                for (int index = 0; index < threadsCount; ++index)
                    DoComplexCalculation();
            });
            long executionTimeMs2 = CalculateExecutionTimeMs(async () =>
            {
                List<Task> cpuBoundTasks = new List<Task>(threadsCount);
                for (int i = 0; i < threadsCount; ++i)
                    cpuBoundTasks.Add(DoCpuBoundComplexCalculation());
                await Task.WhenAll(cpuBoundTasks);
            });
            long executionTimeMs3 = CalculateExecutionTimeMs(() =>
            {
                for (int index = 0; index < threadsCount; ++index)
                    DoIoBoundOperation();
            }));
            long executionTimeMs4 = CalculateExecutionTimeMs(async () =>
            {
                List<Task> ioBoundTasks = new List<Task>(threadsCount);
                for (int i = 0; i < threadsCount; ++i)
                    ioBoundTasks.Add(DoIoBoundOperationAsync());
                await Task.WhenAll(ioBoundTasks);
            });
            Console.WriteLine(string.Format("Time of complex calculation CPU bound synchronous execution: {0} ms", (object)executionTimeMs1));
            Console.WriteLine(string.Format("Time of complex calculation CPU bound asynchronous execution: {0} ms", (object)executionTimeMs2));
            Console.WriteLine(string.Format("Time of IO bound task synchronous execution: {0} ms", (object)executionTimeMs3));
            Console.WriteLine(string.Format("Time of IO bound task asynchronous execution: {0} ms", (object)executionTimeMs4));
        }

        private static Task DoCpuBoundComplexCalculation() => Task.Run(DoComplexCalculation);

        private static void DoComplexCalculation()
        {
            for (int a = 0; a < 99999999; ++a)
                Math.Sin(a);
        }

        private static void DoIoBoundOperation() => File.ReadAllText(
            Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName + "\\data.txt");

        private static Task DoIoBoundOperationAsync() => File.ReadAllTextAsync(
            Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName + "\\data.txt");

        private static long CalculateExecutionTimeMs(Action operation)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            operation();
            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
        }
    }
}
