using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Running;

namespace Demo.BenchmarkDotNet
{
    class Program
    {
        static void Main(string[] args)
        {
            //var summary = BenchmarkRunner.Run<QuickSortVsHeapSort>();

            var random = new Random();
            var datas = Enumerable.Range(1, 10000).ToArray();
            for (int i = datas.Length - 1; i > 0; i--)
            {
                datas[i] = random.Next(1, 10000000);
            }

            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();

            sw.Start();

            Demo.BenchmarkDotNet.QuickSort.Sort(datas.Clone() as int[]);

            Console.WriteLine($"QuickSort : {sw.ElapsedMilliseconds}");

            sw.Restart();
            Demo.BenchmarkDotNet.BubbleSort.Sort(datas.Clone() as int[]);

            Console.WriteLine($"BubbleSort : {sw.ElapsedMilliseconds}");

            sw.Restart();
            Demo.BenchmarkDotNet.HeapSort.Sort(datas.Clone() as int[]);

            Console.WriteLine($"HeapSort : {sw.ElapsedMilliseconds}");

        }
    }
}
