using System;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.Common;
using Pulsus.Internal;

namespace Pulsus.Targets
{
	public class DatabaseTarget : Target
	{
        private bool _initialized;
		private IDbConnection _connection;

		public string ConnectionName { get; set; }
		public string DatabaseName { get; set; }

		[DefaultValue("dbo")]
		public string Schema { get; set; }

		[DefaultValue("LoggingEvents")]
		public string Table { get; set; }

		public bool KeepConnection { get; set; }
		
		public override void Push(LoggingEvent[] loggingEvents)
		{
			if (loggingEvents == null)
				throw new ArgumentNullException("loggingEvents");

			if (!Enabled)
				return;

			var connection = GetConnection();

            if (!_initialized)
            {
				EnsureRepository(connection);
                _initialized = true;
            }

			Save(connection, loggingEvents);
		}

		protected IDbConnection GetConnection()
		{
			if (_connection != null && _connection.State == ConnectionState.Open)
				return _connection;

			ConnectionStringSettings connectionStringSettings;

			if (string.IsNullOrEmpty(ConnectionName))
			{
				if (ConfigurationManager.ConnectionStrings.Count == 0)
					throw new Exception("There is no database connection to push to");

				connectionStringSettings = ConfigurationManager.ConnectionStrings[0];
			}
			else
			{
				connectionStringSettings = ConfigurationManager.ConnectionStrings[ConnectionName];

				if (connectionStringSettings == null)
					throw new Exception(string.Format("Could not find connection name {0}", ConnectionName));
			}

			var connectionString = connectionStringSettings.ConnectionString;
			var provider = connectionStringSettings.ProviderName;

			var providerFactory = DbProviderFactories.GetFactory(provider);
			var connection = providerFactory.CreateConnection();
			if (connection == null)
				throw new Exception("Cannot create database connection");

			connection.ConnectionString = connectionString;

			if (!string.IsNullOrEmpty(DatabaseName))
				connection.ChangeDatabase(DatabaseName);

			connection.Open();

			if (KeepConnection)
				_connection = connection;

			return connection;
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

			sql = string.Format(sql, Schema, Table);
			connection.Execute(sql, new { });
		}

		protected void Save(IDbConnection connection, LoggingEvent[] loggingEvent)
		{
			var sql = @"insert into [{0}].[{1}] ([EventId], [LogKey], [Date], [Level], [Value], [Text], [Tags], [Data], [MachineName], [Host], [Url], [HttpMethod], [IpAddress], [User], [Source], [StatusCode], [Hash], [Count])
						values (@EventId, @LogKey, @Date, @Level, @Value, @Text, @Tags, @Data, @MachineName, @Host, @Url, @HttpMethod, @IpAddress, @User, @Source, @StatusCode, @Hash, @Count)";

			var serialized = Array.ConvertAll(loggingEvent, DatabaseLoggingEvent.Serialize);

			sql = string.Format(sql, Schema, Table);
			connection.Execute(sql, serialized);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && _connection != null)
			{
				_connection.Dispose();
				_connection = null;
			}

			base.Dispose(disposing);
		}
	}
}
