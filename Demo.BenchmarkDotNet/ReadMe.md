# BenchmarkDotNet 概述

---
> **BenchmarkDotNet** helps you to transform methods into benchmarks, track their performance, and share reproducible measurement experiments. It's no harder than writing unit tests

提取几个关键字（其实是只认识那几个英文单词）

- 将方法转换基准测试
- 跟踪性能
- 可重复实验
- 不必单元测试难

说白了，就是代码的性能测试，通常是用来比较两段代码/方法，或者在不同平台上的执行效果。

## BenchmarkDotNet 快速入门

---

1. 添加包

```cs
dotnet add package BenchmarkDotNet
```

2. 添加需要基准测试的方法（这里我准备两个排序算法，快速排序 && 堆排序）
   
```cs
[Benchmark]
[Arguments(new int[] { 3, 1, 10, 9, 6, 2, 5, 7, 8, 4 })]
public void QuickSort(int[] nums) => Demo.BenchmarkDotNet.QuickSort.Sort(nums);

[Benchmark]
[Arguments(new int[] { 3, 1, 10, 9, 6, 2, 5, 7, 8, 4 })]
public void HeapSort(int[] nums) => Demo.BenchmarkDotNet.HeapSort.Sort(nums);
```

3. Main里执行BenchmarkRunner.Run
  
```cs
var summary = BenchmarkRunner.Run<QuickSortVsHeapSort>();
```

4. 执行（需要Release模式）
   
```cs
dotnet run -c=Release
```

5. 分析结果

```cs
BenchmarkDotNet=v0.12.1, OS=Windows 10.0.18363.778 (1909/November2018Update/19H2)
Intel Core i7-10510U CPU 1.80GHz, 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=3.1.202
  [Host]     : .NET Core 3.1.4 (CoreCLR 4.700.20.20201, CoreFX 4.700.20.22101), X64 RyuJIT
  DefaultJob : .NET Core 3.1.4 (CoreCLR 4.700.20.20201, CoreFX 4.700.20.22101), X64 RyuJIT


|    Method |      nums |     Mean |    Error |   StdDev |
|---------- |---------- |---------:|---------:|---------:|
| QuickSort | Int32[10] | 61.98 ns | 0.242 ns | 0.202 ns |
|  HeapSort | Int32[10] | 89.19 ns | 0.374 ns | 0.332 ns |
```
除了控制台，还可以在```BenchmarkDotNet.Artifacts/result```找到多种格式的输出结果

可以看到QuickSort ，HeapSort比较接近，但是我们测试的数据量太少，所以这个没代表性

5. 总结

可以看到BenchmarkDotNet对原来的代码是没有侵入式，通常我是新建一个测试类，然后再测试类初始化测试参数，这样对原来代码没有侵入

## 进阶用法

### 多组输入参数

```cs
[Benchmark]
[ArgumentsSource(nameof(Data))]
public void QuickSort(int[] nums) => Demo.BenchmarkDotNet.QuickSort.Sort(nums);

public IEnumerable<int[]> Data()
{
    var random = new Random();
    var datas = Enumerable.Range(1, 10000).ToArray();
    // 打乱数组
    for (int i = datas.Length - 1; i > 0; i--)
    {
        var value = datas[i];
        var randomIndex = random.Next(0, i);
        datas[i] = datas[randomIndex];
        datas[randomIndex] = value;
    }
    yield return datas.Take(100).ToArray();
    yield return datas.Take(1000).ToArray();
    yield return datas;
}
```

```ArgumentsSource``` : 参数可以是方法/属性的名称

### 多平台比较

1. 在基准测试类中添加SimpleJob

```cs
[SimpleJob(RuntimeMoniker.NetCoreApp31)]
[SimpleJob(RuntimeMoniker.Net472)]
public class QuickSortVsHeapSort
{
}
```

2. 项目方案添加多个运行时
  
```xml
<TargetFrameworks>netcoreapp3.1;net472</TargetFrameworks>
```

### 添加统计字段

在基准测试类添加```MaxColumn``` , ```MinColumn```,```MemoryDiagnoser```

```cs
[MaxColumn, MinColumn, MemoryDiagnoser]
public class QuickSortVsHeapSort
{
  ...
}
```

### 添加基准

比较快速排序和堆排序，可以用其中一个作为基准，也可以新增一个作为基准作为参考。例如这里选择以冒泡排序作为基准 ，下图是各个排序算法的时间复杂度

排序 | 平均情况 | 最坏情况 | 最好情况 | 空间复杂度 
---|---|---|---|---
冒泡排序 |  O($n^2$) | O($n^2$) |  O($n^2$) | O(1)
快速排序 | O(n$\log_2n$) |  O($n^2$) |  O(n$\log_2n$) | O(n$\log_2n$)
堆排序 | O(n$\log_2n$) | O(n$\log_2n$) | O(n$\log_2n$) | O(1)

```cs
[Benchmark(Baseline = true)]
[ArgumentsSource(nameof(Data))]
public void BubbleSort(int[] nums) => Demo.BenchmarkDotNet.BubbleSort.Sort(nums);
```

### 使用BenchmarkDotNet 模板

1. 安装模板
   
``` cs
dotnet new -i BenchmarkDotNet.Templates
```

2. 创建模板

``` cs
dotnet new benchmark
```


### 使用BenchmarkDotNet dotnet tool

1. 安装
  
``` cs
dotnet tool install -g BenchmarkDotNet.Tool
```

2. 使用
  
``` cs
dotnet benchmark [arguments] [options]
```

# 