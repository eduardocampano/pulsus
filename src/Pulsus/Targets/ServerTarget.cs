using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using Pulsus.Configuration;
using Pulsus.Internal;

namespace Pulsus.Targets
{
	public class ServerTarget : ITarget
	{
		private const string LogKeyHeader = "X-PULSUS-LOGKEY";
		private const string ApiKeyHeader = "X-PULSUS-APIKEY";
		private readonly IPulsusSettings _settings;

		public ServerTarget(IPulsusSettings settings)
		{
			_settings = settings;
		}

		public bool Enabled
		{
			get
			{
				return _settings.Server.Enabled;
			}
		}

		public void Push(LoggingEvent[] loggingEvents)
		{
			if (loggingEvents == null)
				throw new ArgumentNullException("loggingEvents");

			if (!Enabled)
				return;

			if (string.IsNullOrEmpty(_settings.Server.Url))
				throw new Exception("There is no URL defined for the server target.");

			Uri uri;
			if (!Uri.TryCreate(_settings.Server.Url, UriKind.Absolute, out uri))
				throw new Exception("The URL defined for the server target is not valid");

			if (string.IsNullOrEmpty(_settings.LogKey))
				throw new Exception("There is no LogKey defined");

			if (string.IsNullOrEmpty(_settings.Server.ApiKey))
				throw new Exception("There is no ApiKey defined for the server target");

			Post(loggingEvents);			
		}

		protected void Post(object loggingEvent)
		{
			var remoteClient = new ServerClient(_settings, loggingEvent);
			remoteClient.Post();
		}

		internal class ServerClient
		{
			private readonly IPulsusSettings _settings;
			
			private readonly JsonSerializerSettings _serializerSettings;
			private readonly object _data;

			public ServerClient(IPulsusSettings settings, object data)
			{
				_settings = settings;
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

			public void Post()
			{
				var request = GetRequest();
				var response = request.GetResponse() as HttpWebResponse;
				if (response == null)
					throw new Exception("There was an error posting the server");

				if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.Accepted)
					throw new Exception(string.Format("The server returned a {0} status with the message: {1}", response.StatusCode, response.StatusDescription));
			}

			protected HttpWebRequest GetRequest()
			{
				var request = (HttpWebRequest)WebRequest.Create(_settings.Server.Url);
				request.KeepAlive = false;
				request.Timeout = 5000;
				request.ServicePoint.ConnectionLeaseTimeout = 5000;
				request.ServicePoint.MaxIdleTime = 5000;
				request.ServicePoint.ConnectionLimit = 50;
				request.Accept = "application/json";
				request.UserAgent = "Pulsus " + PulsusLogger.Version;
				request.Method = "POST";
				request.ContentType = "application/json";
				request.Headers.Add(LogKeyHeader, _settings.LogKey);
				request.Headers.Add(ApiKeyHeader, _settings.Server.ApiKey);

				var serialized = JsonConvert.SerializeObject(_data, _serializerSettings);
				var bytes = Encoding.ASCII.GetBytes(serialized);
				request.ContentLength = bytes.Length;

				if (_settings.Server.Compress)
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

			public override string ToString()
			{
				return string.Format("[Server] Url: {0}, LogKey: {1}, ApiKey: {2}, Compress: {3}", _settings.Server.Url, _settings.LogKey, _settings.Server.ApiKey, _settings.Server.Compress);
			}
		}
	}
}
