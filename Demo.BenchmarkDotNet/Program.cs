using System;
using BenchmarkDotNet.Running;

namespace Demo.BenchmarkDotNet
{
    class Program
    {
        static void Main(string[] args)
        {
            var appDomain = System.Threading.Thread.GetDomain();

            var ad = AppDomain.CreateDomain("Ad #2", null, null);

            var asy = System.Reflection.Assembly.LoadFile(@"F:\GitHub\Me\Net.Demos\Demo.Hot\bin\Debug\netstandard2.0\Demo.Hot.dll");

            var obj = ad.CreateInstanceAndUnwrap(asy.FullName, "Demo.Hot.Runner");

            var method = obj.GetType().GetMethod("Execute");

            var returnValue = method.Invoke(obj, null);

            Console.WriteLine(returnValue);

            foreach (var assembly in ad.GetAssemblies())
            {
                Console.WriteLine($"{assembly.FullName}");
            }

            AppDomain.Unload(ad);

        }
    }

    public class Person : MarshalByRefObject
    {
        public string Name { get; set; }

        public string GetName()
        {
            return "Wilson Pan";
        }
    }
}
