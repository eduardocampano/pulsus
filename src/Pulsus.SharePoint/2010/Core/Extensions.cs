using System;
using System.Net;
using System.Web;

namespace Pulsus.SharePoint.Core
{
    internal static class Extensions
    {
        public static bool IsNullOrEmpty(this string input)
        {
            return string.IsNullOrEmpty(input);
        }

        public static bool IsAjaxRequest(this HttpRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            if (request["X-Requested-With"] == "XMLHttpRequest")
                return true;

            if (request.Headers != null)
                return request.Headers["X-Requested-With"] == "XMLHttpRequest";
            else
                return false;
        }

        public static void JsonResult(this HttpResponse response, object data)
        {
            response.Clear();
            response.AddHeader("Content-Type", "application/json");
            response.Write(LogManager.JsonSerializer.SerializeObject(data));
            response.End();
        }

        public static void HtmlResult(this HttpResponse response, string html)
        {
            response.Clear();
            response.AddHeader("Content-Type", "text/html");
            response.Write(html);
            response.End();
        }
    }
}
