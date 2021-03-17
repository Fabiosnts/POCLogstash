using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain;
using LogUtil;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace apiCadastro.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProdutoController : ControllerBase
    {

        private IConnection _connection;
        private IModel _channel;

        public ProdutoController()
        {
            InitRabbitMQ();
        }

        
        // POST api/<Produto>
        [HttpPost]
        public void Post(Produto produto)
        {
            try
            {
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(produto);
                var body = Encoding.UTF8.GetBytes(json);
                var properties = _channel.CreateBasicProperties();

                properties.ContentType = "text/plain";

                properties.Persistent = true;

                // Comentado para teste, deveria fazer a publicação numa fila para ser comsumido por outro serviço
                //_channel.BasicPublish(exchange: "",
                //                                 routingKey: "Produto",
                //                                 basicProperties: properties,
                //                                 body: body);

                new Log().Publish(json);
            }
            catch (Exception ex)
            {
                new Log().Publish(ex.ToString());
                throw;
            }
            

        }

       

        private void InitRabbitMQ()
        {
            try
            {

                var factory = new ConnectionFactory { HostName = "RabbitMQ", VirtualHost = "EnterpriseLog", UserName = "logUser", Password = "logPwd" };

                // create connection  
                _connection = factory.CreateConnection();

                // create channel  
                _channel = _connection.CreateModel();

                _channel.ExchangeDeclare("demo.exchange", ExchangeType.Topic);
                _channel.QueueDeclare("Produto", false, false, false, null);
                _channel.QueueBind("Produto", "demo.exchange", "demo.queue.*", null);
                _channel.BasicQos(0, 1, false);

                _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;

            }
            catch (Exception ex)
            {
                new Log().Publish(ex.ToString());
            }

        }

        private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            
        }
    }
}
