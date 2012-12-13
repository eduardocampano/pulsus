using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using Pulsus.Configuration;
using Pulsus.Internal;

namespace Pulsus.Repositories
{
	public class MsSqlLoggingEventRepository : ILoggingEventRepository
	{
		private static readonly object _sync = new object();
		private static bool _initialized;
		private readonly IMsSqlSettings _settings;

		public MsSqlLoggingEventRepository(IMsSqlSettings settings)
		{
			_settings = settings;
			
			ConnectionName = settings.ConnectionName;
			DatabaseName = settings.DatabaseName;
			Schema = settings.Schema;
			TableName = settings.TableName;

			if (string.IsNullOrEmpty(ConnectionName))
			{
				if (System.Configuration.ConfigurationManager.ConnectionStrings.Count == 0)
					throw new Exception("There is no connection string in the configuration file");

				ConnectionName = System.Configuration.ConfigurationManager.ConnectionStrings[0].Name;
			}
		}

		public string ConnectionName { get; set; }
		public string DatabaseName { get; set; }
		public string Schema { get; set; }
		public string TableName { get; set; }

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

		public void Save(LoggingEvent[] loggingEvents)
		{
			if (loggingEvents == null)
				throw new ArgumentNullException("loggingEvents");

			using (var connection = GetConnection())
			{
				Save(connection, loggingEvents);
			}
		}

	    public IEnumerable<LoggingEvent> Retrieve()
	    {
	        throw new NotImplementedException();
	    }

	    public IEnumerable<LoggingEvent> Retrieve(int take)
		{
			using (var connection = GetConnection())
			{ 
				var sql = string.Format("select top {2} * from [{0}].[{1}]", Schema, TableName, take);
				return connection.Query<MsSqlLoggingEvent>(sql, null).Select(MsSqlLoggingEvent.Deserialize);
			}
		}

		protected void Save(IDbConnection connection, LoggingEvent[] loggingEvent)
		{
			var sql = @"insert into [{0}].[{1}] ([EventId], [LogKey], [Date], [Level], [Value], [Text], [Tags], [Data], [MachineName], [Host], [Url], [HttpMethod], [IpAddress], [User], [Source], [StatusCode], [Hash], [Count])
						values (@EventId, @LogKey, @Date, @Level, @Value, @Text, @Tags, @Data, @MachineName, @Host, @Url, @HttpMethod, @IpAddress, @User, @Source, @StatusCode, @Hash, @Count)";

			var serialized = Array.ConvertAll(loggingEvent, MsSqlLoggingEvent.Serialize);

			sql = string.Format(sql, Schema, TableName);
			connection.Execute(sql, serialized);
		}

		protected void EnsureRepository(IDbConnection connection)
		{
			var sql = @"if (not exists (select 1 from information_schema.tables where table_schema = '{0}' and table_name = '{1}')) begin
							create table [{0}].[{1}] (
								[Id] [bigint] not null identity,
								[EventId] [uniqueidentifier] not null,
								[LogKey] [varchar](100) not null,
								[Date] [datetime] not null,
								[Level] [int] not null, 
								[Value] [decimal](15,4) null,
								[Text] [varchar](max) not null,
								
								[Tags] [nvarchar](max) null,
								[Data] [nvarchar](max) null,
								[User] [nvarchar](max) null,
								[MachineName] [nvarchar](50) NOT NULL,
								
								[Host] [nvarchar](100) null,
								[Url] [nvarchar](500) null,
								[HttpMethod] [nvarchar](10) null,
								[IpAddress] [varchar](40) null,
								
								[Source] [nvarchar](max) null,
								[StatusCode] [int] null,

								[Hash] [int] null,
								[Count] [int] not null default(1),

								CONSTRAINT [PK_{1}] PRIMARY KEY CLUSTERED ([Id] ASC)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
							) ON [PRIMARY]		
						end";

			sql = string.Format(sql, Schema, TableName);
			connection.Execute(sql, new { });
		}

		protected IDbConnection GetConnection()
		{
			var connectionStringItem = System.Configuration.ConfigurationManager.ConnectionStrings[ConnectionName];
			var connectionString = connectionStringItem.ConnectionString;
			var provider = connectionStringItem.ProviderName;

			var providerFactory = DbProviderFactories.GetFactory(provider);
			var connection = providerFactory.CreateConnection();
            if (connection == null)
                throw new Exception("Cannot create database connection");

			connection.ConnectionString = connectionString;

			if (!string.IsNullOrEmpty(DatabaseName))
				connection.ChangeDatabase(DatabaseName);

			connection.Open();

			return connection;
		}
	}
}
