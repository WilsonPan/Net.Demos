using System;
using Autofac;
using Autofac.Extras.DynamicProxy;

namespace Demo.DI
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<HelloService>().As<IHello>().EnableInterfaceInterceptors().InterceptedBy(typeof(LoggerInterceptor));
            builder.RegisterType<LoggerInterceptor>();

            var ioc = builder.Build();

            var hello = ioc.Resolve<IHello>();

            Console.WriteLine(hello.GetMessage("Wilson"));

        }
    }
}
