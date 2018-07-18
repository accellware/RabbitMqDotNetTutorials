using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;


namespace HelloWorld.Consumer
{
    class Worker
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: "durable_task_queue_new",
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                    // block this user from processing more than one message a time
                    channel.BasicQos(0, 1, false);

                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (sender, ea) => {
                        new Task(() => {
                            var body = ea.Body;
                            var message = Encoding.UTF8.GetString(body);

                            Console.WriteLine(" [x] message received: {0}", message);

                            var dotsCount = message.ToCharArray().Count(c => c == '.') - 1;

                            Thread.Sleep(dotsCount * 1000);

                            Console.WriteLine(" [x] Done");

                            channel.BasicAck(deliveryTag: ea.DeliveryTag,
                                            multiple: false);
                        }).Start();
                    };

                    channel.BasicConsume(queue: "durable_task_queue_new",
                                        autoAck: false,
                                        consumer: consumer);

                    Console.WriteLine(" [*] Waiting for messages. To exit press CTRL+C.");
                    Console.ReadLine();
                }
            }
        }
    }
}
