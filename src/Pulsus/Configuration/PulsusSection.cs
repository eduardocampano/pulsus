using System.Configuration;

namespace Pulsus.Configuration
{
	internal class PulsusSection : ConfigurationSection
	{
		[ConfigurationProperty("server", IsRequired = false)]
		public virtual ServerElement Server
		{
			get
			{
				return (ServerElement)base["server"];
			}
			set
			{
				base["server"] = value;
			}
		}

		[ConfigurationProperty("mssql", IsRequired = false)]
		public virtual MsSqlElement MsSql
		{
			get
			{
				return (MsSqlElement)base["mssql"];
			}
			set
			{
				base["mssql"] = value;
			}
		}

		[ConfigurationProperty("email", IsRequired = false)]
		public virtual EmailElement Email
		{
			get
			{
				return (EmailElement)base["email"];
			}
			set
			{
				base["email"] = value;
			}
		}

		[ConfigurationProperty("logKey", DefaultValue = "Default", IsRequired = false)]
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

		[ConfigurationProperty("tags", DefaultValue = "", IsRequired = false)]
		public string Tags
		{
			get
			{
				return (string)this["tags"];
			}
			set
			{
				this["tags"] = value;
			}
		}

		[ConfigurationProperty("debug", DefaultValue = false, IsRequired = false)]
		public bool Debug
		{
			get
			{
				return (bool)this["debug"];
			}
			set
			{
				this["debug"] = value;
			}
		}

		[ConfigurationProperty("debugFile", DefaultValue = "", IsRequired = false)]
		public string DebugFile
		{
			get
			{
				return (string)this["debugFile"];
			}
			set
			{
				this["debugFile"] = value;
			}
		}

        [ConfigurationProperty("includeHttpContext", DefaultValue = false, IsRequired = false)]
        public bool IncludeHttpContext
        {
            get
            {
                return (bool)this["includeHttpContext"];
            }
            set
            {
                this["includeHttpContext"] = value;
            }
        }

        [ConfigurationProperty("includeStackTrace", DefaultValue = false, IsRequired = false)]
        public bool IncludeStackTrace
        {
            get
            {
                return (bool)this["includeStackTrace"];
            }
            set
            {
                this["includeStackTrace"] = value;
            }
        }
	}
}
