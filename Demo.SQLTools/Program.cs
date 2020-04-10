using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Demo.SQLTools
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            // var hexString = "7f7ece03 ea400b05";

            // Console.WriteLine($"Hex String : {hexString}");

            // Console.WriteLine(DecodeText(hexString, System.Text.Encoding.GetEncoding("gb2312")));

            // var hexString = "57696c 736f6eb9 e3d6ddca d0ccecba d3c7f8";

            // Console.WriteLine($"Hex String : {hexString}");

            // Console.WriteLine(DecodeText(hexString, System.Text.Encoding.GetEncoding("gb2312")));

            var hexString = "416368 6f6e67b9 e3cef7ca a1d3f1c1 d6cad03000100002 0000006b 0d5386af ed400b04 00000200 1e002a00";
            Console.WriteLine(hexString.Replace(" ", "").Length);
            Console.WriteLine($"Hex String : {hexString}");

            Console.WriteLine("Convert Int : " + BitConverter.ToInt32(HexToBytes(hexString), 0));
            Console.WriteLine("Convert Text : " + DecodeText(hexString, System.Text.Encoding.GetEncoding("gb2312")));
            Console.WriteLine("Convert DateTime : " + DecodeDateTime(hexString, 7));

        }

        static byte[] HexToBytes(string hexString)
        {
            hexString = hexString.StartsWith("0x") ? hexString.Substring(2) : hexString;
            hexString = Regex.Replace(hexString, @"\s*", "", RegexOptions.Singleline);

            var data = new byte[hexString.Length / 2];

            for (var i = 0; i < data.Length; i++)
            {
                data[i] = byte.Parse(hexString.Substring(i * 2, 2),
                                               NumberStyles.AllowHexSpecifier,
                                               CultureInfo.InvariantCulture);
            }
            return data;
        }
        static string DecodeDateTime(string hexString, int scale = 7)
        {
            var data = HexToBytes(hexString);

            var dateData = new byte[4];
            var timeData = new byte[8];

            var scaleFactor = 1000F / (float)Math.Pow(10, scale);

            Array.Copy(data, timeData, data.Length - 3);
            Array.Copy(data, data.Length - 3, dateData, 0, 3);

            var datePart = BitConverter.ToInt32(dateData, 0);
            var timePart = BitConverter.ToInt64(timeData, 0);

            var returnDate = new DateTime(0001, 01, 01);
            returnDate = returnDate.AddDays(datePart);
            returnDate = returnDate.AddMilliseconds(scaleFactor * timePart);

            return returnDate.ToString("yyyy-MM-dd HH:mm:ss.fff");
        }
        static string DecodeText(string hexString, System.Text.Encoding encoding)
        {
            return encoding.GetString(HexToBytes(hexString));
        }
    }
}
