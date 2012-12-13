using System.Configuration;

namespace Pulsus.Configuration
{
	internal class EmailElement : ConfigurationElement
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

		[ConfigurationProperty("link", DefaultValue = "", IsRequired = false)]
		public string Link
		{
			get
			{
				return (string)this["link"];
			}
			set
			{
				this["link"] = value;
			}
		}

		[ConfigurationProperty("smtpServer", DefaultValue = "", IsRequired = false)]
		public string SmtpServer
		{
			get
			{
				return (string)this["smtpServer"];
			}
			set
			{
				this["smtpServer"] = value;
			}
		}

		[ConfigurationProperty("smtpPort", DefaultValue = 0, IsRequired = false)]
		public int SmtpPort
		{
			get
			{
				return (int)this["smtpPort"];
			}
			set
			{
				this["smtpPort"] = value;
			}
		}

		[ConfigurationProperty("smtpUsername", DefaultValue = "", IsRequired = false)]
		public string SmtpUsername
		{
			get
			{
				return (string)this["smtpUsername"];
			}
			set
			{
				this["smtpUsername"] = value;
			}
		}

		[ConfigurationProperty("smtpPassword", DefaultValue = "", IsRequired = false)]
		public string SmtpPassword
		{
			get
			{
				return (string)this["smtpPassword"];
			}
			set
			{
				this["smtpPassword"] = value;
			}
		}

		[ConfigurationProperty("smtpEnableSsl", DefaultValue = false, IsRequired = false)]
		public bool SmtpEnableSsl
		{
			get
			{
				return (bool)this["smtpEnableSsl"];
			}
			set
			{
				this["smtpEnableSsl"] = value;
			}
		}
	}
}
