using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using System.Threading;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace orderconsumerapi.Consumer
{
    public class Orderprocessor : BackgroundService
    {
        private readonly ILogger _logger;
        private IConnection _connection;
        private IModel _channel;

        public Orderprocessor(ILoggerFactory loggerFactory)
        {
            this._logger = loggerFactory.CreateLogger<Orderprocessor>();
            InitRabbitMQ();
        }

        private void InitRabbitMQ()
        {
            var factory = new ConnectionFactory
            {

                //  HostName = "localhost" , 
                //   Port = 31500
                HostName = Environment.GetEnvironmentVariable("RABBITMQ_HOST"),
                Port = Convert.ToInt32(Environment.GetEnvironmentVariable("RABBITMQ_PORT"))

            };

            // create connection  
            _connection = factory.CreateConnection();

            // create channel  
            _channel = _connection.CreateModel();

            //_channel.ExchangeDeclare("demo.exchange", ExchangeType.Topic);
            _channel.QueueDeclare("orders", true, false, false, null);
            // _channel.QueueBind("demo.queue.log", "demo.exchange", "demo.queue.*", null);
            // _channel.BasicQos(0, 1, false);

            _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (ch, ea) =>
            {
                // received message  
                var content = System.Text.Encoding.UTF8.GetString(ea.Body.ToArray());

                // handle the received message  
                HandleMessageAsync(content);
                _channel.BasicAck(ea.DeliveryTag, false);
            };

            consumer.Shutdown += OnConsumerShutdown;
            consumer.Registered += OnConsumerRegistered;
            consumer.Unregistered += OnConsumerUnregistered;
            consumer.ConsumerCancelled += OnConsumerConsumerCancelled;

            _channel.BasicConsume("orders", false, consumer);
            return Task.CompletedTask;
        }

        private async Task HandleMessageAsync(string content)
        {
            // we just print this message   
            _logger.LogInformation($"consumer received {content}");



            string[] tokens = content.Split("=");
            string[] CartId = tokens[1].Split(",");
            string[] ProductId = tokens[2].Split(",");
            string[] ProductPrices = tokens[3].Split(",");
            string[] Total = tokens[4].Split(",");
            string[] OrderStatus = tokens[5].Split(",");
            string[] OrderId = tokens[6].Split(",");

            _logger.LogInformation($"after received {CartId[0]}");
            _logger.LogInformation($"Processing order... {OrderId[0]}");
            _logger.LogInformation($"Total amount is {Total[0]}");

            await Task.Delay(30000);

            _logger.LogInformation($"will be published with updated orderid {OrderId[0]} and cart ID {CartId[0]}");

            var factory = new ConnectionFactory()
            {
                HostName = Environment.GetEnvironmentVariable("RABBITMQ_HOST"),
                Port = Convert.ToInt32(Environment.GetEnvironmentVariable("RABBITMQ_PORT"))
            };

            Console.WriteLine(factory.HostName + ":" + factory.Port);
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "orders",
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                string message = "OrderId=" + OrderId[0] + ", Cart Id=" + CartId[0] + ", Order Status=" + OrderStatus[0];
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "",
                                     routingKey: "orders",
                                     basicProperties: null,
                                     body: body);
            }

        }

        private void OnConsumerConsumerCancelled(object sender, ConsumerEventArgs e) { }
        private void OnConsumerUnregistered(object sender, ConsumerEventArgs e) { }
        private void OnConsumerRegistered(object sender, ConsumerEventArgs e) { }
        private void OnConsumerShutdown(object sender, ShutdownEventArgs e) { }
        private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e) { }

        public override void Dispose()
        {
            _channel.Close();
            _connection.Close();
            base.Dispose();
        }
    }
}