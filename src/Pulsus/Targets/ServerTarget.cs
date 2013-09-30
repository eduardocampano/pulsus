using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;
using System.Xml.Serialization.Advanced;
using Pulsus.Internal;

namespace Pulsus.Targets
{
    public class ServerTarget : Target
    {
        public string Url { get; set; }
        public bool Compress { get; set; }

        public override void Push(LoggingEvent[] loggingEvents)
        {
            if (loggingEvents == null)
                throw new ArgumentNullException("loggingEvents");

            if (string.IsNullOrEmpty(Url))
                throw new Exception("There is no URL defined for the server target.");

            Uri uri;
            if (!Uri.TryCreate(Url, UriKind.Absolute, out uri))
                throw new Exception("The URL defined for the server target is not valid");

            Post(loggingEvents);			
        }

        public void Post(LoggingEvent[] loggingEvents)
        {
            var request = GetRequest(loggingEvents);

            try
            {
                var response = request.GetResponse() as HttpWebResponse;

                if (response == null)
                    throw new Exception("There was an error posting the server");

                if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.Accepted)
                    throw new Exception(string.Format("The server returned a {0} status with the message: {1}", response.StatusCode, response.StatusDescription));
            } 
            catch (WebException ex)
            {
                if (ex.Response == null)
                    throw new Exception(string.Format("Response Status: {0}, Description: {1}", ex.Status, ex.Message), ex);

                var responseStream = ex.Response.GetResponseStream();
                if (responseStream == null)
                    throw new Exception("GetResponseStream() returned null", ex);
                
                var reader = new StreamReader(responseStream);
                var responseContent = reader.ReadToEnd();

                throw new Exception(string.Format("Response Status: {0}, Content: {1}", ex.Status, responseContent), ex);
            }
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
            request.UserAgent = "Pulsus " + Constants.Version;
            request.Method = "POST";
            request.ContentType = "application/json";

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
            var serialized = LogManager.JsonSerializer.SerializeObject(loggingEvents);
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
