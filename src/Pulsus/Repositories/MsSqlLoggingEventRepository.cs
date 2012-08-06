using System;
using System.Data;
using System.Data.Common;
using Pulsus.Configuration;

namespace Pulsus.Repositories
{
	public class MsSqlLoggingEventRepository : ILoggingEventRepository
	{
		private static object _sync = new object();
		private static bool _initialized;
		private readonly IMsSqlSettings _settings;

		public MsSqlLoggingEventRepository() : this(ConfigurationManager.Settings.MsSql)
		{
		}

		public MsSqlLoggingEventRepository(IMsSqlSettings settings)
		{
			_settings = settings;
			ConnectionName = settings.ConnectionName;
			DatabaseName = settings.DatabaseName;
			Schema = settings.Schema;
			TableName = settings.TableName;
		}

		public string ConnectionName { get; set; }
		public string DatabaseName { get; set; }
		public string Schema { get; set; }
		public string TableName { get; set; }

		public void Save(LoggingEvent loggingEvent)
		{
			if (loggingEvent == null)
				throw new ArgumentNullException("loggingEvent");

			using (var connection = GetConnection())
			{
				Save(connection, loggingEvent);
			}			
		}

		public void Initialize()
		{
			lock (_sync)
			{
				if (_initialized)
					return;

				using (var connection = GetConnection())
				{
					EnsureRepository(connection);
				}

				_initialized = true;
			}
		}

		protected void Save(IDbConnection connection, LoggingEvent loggingEvent)
		{
			var sql = @"insert into [{0}].[{1}] ([EventId], [Timestamp], [Level], [Value], [Text], [Source], [User], [Tags], [Data])
						values (@EventId, @Timestamp, @Level, @Value, @Text, @Source, @User, @Tags, @Data)";

			sql = string.Format(sql, Schema, TableName);
			connection.Execute(sql, MsSqlLoggingEvent.Create(loggingEvent));
		}

		protected void EnsureRepository(IDbConnection connection)
		{
			var sql = @"if (not exists (select 1 from information_schema.tables where table_schema = '{0}' and table_name = '{1}')) begin
							create table [{0}].[{1}] (
								[Id] [int] IDENTITY(1,1) not null,
								[EventId] [uniqueidentifier] not null,
								[Timestamp] [datetime] not null,
								[Level] [int] not null, 
								[Value] [decimal](15,4) null,
								[Text] [varchar](max) not null,
								[Source] [varchar](max) not null,
								[User] [varchar](max) null,
								[Tags] [varchar](max) null,
								[Data] [varchar](max) null,
								CONSTRAINT [PK_{1}] PRIMARY KEY CLUSTERED ([Id] ASC)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
							) ON [PRIMARY]		
						end";

			sql = string.Format(sql, Schema, TableName);
			connection.Execute(sql, new { });
		}

		protected IDbConnection GetConnection()
		{
			if (string.IsNullOrEmpty(ConnectionName))
			{
				if (System.Configuration.ConfigurationManager.ConnectionStrings.Count == 0)
					throw new Exception("There is no connection string in the configuration file");

				ConnectionName = System.Configuration.ConfigurationManager.ConnectionStrings[0].Name;
			}

			var connectionStringItem = System.Configuration.ConfigurationManager.ConnectionStrings[ConnectionName];
			var connectionString = connectionStringItem.ConnectionString;
			var provider = connectionStringItem.ProviderName;

			var providerFactory = DbProviderFactories.GetFactory(provider);
			var connection = providerFactory.CreateConnection();
			connection.ConnectionString = connectionString;

			if (!string.IsNullOrEmpty(DatabaseName))
				connection.ChangeDatabase(DatabaseName);

			connection.Open();

			return connection;
		}
	}
}
