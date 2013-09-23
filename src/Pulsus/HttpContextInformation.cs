using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Hosting;
using System.Web.Routing;
using Pulsus.Internal;

namespace Pulsus
{
    public class HttpContextInformation
    {
        public static bool IncludePasswords { get; set; }
        public static bool IncludeSession { get; set; }

        private static readonly PropertyInfo RequestContextProperty = GetRequestContextProperty();

        public static HttpContextInformation Create(HttpContextBase httpContext, RouteValueDictionary routeValues = null)
        {
            if (httpContext == null)
                throw new ArgumentNullException("httpContext");

            var httpContextInformation = new HttpContextInformation();

            httpContextInformation.Url = httpContext.Request.Url != null ? httpContext.Request.Url.ToString() : string.Empty;
            httpContextInformation.User = httpContext.User != null && httpContext.User.Identity.IsAuthenticated ? httpContext.User.Identity.Name : string.Empty;
            httpContextInformation.UserAgent = httpContext.Request.ServerVariables["HTTP_USER_AGENT"];
            httpContextInformation.Host = HostingEnvironment.IsHosted ? HostingEnvironment.SiteName : EnvironmentHelpers.TryGetMachineName(httpContext);
            httpContextInformation.Referer = httpContext.Request.ServerVariables["HTTP_REFERER"];
            httpContextInformation.Method = httpContext.Request.ServerVariables["REQUEST_METHOD"];

            httpContextInformation.IpAddress = httpContext.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(httpContextInformation.IpAddress))
                httpContextInformation.IpAddress = httpContext.Request.ServerVariables["REMOTE_ADDR"];

            httpContextInformation.Headers = KeyValueCollection.Create(httpContext.Request.Headers);
            httpContextInformation.Cookies = GetCookies(httpContext.Request.Cookies);
            httpContextInformation.ServerVariables = KeyValueCollection.Create(httpContext.Request.ServerVariables);
            httpContextInformation.QueryString = KeyValueCollection.Create(httpContext.Request.QueryString, !IncludePasswords);
            httpContextInformation.PostedValues = KeyValueCollection.Create(httpContext.Request.Form, !IncludePasswords);
            httpContextInformation.PostedFiles = GetFiles(httpContext.Request.Files);
            httpContextInformation.Session = GetSession(httpContext.Session);

            if (routeValues == null)
                routeValues = GetRouteValues(httpContext);

            if (routeValues != null)
            httpContextInformation.RouteValues = GetRouteValues(routeValues);

            return httpContextInformation;
        }

        public virtual string Url { get; set; }
        public virtual string UserAgent { get; set; }
        public virtual string User { get; set; }
        public virtual string Host { get; set; }
        public virtual string Referer { get; set; }
        public virtual string IpAddress { get; set; }
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
                if (key == null)
                    continue;

                var cookie = cookies[key];
                if (cookie == null)
                    continue;;

                result.Add(cookie.Name, cookie.Value);
            }

            return result.Any() ? result : null;
        }

        private static List<HttpFileInformation> GetFiles(HttpFileCollectionBase files)
        {
            var result = new List<HttpFileInformation>();
            for (var i = 0; i < files.Count; i++)
            {
                var file = files[i];
                if (file == null)
                    continue;

                result.Add(HttpFileInformation.Create(file.FileName, file.ContentLength, file.ContentType));
            }
                
            return result.Any() ? result : null;
        }

        private static KeyValueCollection GetSession(HttpSessionStateBase session)
        {
            if (!IncludeSession)
                return null;

            var collection = new KeyValueCollection();
            foreach (var key in session.Keys)
                collection.Add(key.ToString(), SimpleJson.SerializeObject(session[key.ToString()]));

            return collection.Any() ? collection : null;
        }

        private static RouteValueDictionary GetRouteValues(HttpContextBase httpContext)
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
                collection.Add(item.Key, SimpleJson.SerializeObject(item.Value));

            return collection.Any() ? collection : null;
        }

        private static PropertyInfo GetRequestContextProperty()
        {
            try
            {
                return typeof(HttpRequestBase).GetProperty("RequestContext", BindingFlags.Instance | BindingFlags.Public);
            }
            catch
            {
                return null;
            }
        }
    }

    public class KeyValueCollection : List<KeyValuePair<string, string>>
    {
        public static KeyValueCollection Create(NameValueCollection collection, bool obfuscatePasswords = false)
        {
            if (collection == null)
                return null;

            var result = new KeyValueCollection();
            
            foreach (var key in collection.AllKeys)
            {
                if (key == null)
                    continue;

                string value;
                try
                {
                    value = collection[key];
                }
                catch (HttpRequestValidationException)
                {
                    // TODO the value cannot be retrieved because of the HttpRequestValidationException, is there a better way to do this?
                    value = "[HttpRequestValidationException]";
                }
                
                if (obfuscatePasswords && key.IndexOf("password", StringComparison.InvariantCultureIgnoreCase) >= 0)
                    value = new string('*', (value ?? string.Empty).Length);

                result.Add(key, value);
            }

            return result.Any() ? result : null;
        }

        public void Add(string key, string value)
        {
            Add(new KeyValuePair<string, string>(key, value));
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
