using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingService2.RabbitMQ
{
    public interface IRabitMQProducer
    {
        public void SendProductMessage<T>(T product);
    }
}
