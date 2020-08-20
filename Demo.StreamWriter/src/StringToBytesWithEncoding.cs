using System;
using System.Text;

namespace Demo.StreamWriter
{
    public static class StringToBytesWithEncoding
    {
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