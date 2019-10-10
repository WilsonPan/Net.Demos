using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            ConnectionFactory factory = new ConnectionFactory();
            factory.UserName = "dev";
            factory.Password = "dev";
            factory.VirtualHost = "dev";
            factory.HostName = "192.168.6.86";
            factory.Port = 5672;

            IConnection conn = factory.CreateConnection();

            var model = conn.CreateModel();

            byte[] messageBodyBytes = System.Text.Encoding.UTF8.GetBytes("Hello, world! " + DateTime.Now);
            model.BasicPublish("", "Wilson", null, messageBodyBytes);

            conn.Dispose();
            model.Dispose();

            Console.WriteLine("Finish");

            Console.Read();
        }
    }
}
