using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Pulsus.Server
{
	public class JsonpMediaTypeFormatter : JsonMediaTypeFormatter
	{
		private string _callbackQueryParameter;

		public JsonpMediaTypeFormatter()
		{
			SupportedMediaTypes.Add(DefaultMediaType);
			SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/javascript"));

			// need a lambda here so that it'll always get the 'live' value of CallbackQueryParameter.
			MediaTypeMappings.Add(new Mapping(() => CallbackQueryParameter, "application/javascript"));
		}

		public string CallbackQueryParameter
		{
			get { return _callbackQueryParameter ?? "callback"; }
			set { _callbackQueryParameter = value; }
		}

		public override Task WriteToStreamAsync(Type type, object value, Stream writeStream, HttpContent content,
												TransportContext transportContext)
		{
			var callback = GetCallbackName();

			if (!String.IsNullOrEmpty(callback))
			{
				// select the correct encoding to use.
				Encoding encoding = SelectCharacterEncoding(content.Headers);

				// write the callback and opening paren.
				return Task.Factory.StartNew(() =>
				{
					var bytes = encoding.GetBytes(callback + "(");
					writeStream.Write(bytes, 0, bytes.Length);
				})
					// then we do the actual JSON serialization...
				.ContinueWith(t => base.WriteToStreamAsync(type, value, writeStream, content, transportContext))

				// finally, we close the parens.
				.ContinueWith(t =>
				{
					var bytes = encoding.GetBytes(")");
					writeStream.Write(bytes, 0, bytes.Length);
				});
			}
			return base.WriteToStreamAsync(type, value, writeStream, content, transportContext);
		}

		private string GetCallbackName()
		{
			if (HttpContext.Current.Request.HttpMethod != "GET")
				return null;
			return HttpContext.Current.Request.QueryString[CallbackQueryParameter];
		}

		private class Mapping : MediaTypeMapping
		{
			private readonly Func<string> _param;

			public Mapping(Func<string> discriminator, string mediaType)
				: base(mediaType)
			{
				_param = discriminator;
			}

			public override double TryMatchMediaType(HttpRequestMessage request)
			{
				if (request.RequestUri.Query.Contains(_param() + "="))
					return 1.0;
				return 0.0;
			}
		}
	}
}
