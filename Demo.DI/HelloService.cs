namespace Demo.DI
{
    public class HelloService : IHello
    {
        public string GetMessage(string name)
        {
            return $"{name} Welcome to Autofac";
        }
    }
}