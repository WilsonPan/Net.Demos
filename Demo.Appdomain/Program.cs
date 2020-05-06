using System;

namespace Demo.Appdomain
{
    class Program
    {
        static void Main(string[] args)
        {
    
            Console.WriteLine("Hello World!");

            Console.Read();
        }
    }

    [Serializable]
    public class Person
    {
        public string Name { get; set; }
    }
}
