using System;
using BenchmarkDotNet;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using Demo.StreamWriter;

namespace BenchmarksTests
{
    [SimpleJob(launchCount: 10)]
    [MemoryDiagnoser]
    public class StreamVsEncoding
    {
        [Params("Hello Wilson!", "使用【BenchmarkDotNet】基准测试，Encoding vs Stream")]
        public string _stringValue;

        [Benchmark]
        public void Encoding() => StringToBytesWithEncoding.StringToBytes(_stringValue);

        [Benchmark]
        public void Stream() => StringToBytesWithStream.StringToBytes(_stringValue);

    }
}
