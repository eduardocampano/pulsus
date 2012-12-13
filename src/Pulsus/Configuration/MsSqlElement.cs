using System.Configuration;

namespace Pulsus.Configuration
{
    internal class MsSqlElement : ConfigurationElement, IMsSqlSettings
	{
		[ConfigurationProperty("enabled", DefaultValue = true, IsRequired = true)]
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

		[ConfigurationProperty("connectionName", DefaultValue = "", IsRequired = false)]
		public string ConnectionName
		{
			get
			{
				return (string)this["connectionName"];
			}
			set
			{
				this["connectionName"] = value;
			}
		}

		[ConfigurationProperty("databaseName", DefaultValue = "", IsRequired = false)]
		public string DatabaseName
		{
			get
			{
				return (string)this["databaseName"];
			}
			set
			{
				this["databaseName"] = value;
			}
		}

		[ConfigurationProperty("schema", DefaultValue = "dbo", IsRequired = false)]
		public string Schema
		{
			get
			{
				return (string)this["schema"];
			}
			set
			{
				this["schema"] = value;
			}
		}

		[ConfigurationProperty("tableName", DefaultValue = "PulsusEvents", IsRequired = false)]
		public string TableName
		{
			get
			{
				return (string)this["tableName"];
			}
			set
			{
				this["tableName"] = value;
			}
		}
	}
}
