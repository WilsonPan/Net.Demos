using System;

namespace Demo.StreamWriter
{
    public static class StringToBytesWithStream
    {
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