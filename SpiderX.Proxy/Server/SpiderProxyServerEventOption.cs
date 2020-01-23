using System;
using System.Net.Sockets;
using Titanium.Web.Proxy;
using Titanium.Web.Proxy.EventArguments;

namespace SpiderX.Proxy
{
	public sealed class SpiderProxyServerEventOption
	{
		public AsyncEventHandler<SessionEventArgs> AfterResponse { get; set; }
		public AsyncEventHandler<SessionEventArgs> BeforeResponse { get; set; }
		public AsyncEventHandler<SessionEventArgs> BeforeRequest { get; set; }
		public AsyncEventHandler<CertificateSelectionEventArgs> ClientCertificateSelectionCallback { get; set; }
		public AsyncEventHandler<CertificateValidationEventArgs> ServerCertificateValidationCallback { get; set; }
		public EventHandler ServerConnectionCountChanged { get; set; }
		public EventHandler ClientConnectionCountChanged { get; set; }
		public AsyncEventHandler<Socket> OnServerConnectionCreate { get; set; }
		public AsyncEventHandler<Socket> OnClientConnectionCreate { get; set; }

		internal void Bind(ProxyServer server)
		{
			if (AfterResponse != null)
			{
				server.AfterResponse += AfterResponse;
			}
			if (BeforeResponse != null)
			{
				server.BeforeResponse += BeforeResponse;
			}
			if (BeforeRequest != null)
			{
				server.BeforeRequest += BeforeRequest;
			}
			if (ClientCertificateSelectionCallback != null)
			{
				server.ClientCertificateSelectionCallback += ClientCertificateSelectionCallback;
			}
			if (ServerCertificateValidationCallback != null)
			{
				server.ServerCertificateValidationCallback += ServerCertificateValidationCallback;
			}
			if (ServerConnectionCountChanged != null)
			{
				server.ServerConnectionCountChanged += ServerConnectionCountChanged;
			}
			if (ClientConnectionCountChanged != null)
			{
				server.ClientConnectionCountChanged += ClientConnectionCountChanged;
			}
			if (OnServerConnectionCreate != null)
			{
				server.OnServerConnectionCreate += OnServerConnectionCreate;
			}
			if (OnClientConnectionCreate != null)
			{
				server.OnClientConnectionCreate += OnClientConnectionCreate;
			}
		}

		internal void UnBind(ProxyServer server)
		{
			if (AfterResponse != null)
			{
				server.AfterResponse -= AfterResponse;
			}
			if (BeforeResponse != null)
			{
				server.BeforeResponse -= BeforeResponse;
			}
			if (BeforeRequest != null)
			{
				server.BeforeRequest -= BeforeRequest;
			}
			if (ClientCertificateSelectionCallback != null)
			{
				server.ClientCertificateSelectionCallback -= ClientCertificateSelectionCallback;
			}
			if (ServerCertificateValidationCallback != null)
			{
				server.ServerCertificateValidationCallback -= ServerCertificateValidationCallback;
			}
			if (ServerConnectionCountChanged != null)
			{
				server.ServerConnectionCountChanged -= ServerConnectionCountChanged;
			}
			if (ClientConnectionCountChanged != null)
			{
				server.ClientConnectionCountChanged -= ClientConnectionCountChanged;
			}
			if (OnServerConnectionCreate != null)
			{
				server.OnServerConnectionCreate -= OnServerConnectionCreate;
			}
			if (OnClientConnectionCreate != null)
			{
				server.OnClientConnectionCreate -= OnClientConnectionCreate;
			}
		}
	}
}