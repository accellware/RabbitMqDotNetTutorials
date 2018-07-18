using RabbitMQ.Client;
using System;
using System.Text;
using System.Threading.Tasks;

namespace HelloWorld.Publisher
{
    class NewTask
    {
        static void Main(string[] args)
        {
            Console.WriteLine(" [*] Program started, enter empty line to exit.");

            var factory = new ConnectionFactory { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: "durable_task_queue_new",
                                        durable: true,
                                        exclusive: false,
                                        autoDelete: false,
                                        arguments: null);
                    
                    String message;
                    Console.WriteLine("Write your first message: ");
                    while (!String.IsNullOrWhiteSpace(message = Console.ReadLine()))
                    {
                        var body = Encoding.UTF8.GetBytes(message);
                        var properties = channel.CreateBasicProperties();

                        properties.Persistent = true;

                        channel.BasicPublish(exchange: "",
                                            routingKey: "durable_task_queue_new",
                                            basicProperties: properties,
                                            body: body);

                        Console.WriteLine(" [x] Sent {0}, Enter a new one or enter empty message to exit.", message);
                    }
                }
            }
        }
    }
}
