using System;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using Pulsus.Internal;

namespace Pulsus.Targets
{
	public class AmazonS3Target : Target
	{
		private readonly JsonSerializerSettings _serializerSettings;

		public AmazonS3Target()
		{
			FileNameFormat = "{eventid}.json";
			_serializerSettings = new JsonSerializerSettings();
			_serializerSettings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
		}

		public string AccessKey { get; set; }
		public string SecretKey { get; set; }
		public string BucketName { get; set; }
		public string FileNameFormat { get; set; }
		public bool Compress { get; set; }

		public override void Push(LoggingEvent[] loggingEvents)
		{
			if (loggingEvents == null)
				throw new ArgumentNullException("loggingEvents");

			if (string.IsNullOrEmpty(AccessKey))
				throw new Exception("You must define an AccessKey");

			if (string.IsNullOrEmpty(SecretKey))
				throw new Exception("You must specify a SecretKey");

			if (string.IsNullOrEmpty(BucketName))
				throw new Exception("You must specify a BucketName");

			foreach (var loggingEvent in loggingEvents)
				Post(loggingEvent);
		}

		protected void Post(LoggingEvent loggingEvent)
		{
			var request = GetRequest(loggingEvent);
			try
			{
				var response = request.GetResponse() as HttpWebResponse;
			}
			catch (WebException ex)
			{
				try
				{
					var responseStream = ex.Response.GetResponseStream();
					if (responseStream == null)
						throw new Exception("GetResponseStream() returned null", ex);
					var reader = new StreamReader(responseStream);
					var responseContent = reader.ReadToEnd();
					throw new Exception(string.Format("Response Status: {0}, Response Content: {1}", ex.Status, responseContent), ex);
				}
				catch
				{
				}
			}
		}

		protected HttpWebRequest GetRequest(LoggingEvent loggingEvent)
		{
			var dateString = DateTime.UtcNow.ToString("ddd, dd MMM yyyy HH:mm:ss", CultureInfo.InvariantCulture) + " GMT";
			var domain = string.Format("{0}.s3.amazonaws.com", BucketName);
			var fileName = loggingEvent.EventId + ".json";
			var url = string.Format("http://{0}/{1}", domain, fileName);
			const string httpVerb = "PUT";
			const string contentType = "application/json";

			var request = (HttpWebRequest)WebRequest.Create(url);
			request.KeepAlive = false;
			request.Timeout = 5000;
			request.UserAgent = "Pulsus " + PulsusLogger.Version;
			request.Method = httpVerb;
				
			var bytes = GetRequestBody(loggingEvent, Compress);
			request.ContentLength = bytes.Length;
			if (Compress)
				request.Headers.Add("Content-Encoding", "gzip");
				
			var contentMd5 = CalculateMd5(bytes);

			request.ContentType = contentType;
			//request.Headers.Set("Host", domain);
			request.Headers.Set("x-amz-date", dateString);
			request.Headers.Add("Content-MD5", contentMd5);
			request.Headers.Add("Authorization", string.Format("AWS {0}:{1}", AccessKey, GetSignature(httpVerb, contentMd5, contentType, dateString, BucketName, fileName)));

			using (var requestStream = request.GetRequestStream())
			{
				requestStream.Write(bytes, 0, bytes.Length);
			}

			return request;
		}

		protected byte[] GetRequestBody(LoggingEvent loggingEvent, bool compress)
		{
			var serialized = JsonConvert.SerializeObject(loggingEvent, _serializerSettings);
			var bytes = Encoding.UTF8.GetBytes(serialized);

			if (Compress)
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

		protected string GetFileName(LoggingEvent loggingEvent)
		{
			return FileNameFormat.Format(loggingEvent);
		}

		protected string CalculateMd5(byte[] content)
		{
			var hashBytes = MD5.Create().ComputeHash(content);
			return Convert.ToBase64String(hashBytes);
		}

		protected string GetSignature(string httpVerb, string contentMd5, string contentType, string dateString, string bucketName, string fileName)
		{
			var canonicalString = string.Format("{0}\n{1}\n{2}\n{3}\nx-amz-date:{4}\n/{5}/{6}", httpVerb, contentMd5, contentType, string.Empty, dateString, bucketName, fileName);
			var signature = new HMACSHA1(Encoding.ASCII.GetBytes(SecretKey));
			var bytes = Encoding.ASCII.GetBytes(canonicalString);
			var hashBytes = signature.ComputeHash(bytes);
			return Convert.ToBase64String(hashBytes);
		}
	}
}