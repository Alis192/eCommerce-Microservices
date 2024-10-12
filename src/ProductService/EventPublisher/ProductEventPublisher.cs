using RabbitMQ.Client;
using System.Text;
using Newtonsoft.Json;
using ProductService.Models; // Your product model

namespace ProductService.EventPublishers
{
    public class ProductEventPublisher
    {
        private readonly IModel _channel;

        public ProductEventPublisher()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            var connection = factory.CreateConnection();
            _channel = connection.CreateModel();
            _channel.QueueDeclare(queue: "product_queue",
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);
        }

        public void PublishProductEvent(Product product)
        {
            var message = JsonConvert.SerializeObject(product);
            var body = Encoding.UTF8.GetBytes(message);
            _channel.BasicPublish(exchange: "",
                                 routingKey: "product_queue",
                                 basicProperties: null,
                                 body: body);
        }
    }
}
