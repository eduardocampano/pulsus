using System.Configuration;

namespace Pulsus.Configuration
{
	public interface IEmailSettings
	{
		bool Enabled { get; }
		string To { get; }
		string From { get; }
	}

	internal class EmailElement : ConfigurationElement, IEmailSettings
	{
		[ConfigurationProperty("enabled", DefaultValue = false, IsRequired = true)]
		public bool Enabled
		{
			get
			{
				return (bool)this["enabled"];
			}
			set
			{
				this["enabled"] = value;
			}
		}

		[ConfigurationProperty("to", DefaultValue = "", IsRequired = true)]
		public string To
		{
			get
			{
				return (string)this["to"];
			}
			set
			{
				this["to"] = value;
			}
		}

		[ConfigurationProperty("from", DefaultValue = "", IsRequired = false)]
		public string From
		{
			get
			{
				return (string)this["from"];
			}
			set
			{
				this["from"] = value;
			}
		}
	}
}
