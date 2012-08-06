using System.Configuration;

namespace Pulsus.Configuration
{
	public interface IRemoteSettings
	{
		bool Enabled { get; }
		string LogKey { get; }
		string Url { get; }
		bool Compress { get; }
	}

	internal class RemoteElement : ConfigurationElement, IRemoteSettings
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

		[ConfigurationProperty("logKey", DefaultValue = "", IsRequired = true)]
		public string LogKey
		{
			get
			{
				return (string)this["logKey"];
			}
			set
			{
				this["logKey"] = value;
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
