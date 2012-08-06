using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reflection;
using System.Web;
using System.Web.Routing;
using System.Web.SessionState;
using Newtonsoft.Json;

namespace Pulsus
{
	public class HttpContextInformation
	{
		public static bool IncludePasswords { get; set; }
		public static bool IncludeSessionInformation { get; set; }

		private static PropertyInfo RequestContextProperty = GetRequestContextProperty();

		public static HttpContextInformation Create(HttpContext httpContext, RouteValueDictionary routeValues = null)
		{
			if (httpContext == null)
				throw new ArgumentNullException("httpContext");

			var httpContextInformation = new HttpContextInformation();

			httpContextInformation.Url = httpContext.Request.Url.ToString();
			httpContextInformation.User = httpContext.User.Identity.IsAuthenticated ? httpContext.User.Identity.Name : string.Empty;
			httpContextInformation.UserAgent = httpContext.Request.ServerVariables["HTTP_USER_AGENT"];
			httpContextInformation.Host = Environment.MachineName;
			httpContextInformation.Referer = httpContext.Request.ServerVariables["HTTP_REFERER"];
			httpContextInformation.Method = httpContext.Request.ServerVariables["REQUEST_METHOD"];
			httpContextInformation.RemoteIp = httpContext.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
			if (string.IsNullOrEmpty(httpContextInformation.RemoteIp))
				httpContextInformation.RemoteIp = httpContext.Request.ServerVariables["REMOTE_ADDR"];

			httpContextInformation.Headers = KeyValueCollection.Create(httpContext.Request.Headers);
			httpContextInformation.Cookies = GetCookies(httpContext.Request.Cookies);
			httpContextInformation.ServerVariables = KeyValueCollection.Create(httpContext.Request.ServerVariables);
			httpContextInformation.QueryString = KeyValueCollection.Create(httpContext.Request.QueryString);
			httpContextInformation.PostedValues = KeyValueCollection.Create(httpContext.Request.Form);
			httpContextInformation.PostedFiles = GetFiles(httpContext.Request.Files);
			httpContextInformation.Session = GetSession(httpContext.Session);

			if (!IncludePasswords)
			{
				ObfuscatePasswords(httpContextInformation.QueryString);
				ObfuscatePasswords(httpContextInformation.PostedValues);
			}

			if (routeValues == null)
				routeValues = GetRouteValues(httpContext);

			httpContextInformation.RouteValues = GetRouteValues(routeValues);

			return httpContextInformation;
		}

		public virtual string Url { get; set; }
		public virtual string UserAgent { get; set; }
		public virtual string User { get; set; }
		public virtual string Host { get; set; }
		public virtual string Referer { get; set; }
		public virtual string RemoteIp { get; set; }
		public virtual string Method { get; set; }

		public virtual KeyValueCollection Headers { get; set; }
		public virtual KeyValueCollection QueryString { get; set; }
		public virtual KeyValueCollection PostedValues { get; set; }
		public virtual List<HttpFileInformation> PostedFiles { get; set; }
		public virtual KeyValueCollection Cookies { get; set; }
		public virtual KeyValueCollection RouteValues { get; set; }
		public virtual KeyValueCollection ServerVariables { get; set; }
		public virtual KeyValueCollection Session { get; set; }

		private static KeyValueCollection GetCookies(HttpCookieCollection cookies)
		{
			var result = new KeyValueCollection();
			foreach (var key in cookies.AllKeys)
			{
				var cookie = cookies[key];
				result.Add(cookie.Name, cookie.Value);
			}
			return result;
		}

		private static List<HttpFileInformation> GetFiles(HttpFileCollection files)
		{
			var result = new List<HttpFileInformation>();
			foreach (HttpPostedFile file in files)
				result.Add(HttpFileInformation.Create(file.FileName, file.ContentLength, file.ContentType));

			return result;
		}

		private static KeyValueCollection GetSession(HttpSessionState session)
		{
			if (!IncludeSessionInformation)
				return null;

			var collection = new KeyValueCollection();
			foreach (var key in session.Keys)
				collection.Add(key.ToString(), JsonConvert.SerializeObject(session[key.ToString()]));

			return collection;
		}

		private static void ObfuscatePasswords(KeyValueCollection collection)
		{
			foreach (var key in collection.Keys)
			{
				if (key.IndexOf("password", StringComparison.InvariantCultureIgnoreCase) >= 0)
					collection[key] = new string('*', collection[key].Length);
			}
		}

		private static RouteValueDictionary GetRouteValues(HttpContext httpContext)
		{
			// this is basically for retriving the RouteValues in a .NET 4 environment
			// and the RequestContext propery exists in HttpRequest
			if (RequestContextProperty == null)
				return null;

			var requestContext = RequestContextProperty.GetValue(httpContext.Request, null) as RequestContext;
			if (requestContext == null)
				return null;

			return requestContext.RouteData.Values;
		}

		private static KeyValueCollection GetRouteValues(RouteValueDictionary routeValues)
		{
			var collection = new KeyValueCollection();
			foreach (var item in routeValues)
				collection.Add(item.Key, JsonConvert.SerializeObject(item.Value));

			return collection;
		}

		private static PropertyInfo GetRequestContextProperty()
		{
			try
			{
				return typeof(HttpRequest).GetProperty("RequestContext", BindingFlags.Instance | BindingFlags.Public);
			}
			catch
			{
				return null;
			}
		}
	}

	public class KeyValueCollection : Dictionary<string, string>
	{
		public static KeyValueCollection Create(NameValueCollection collection)
		{
			var result = new KeyValueCollection();
			foreach (var key in collection.AllKeys)
				result.Add(key, collection[key]);

			return result;
		}
	}

	public class HttpFileInformation
	{
		public static HttpFileInformation Create(string name, int contentLength, string contentType)
		{
			return new HttpFileInformation
			{
				Name = name,
				ContentLength = contentLength,
				ContentType = contentType
			};
		}

		public string Name { get; set; }
		public int ContentLength { get; set; }
		public string ContentType { get; set; }
	}
}
