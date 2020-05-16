using System;

using Castle.DynamicProxy;

namespace Demo.DI
{
    public class LoggerInterceptor : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            Console.WriteLine($"{invocation.Arguments[0]}");
            Console.WriteLine($"{invocation.Method.Name}");

            invocation.Proceed();

            Console.WriteLine($"{invocation.ReturnValue}");

        }
    }
}