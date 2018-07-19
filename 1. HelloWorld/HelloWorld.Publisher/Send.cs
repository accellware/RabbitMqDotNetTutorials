using RabbitMQ.Client;
using System;
using System.Text;

namespace HelloWorld.Publisher
{
    class Send
    {
        static void Main(string[] args)
        {
            Console.WriteLine(" Program started.");

            var factory = new ConnectionFactory { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: "hello",
                                        durable: false,
                                        exclusive: false,
                                        autoDelete: false,
                                        arguments: null);

                    while(true)
                    {
                        string input = Console.ReadLine();
                        string message = $"{input}\nDate: {DateTime.UtcNow}";
                        var body = Encoding.UTF8.GetBytes(message);

                        channel.BasicPublish(exchange: "",
                                            routingKey: "hello",
                                            basicProperties: null,
                                            body: body);

                        Console.WriteLine(" [x] Sent {0}", message);
                    }
                }
            }

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadKey();
        }
    }
}
