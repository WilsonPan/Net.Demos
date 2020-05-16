using System;
using System.Linq;
using System.Collections.Generic;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace Demo.BenchmarkDotNet
{
    [MinColumn, MaxColumn]
    public class QuickSortVsHeapSort
    {
        [Benchmark]
        [ArgumentsSource(nameof(Data))]
        public void QuickSort(int[] nums) => Demo.BenchmarkDotNet.QuickSort.Sort(nums);

        //[Benchmark]
        [ArgumentsSource(nameof(Data))]
        public void HeapSort(int[] nums) => Demo.BenchmarkDotNet.HeapSort.Sort(nums);

        //[Benchmark(Baseline = true)]
        [ArgumentsSource(nameof(Data))]
        public void BubbleSort(int[] nums) => Demo.BenchmarkDotNet.BubbleSort.Sort(nums);
        public IEnumerable<int[]> Data()
        {
            var random = new Random();
            var datas = Enumerable.Range(1, 10000).ToArray();

            // 生成随机数组
            for (int i = datas.Length - 1; i > 0; i--)
            {
                datas[i] = random.Next(1, 10000000);
            }
            yield return datas.Take(100).ToArray();
            yield return datas.Take(1000).ToArray();
            yield return datas;
        }
    }
}