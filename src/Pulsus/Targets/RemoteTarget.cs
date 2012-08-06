using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using Pulsus.Configuration;

namespace Pulsus
{
	public class RemoteTarget : ITarget
	{
		private static string version = Assembly.GetAssembly(typeof(RemoteTarget)).GetName().Version.ToString();

		private readonly IRemoteSettings _settings;

		public RemoteTarget() : this(ConfigurationManager.Settings.Remote)
		{ 
		}

		public RemoteTarget(IRemoteSettings settings)
		{
			_settings = settings;
		}

		public static string Url { get; set; }
		public static string LogKey { get; set; }

		public void Log(LoggingEvent loggingEvent)
		{
			if (loggingEvent == null)
				throw new ArgumentNullException("loggingEvent");

			if (!_settings.Enabled)
				return;

			var url = Url ?? _settings.Url;
			if (string.IsNullOrEmpty(url))
				throw new Exception("There is no URL defined for the remote logger.");

			Uri uri;
			if (!Uri.TryCreate(url, UriKind.Absolute, out uri))
				throw new Exception("The URL defined for the remote logger is not valid");

			var logKey = LogKey ?? _settings.LogKey;
			if (string.IsNullOrEmpty(logKey))
				throw new Exception("There is no LogKey defined for the remote logger");

			var remoteLoggingEvent = RemoteLoggingEvent.Create(LogKey ?? _settings.LogKey, loggingEvent);

			Post(uri, remoteLoggingEvent);			
		}

		protected void Post(Uri uri, object loggingEvent)
		{
			var remoteClient = new RemoteClient(uri, loggingEvent);
			var compress = _settings.Compress;
			remoteClient.Post(compress);
		}

		internal class RemoteClient
		{ 
			private readonly Uri _uri;
			
			private JsonSerializerSettings _serializerSettings;
			private object _data;

			public RemoteClient(Uri uri, object data)
			{
				_uri = uri;
				_data = data;
				_serializerSettings = new JsonSerializerSettings();
				_serializerSettings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
			}

			public object Data
			{
				get
				{
					return _data;
				}
			}

			public void Post(bool compress)
			{
				var request = GetRequest(_uri, compress);
				var response = (HttpWebResponse)request.GetResponse();
			}

			protected HttpWebRequest GetRequest(Uri uri, bool compress)
			{
				var request = (HttpWebRequest)WebRequest.Create(_uri);
				request.KeepAlive = false;
				request.Timeout = 5000;
				request.ServicePoint.ConnectionLeaseTimeout = 5000;
				request.ServicePoint.MaxIdleTime = 5000;
				request.ServicePoint.ConnectionLimit = 50;
				request.Accept = "application/json";
				request.UserAgent = "Pulsus Remote Logger " + version;
				request.Method = "POST";
				request.ContentType = "application/json";
				

				var serialized = JsonConvert.SerializeObject(_data, _serializerSettings);
				var bytes = Encoding.ASCII.GetBytes(serialized);
				request.ContentLength = bytes.Length;

				if (compress)
				{
					using (var memoryStream = new MemoryStream())
					{
						using (var zipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
						{
							zipStream.Write(bytes, 0, bytes.Length);
							zipStream.Flush();
						}

						memoryStream.Seek(0, SeekOrigin.Begin);
						bytes = memoryStream.ToArray();
					}

					request.Headers.Add("Content-Encoding", "gzip");
					request.ContentLength = bytes.Length;
				}

				using (var requestStream = request.GetRequestStream())
				{
					requestStream.Write(bytes, 0, bytes.Length);
				}

				return request;
			}
		}

		internal class RemoteLoggingEvent : LoggingEvent
		{
			public string LogKey { get; set; }

			public static RemoteLoggingEvent Create(string logKey, LoggingEvent loggingEvent)
			{
				var remote = new RemoteLoggingEvent();
				remote.LogKey = logKey;
				remote.Timestamp = loggingEvent.Timestamp;
				remote.Level = loggingEvent.Level;
				remote.User = loggingEvent.User;
				remote.Source = loggingEvent.Source;
				remote.Text = loggingEvent.Text;
				remote.Value = loggingEvent.Value;
				remote.Tags = loggingEvent.Tags;
				remote.Data = loggingEvent.Data;
				return remote;
			}
		}
	}
}
