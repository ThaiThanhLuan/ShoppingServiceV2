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
            factory.UserName = "master";
            factory.Password = "master@123456789";
            factory.HostName = "Demo-queue";
            factory.Port = 5671; 
            factory.Ssl = new SslOption
            {
                Enabled = true,
                ServerName = "Demo-queue"
            };

/*            ServicePointManager.SecurityProtocol = SecurityProtocolTypeExtensions.Tls12;*/
            try
            {
                using var connection = factory.CreateConnection();
                _logger.LogInformation("Connect successfull");
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
