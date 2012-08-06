using System.Configuration;

namespace Pulsus.Configuration
{
	public interface IPulsusSettings
	{
		string Source { get; }
		string Tags { get; }
		bool Async { get; }
		IRemoteSettings Remote { get; }
		IMsSqlSettings MsSql { get; }
		IEmailSettings Email { get; }
	}

	internal class PulsusSectionWrapper : IPulsusSettings
	{
		private readonly PulsusSection _section;

		public PulsusSectionWrapper(PulsusSection section)
		{
			_section = section;
		}

		public bool Async
		{
			get
			{
				return _section.Async;
			}
		}

		public string Tags
		{
			get
			{
				return _section.Tags;
			}
		}

		public string Source
		{
			get
			{
				return _section.Source;
			}
		}

		public virtual IRemoteSettings Remote 
		{
			get
			{
				return _section.Remote;
			}
		}

		public virtual IMsSqlSettings MsSql
		{
			get
			{
				return _section.MsSql;
			}
		}

		public virtual IEmailSettings Email
		{
			get
			{
				return _section.Email;
			}
		}
	}

	internal class PulsusSection : ConfigurationSection
	{
		[ConfigurationProperty("remote", IsRequired = false)]
		public virtual RemoteElement Remote
		{
			get
			{
				return (RemoteElement)base["remote"];
			}
			set
			{
				base["remote"] = value;
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

		[ConfigurationProperty("async", DefaultValue = true, IsRequired = false)]
		public bool Async
		{
			get
			{
				return (bool)this["async"];
			}
			set
			{
				this["async"] = value;
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

		[ConfigurationProperty("source", DefaultValue = "", IsRequired = false)]
		public string Source
		{
			get
			{
				return (string)this["source"];
			}
			set
			{
				this["source"] = value;
			}
		}
	}
}
