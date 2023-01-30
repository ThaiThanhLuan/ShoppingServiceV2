using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using ShoppingService2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ShoppingService2.RabbitMQ
{
    public class ProductPublisher : IRabitMQProducer
    {
        
        private readonly ILogger<ProductPublisher> _logger;
        private readonly IConfiguration _configuration;
        public ProductPublisher(ILogger<ProductPublisher> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public void SendProductMessage<T>(T product)
        {
            ConnectionFactory factory = new ConnectionFactory();
            factory.UserName = _configuration.GetValue<string>("QueueService:UserName");
            factory.Password = _configuration.GetValue<string>("QueueService:Password");
            factory.VirtualHost = _configuration.GetValue<string>("QueueService:VirtualHost");
            factory.HostName = _configuration.GetValue<string>("QueueService:Host");
            factory.Port = _configuration.GetValue<int>("QueueService:Port"); 
            factory.Ssl = new SslOption
            {
                Enabled = true,
                ServerName = _configuration.GetValue<string>("QueueService:Host")
            };

            ServicePointManager.SecurityProtocol = SecurityProtocolTypeExtensions.Tls12;
            try
            {
                using var connection = factory.CreateConnection();
                using var channel = connection.CreateModel();
                channel.QueueDeclare(queue: "hello",
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);
                var json = JsonConvert.SerializeObject(product);
                var body = Encoding.UTF8.GetBytes(json);

                channel.BasicPublish(exchange: "",
                       routingKey: "hello",
                       basicProperties: null,
                       body: body);
                _logger.LogInformation($"Send data successfully {json}");
            }
            catch(Exception e)
            {
                _logger.LogInformation(e.Message);

            }

        }


        internal static class SecurityProtocolTypeExtensions
        {
            public const SecurityProtocolType Tls12 = (SecurityProtocolType)3072;
            public const SecurityProtocolType Tls11 = (SecurityProtocolType)768;
        }
    }
}
