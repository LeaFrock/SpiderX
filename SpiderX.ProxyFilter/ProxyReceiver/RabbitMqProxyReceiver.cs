using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SpiderX.Proxy;
using SpiderX.ProxyFilter.Events;
using SpiderX.Tools;

namespace SpiderX.ProxyFilter
{
	internal sealed class RabbitMqProxyReceiver : IProxyReceiver
	{
		private readonly IConnectionFactory _connectionFactory;

		public RabbitMqProxyReceiver(IConnectionFactory connectionFactory)
		{
			_connectionFactory = connectionFactory;
		}

		private IConnection Connection { get; set; }
		private IModel Model { get; set; }

		public string ChannelName { get; set; }

		public EventHandler<ProxyReceiveEventArgs> OnProxyReceived { get; set; }

		public async Task InitializeAsync()
		{
			Connection = _connectionFactory.CreateConnection();
			Model = Connection.CreateModel();
			Model.QueueDeclare(ChannelName, false, false, false, null);
			var consumer = new EventingBasicConsumer(Model);
			consumer.Received += DeserializeProxies;
			Model.BasicConsume(ChannelName, true, consumer);
			await Task.CompletedTask;
		}

		public void Disable()
		{
			Model?.Dispose();
			Connection?.Dispose();
		}

		private void DeserializeProxies(object model, BasicDeliverEventArgs ea)
		{
			var body = ea.Body;
			string json = Encoding.UTF8.GetString(body);
			var proxies = JsonTool.DeserializeObject<SpiderProxyUriEntity[]>(json);
			if (proxies == null || proxies.Length < 1)
			{
				OnProxyReceived?.Invoke(this, ProxyReceiveEventArgs.Empty);
			}
			else
			{
				OnProxyReceived?.Invoke(this, new ProxyReceiveEventArgs() { Proxies = proxies });
			}
			Model.BasicAck(ea.DeliveryTag, false);
		}

		public static RabbitMqProxyReceiver Create(IConfiguration root)
		{
			var mqSection = root.GetSection("MsgQueueConfig");
			string host = mqSection.GetValue<string>("HostName");
			int port = mqSection.GetValue<int>("Port");
			string userName = mqSection.GetValue<string>("UserName");
			string password = mqSection.GetValue<string>("Password");
			string channelName = mqSection.GetValue<string>("ChannelName");
			var factory = new ConnectionFactory()
			{
				HostName = host,
				Port = port
			};
			if (!string.IsNullOrEmpty(userName))
			{
				factory.UserName = userName;
			}
			if (!string.IsNullOrEmpty(password))
			{
				factory.Password = password;
			}
			return new RabbitMqProxyReceiver(factory) { ChannelName = channelName };
		}
	}
}