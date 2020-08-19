using System;
using System.Text;
using BenchmarkDotNet.Attributes;

namespace Demo.StreamWriter
{
    public class StringToBytesWithEncoding
    {
        [Benchmark]
        [Arguments("Hello World! Wilson , 深度了解咖啡")]
        public static byte[] StringToBytes(string value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));

            var bytes = System.Text.Encoding.UTF8.GetBytes(value);

            var result = new byte[bytes.Length + 3];

            Array.Copy(Encoding.UTF8.GetPreamble(), result, 3);
            Array.Copy(bytes, 0, result, 3, bytes.Length);

            return result;
        }
    }
}