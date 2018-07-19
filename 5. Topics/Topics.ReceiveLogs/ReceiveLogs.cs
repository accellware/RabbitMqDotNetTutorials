using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;


namespace HelloWorld.Consumer
{
    class ReceiveLogs
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare(exchange: "topic_logs", type: "topic");

                    var queueName = channel.QueueDeclare().QueueName;

                    foreach(var severity in args)
                    {
                        Console.WriteLine(" [*] Binding to severity {0}", severity);
                        channel.QueueBind(queue: queueName, exchange: "topic_logs", routingKey: severity);
                    }

                    Console.Write(" [*] Waiting for logs.");

                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (sender, ea) => {
                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body);

                        Console.WriteLine(" [x] {0}", message);
                    };

                    channel.BasicConsume(queue: queueName,
                                        autoAck: true,
                                        consumer: consumer);

                    Console.WriteLine(" Hit CTRL+C to exit.");
                    Console.ReadLine();
                }
            }
        }
    }
}
