using System;
using System.Linq;
using System.Collections.Generic;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Engines;

namespace Demo.BenchmarkDotNet
{
    public class QuickSortVsHeapSort
    {
        private int[] _datas;
        public QuickSortVsHeapSort()
        {
            _datas = Enumerable.Range(1, 100).ToArray();
            var random = new Random();
            // 打乱数组
            for (int i = _datas.Length - 1; i > 0; i--)
            {
                var randomIndex = random.Next(0, i - 1);
                _datas[i] = _datas[i] + _datas[randomIndex];
                _datas[randomIndex] = _datas[i] - _datas[randomIndex];
            }
        }
        [Benchmark]
        [ArgumentsSource(nameof(Data))]
        public void HeapSort(int[] nums) => Demo.BenchmarkDotNet.HeapSort.Sort(nums);

        [Benchmark]
        [ArgumentsSource(nameof(Data))]
        public void QuickSort(int[] nums) => Demo.BenchmarkDotNet.QuickSort.Sort(nums, 0, nums.Length - 1);

        //[Benchmark(Baseline = true)]
        [ArgumentsSource(nameof(Data))]
        public void BubbleSort(int[] nums) => Demo.BenchmarkDotNet.BubbleSort.Sort(nums);

        public IEnumerable<int[]> Data()
        {
            //yield return _datas.Take(100).ToArray();
            //yield return _datas.Take(1000).ToArray();
            yield return _datas;
        }
    }
}