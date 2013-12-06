using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;

namespace Pulsus.SharePoint.Core
{
    internal class RazorTemplateBase<T>
    {
        private string _content;
        public RazorTemplateBase<T> Layout { get; set; }
        public T Model { get; set; }

        private readonly StringBuilder _generatingEnvironment = new StringBuilder();

        public virtual void Execute()
        {
        }

        public void WriteLiteral(string textToAppend)
        {
            if (textToAppend.IsNullOrEmpty())
                return;
            
            _generatingEnvironment.Append(textToAppend); ;
        }

        public void Write(object value)
        {
            if (value == null)
                return;

            WriteLiteral(Convert.ToString(value, CultureInfo.InvariantCulture));
        }

        public string WriteTable(KeyValueCollection keyValueCollection)
        {
            return WriteTable(keyValueCollection.Select(x => new KeyValuePair<string, object>(x.Key, x.Value)));
        }

        public string WriteTable(IEnumerable<KeyValuePair<string, object>> items)
        {
            var hideKeys = (LogManager.Configuration.HideKeys ?? string.Empty).Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            var sb = new StringBuilder();
            sb.Append("<table cellpadding=\"0\" cellspacing=\"0\">");

            var i = 0;
            foreach (var item in items)
            {
                // bypass keys to be hidden
                if (hideKeys.Contains(item.Key ?? string.Empty))
                    continue;

                var valueString = item.Value == null ? "&nbsp;" : HttpUtility.HtmlEncode(item.Value.ToString());

                var @class = i % 2 > 0 ? "class=\"alt\"" : string.Empty;
                sb.Append("<tr " + @class + "><td class=\"first-col\">" + item.Key + "</td><td>" + valueString + "</td></tr>");
                i++;
            }

            sb.Append("</table>");
            return sb.ToString();
        }

        public string RenderBody()
        {
            return _content;
        }

        public string TransformText()
        {
            Execute();
            if (Layout != null)
            {
                Layout._content = _generatingEnvironment.ToString();
                return Layout.TransformText();
            }
            
            return _generatingEnvironment.ToString();
        }
    }
}
