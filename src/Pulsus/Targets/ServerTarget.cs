using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using Pulsus.Internal;

namespace Pulsus.Targets
{
	public class ServerTarget : Target
	{
		private readonly JsonSerializerSettings _serializerSettings;
		private const string LogKeyHeader = "X-PULSUS-LOGKEY";
		private const string ApiKeyHeader = "X-PULSUS-APIKEY";

		public ServerTarget()
		{
			_serializerSettings = new JsonSerializerSettings();
			_serializerSettings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
		}

		public string Url { get; set; }
		public string LogKey { get; set; }
		public string ApiKey { get; set; }
		public bool Compress { get; set; }

		public override void Push(LoggingEvent[] loggingEvents)
		{
			if (loggingEvents == null)
				throw new ArgumentNullException("loggingEvents");

			//if (!Enabled)
			//	return;

			//if (string.IsNullOrEmpty(Url))
			//	throw new Exception("There is no URL defined for the server target.");

			//Uri uri;
			//if (!Uri.TryCreate(_settings.Server.Url, UriKind.Absolute, out uri))
			//	throw new Exception("The URL defined for the server target is not valid");

			//if (string.IsNullOrEmpty(_settings.LogKey))
			//	throw new Exception("There is no LogKey defined");

			//if (string.IsNullOrEmpty(_settings.Server.ApiKey))
			//	throw new Exception("There is no ApiKey defined for the server target");

			Post(loggingEvents);			
		}

		public void Post(LoggingEvent[] loggingEvents)
		{
			var request = GetRequest(loggingEvents);
			var response = request.GetResponse() as HttpWebResponse;
			if (response == null)
				throw new Exception("There was an error posting the server");

			if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.Accepted)
				throw new Exception(string.Format("The server returned a {0} status with the message: {1}", response.StatusCode, response.StatusDescription));
		}

		protected HttpWebRequest GetRequest(LoggingEvent[] loggingEvents)
		{
			var request = (HttpWebRequest)WebRequest.Create(Url);
			request.KeepAlive = false;
			request.Timeout = 5000;
			request.ServicePoint.ConnectionLeaseTimeout = 5000;
			request.ServicePoint.MaxIdleTime = 5000;
			request.ServicePoint.ConnectionLimit = 50;
			request.Accept = "application/json";
			request.UserAgent = "Pulsus " + PulsusLogger.Version;
			request.Method = "POST";
			request.ContentType = "application/json";
			request.Headers.Add(LogKeyHeader, LogKey);
			request.Headers.Add(ApiKeyHeader, ApiKey);

			var bytes = GetRequestBody(loggingEvents, Compress);
			request.ContentLength = bytes.Length;
			if (Compress)
				request.Headers.Add("Content-Encoding", "gzip");

			using (var requestStream = request.GetRequestStream())
			{
				requestStream.Write(bytes, 0, bytes.Length);
			}

			return request;
		}

		protected byte[] GetRequestBody(LoggingEvent[] loggingEvents, bool compress)
		{
			var serialized = JsonConvert.SerializeObject(loggingEvents, _serializerSettings);
			var bytes = Encoding.UTF8.GetBytes(serialized);

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
			}

			return bytes;
		}
	}
}
