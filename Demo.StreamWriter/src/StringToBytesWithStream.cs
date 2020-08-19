using System;
using BenchmarkDotNet.Attributes;

namespace Demo.StreamWriter
{
    public class StringToBytesWithStream
    {
        [Benchmark]
        [Arguments("Hello World! Wilson , 深度了解咖啡")]
        public static byte[] StringToBytes(string value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));

            using (var ms = new System.IO.MemoryStream())
            using (var streamWriter = new System.IO.StreamWriter(ms, System.Text.Encoding.UTF8))
            {
                streamWriter.Write(value);
                streamWriter.Flush();

                return ms.ToArray();
            }
        }
    }
}