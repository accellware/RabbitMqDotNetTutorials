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
                    channel.ExchangeDeclare(exchange: "topic_logs", type: "topic");
                    
                    String message;
                    Console.WriteLine("Write your first message: ");
                    while (!String.IsNullOrWhiteSpace(message = Console.ReadLine()))
                    {
                        var tmp = message.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);

                        if(tmp.Length != 2)
                        {
                            Console.WriteLine(" [*] Invalid entry, skipping.");
                        }

                        var severity = tmp[0].Trim();

                        var body = Encoding.UTF8.GetBytes(message);
                        
                        channel.BasicPublish(exchange: "topic_logs",
                                            routingKey: severity,
                                            basicProperties: null,
                                            body: body);

                        Console.WriteLine(" [x] Sent {0}, Enter a new one or enter empty message to exit.", message);
                    }
                }
            }
        }
    }
}
