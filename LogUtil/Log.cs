using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Text;

namespace LogUtil
{
    public class Log
    {
        public void Publish(string message)
        {
            try
            {
                var factory = new ConnectionFactory() { HostName = "RabbitMQ", VirtualHost = "EnterpriseLog", UserName = "logUser", Password = "logPwd" };
                using (var connectionB = factory.CreateConnection())
                using (var channel = connectionB.CreateModel())
                {
                    channel.QueueDeclare(queue: "ApplicationLog",
                                         durable: true,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);


                    //var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
                    var body = Encoding.UTF8.GetBytes(message);

                    var properties = channel.CreateBasicProperties();
                    properties.Persistent = true;

                    channel.BasicPublish(exchange: "",
                                         routingKey: "ApplicationLog",
                                         basicProperties: properties,
                                         body: body);

                }

            }
            catch (Exception ex)
            {

                throw;
            }

            

        }

    }
}
