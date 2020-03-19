using RabbitMQ.Client;
using System;
using System.Text;

namespace NetasRabbitMQ.Publisher
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory();
            factory.HostName = "localhost";
            
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare("hello", false, false, false, null);

                    string message = "Hello Netas";

                    var bodyByte = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish("", routingKey: "hello", null, body:bodyByte);

                    Console.WriteLine("Mesajınız gönderilmiştir.");

                }

                Console.WriteLine("Çıkış yapmak için tıklayınız.");
                Console.ReadLine();
            }
        }
    }
}
