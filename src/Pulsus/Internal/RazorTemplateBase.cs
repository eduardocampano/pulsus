using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Pulsus.Internal
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
			if (String.IsNullOrEmpty(textToAppend))
			{
				return;
			}
			_generatingEnvironment.Append(textToAppend); ;
		}

		public void Write(object value)
		{
			if ((value == null))
			{
				return;
			}

			WriteLiteral(Convert.ToString(value, CultureInfo.InvariantCulture));
		}

		public string WriteTable(KeyValueCollection keyValueCollection)
		{
			var sb = new StringBuilder();
			sb.Append("<table cellpadding=\"0\" cellspacing=\"0\">");
			sb.Append("<tr><th class=\"first-col\" align=\"left\">Key</th><th align=\"left\">Value</th></tr>");

			var i = 0;
			foreach (var item in keyValueCollection)
			{
				var @class = i % 2 > 0 ? "class=\"alt\"" : string.Empty;
				sb.Append("<tr " + @class + "><td class=\"first-col\">" + item.Key + "</td><td>" + item.Value + "</td></tr>");
				i++;
			}

			sb.Append("</table>");
			return sb.ToString();
		}

		public string WriteTable(IDictionary<string, object> dictionary)
		{
			var sb = new StringBuilder();
			sb.Append("<table cellpadding=\"0\" cellspacing=\"0\">");
			sb.Append("<tr><th class=\"first-col\" align=\"left\">Key</th><th align=\"left\">Value</th></tr>");

			var i = 0;
			foreach (var item in dictionary)
			{
				var @class = i % 2 > 0 ? "class=\"alt\"" : string.Empty; 
				sb.Append("<tr " + @class + "><td class=\"first-col\">" + item.Key + "</td><td>" + item.Value + "</td></tr>");
				i++;
			}

			sb.Append("</table>");
			return sb.ToString();
		}

		public string WriteTable(object instance)
		{
			return WriteTable(ObjectHelpers.ToDictionary(instance));
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
			else
			{
				return _generatingEnvironment.ToString();
			}
		}
	}

}
