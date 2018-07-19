using RabbitMQ.Client;
using System;
using System.Text;
using System.Threading.Tasks;

namespace HelloWorld.Publisher
{
    class EmitLog
    {
        static void Main(string[] args)
        {
            Console.WriteLine(" [*] Program started, enter empty line to exit.");

            var factory = new ConnectionFactory { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare(exchange: "logs", type: "fanout");
                    
                    String message;
                    Console.WriteLine("Write your first message: ");
                    while (!String.IsNullOrWhiteSpace(message = Console.ReadLine()))
                    {
                        var body = Encoding.UTF8.GetBytes(message);
                        
                        channel.BasicPublish(exchange: "logs",
                                            routingKey: "",
                                            basicProperties: null,
                                            body: body);

                        Console.WriteLine(" [x] Sent {0}, Enter a new one or enter empty message to exit.", message);
                    }
                }
            }
        }
    }
}
