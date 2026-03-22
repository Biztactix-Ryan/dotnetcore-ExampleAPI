using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;
using ExampleAPI.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Text;

namespace ExampleAPI.Services
{
    public class RabbitManager : IRabbitManager, IDisposable
    {
        private readonly DefaultObjectPool<IModel> _objectPool;
        private readonly IPooledObjectPolicy<IModel> _objectPolicy;
        private bool _disposed;

        public RabbitManager(IPooledObjectPolicy<IModel> objectPolicy)
        {
            _objectPolicy = objectPolicy;
            _objectPool = new DefaultObjectPool<IModel>(objectPolicy, Environment.ProcessorCount * 2);
        }

        public void Publish<T>(T message, string queueName)
            where T : class
        {
            if (message == null)
                return;

            var channel = _objectPool.Get();

            try
            {
                channel.QueueDeclare(queueName, true, false, false);

                var sendBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));

                var properties = channel.CreateBasicProperties();
                properties.Persistent = true;

                channel.BasicPublish("", queueName, properties, sendBytes);
            }
            finally
            {
                _objectPool.Return(channel);
            }
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;

            if (_objectPolicy is IDisposable disposablePolicy)
            {
                disposablePolicy.Dispose();
            }
        }
    }
    public interface IRabbitManager
    {
        void Publish<T>(T message, string queueName)
            where T : class;
    }


    public class RabbitModelPooledObjectPolicy : IPooledObjectPolicy<IModel>, IDisposable
    {
        private readonly RabbitOptions _options;

        private readonly IConnection _connection;
        private bool _disposed;

        public RabbitModelPooledObjectPolicy(IOptions<RabbitOptions> optionsAccs)
        {
            _options = optionsAccs.Value;
            _connection = GetConnection();
        }

        private IConnection GetConnection()
        {
            var factory = new ConnectionFactory()
            {
                HostName = _options.HostName,
                UserName = _options.UserName,
                Password = _options.Password,
                Port = _options.Port,
                VirtualHost = _options.VHost,
            };
            factory.ClientProvidedName = _options.AppName;
            return factory.CreateConnection();
        }

        public IModel Create()
        {
            return _connection.CreateModel();
        }

        public bool Return(IModel obj)
        {
            if (obj.IsOpen)
            {
                return true;
            }
            else
            {
                obj?.Dispose();
                return false;
            }
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;

            if (_connection != null)
            {
                try
                {
                    _connection.Close();
                }
                catch
                {
                    // Connection may already be closed
                }

                _connection.Dispose();
            }
        }
    }
}
