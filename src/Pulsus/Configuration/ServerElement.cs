using System.Configuration;

namespace Pulsus.Configuration
{
    internal class ServerElement : ConfigurationElement, IServerSettings
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

		[ConfigurationProperty("apiKey", DefaultValue = "", IsRequired = true)]
		public string ApiKey
		{
			get
			{
				return (string)this["apiKey"];
			}
			set
			{
				this["apiKey"] = value;
			}
		}

		[ConfigurationProperty("url", DefaultValue = "", IsRequired = false)]
		public string Url
		{
			get
			{
				return (string)this["url"];
			}
			set
			{
				this["url"] = value;
			}
		}

		[ConfigurationProperty("compress", DefaultValue = true, IsRequired = false)]
		public bool Compress
		{
			get
			{
				return (bool)this["compress"];
			}
			set
			{
				this["compress"] = value;
			}
		}
	}
}
