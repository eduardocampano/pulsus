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
		private enum DbProviderType
		{
			MsSql,
			MySql
		}

		private readonly object _initializedLock = new object();
        private bool _initialized;
		private IDbConnection _connection;
		private DbProviderType _dbProviderType;

		public DatabaseTarget()
		{
			Schema = "dbo";
			Table = "LoggingEvents";
		}

		public string ConnectionName { get; set; }
		public string DatabaseName { get; set; }

		[DefaultValue("dbo")]
		public string Schema { get; set; }

		[DefaultValue("LoggingEvents")]
		public string Table { get; set; }

        [DefaultValue(true)]
		public bool KeepConnection { get; set; }
		
		public override void Push(LoggingEvent[] loggingEvents)
		{
			if (loggingEvents == null)
				throw new ArgumentNullException("loggingEvents");

		    try
		    {
                EnsureConnection();
                Save(loggingEvents);
		    }
		    catch (Exception)
		    {
                CloseConnection();
		        throw;
		    }
            finally
		    {
		        if (!KeepConnection)
                    CloseConnection();
            }
		}

		protected void EnsureConnection()
		{
            if (_connection != null && _connection.State == ConnectionState.Open)
                return;

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
			_dbProviderType = provider.IndexOf("mysql", StringComparison.InvariantCultureIgnoreCase) >= 0 ? DbProviderType.MySql : DbProviderType.MsSql;

			var connection = providerFactory.CreateConnection();
			if (connection == null)
				throw new Exception("Cannot create database connection");

			connection.ConnectionString = connectionString;

			if (!string.IsNullOrEmpty(DatabaseName))
				connection.ChangeDatabase(DatabaseName);

			connection.Open();

            if (!_initialized)
            {
                lock (_initializedLock)
                {
                    if (!_initialized)
                    {
                        EnsureRepository(connection);
                        _initialized = true;
                    }
                }
            }

            _connection = connection;
		}

        protected void CloseConnection()
        {
            if (_connection != null)
            {
                _connection.Close();
                _connection = null;
            }
        }

	    protected void EnsureRepository(IDbConnection connection)
		{
			var sql = _dbProviderType == DbProviderType.MySql ? GetMySqlEnsureRepository() : GetMsSqlEnsureRepository();
			connection.Execute(sql, new { });
		}

		protected void Save(LoggingEvent[] loggingEvent)
		{
			var sql = _dbProviderType == DbProviderType.MySql ? GetMySqlInsert() : GetMsSqlInsert();
			var serialized = Array.ConvertAll(loggingEvent, DatabaseLoggingEvent.Serialize);
			_connection.Execute(sql, serialized);
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

		protected virtual string GetMsSqlEnsureRepository()
		{
			var sql = @"if (not exists (select 1 from information_schema.tables where table_schema = '{0}' and table_name = '{1}')) begin
							create table [{0}].[{1}] (
								[Id] [bigint] not null identity,
								[EventId] [varchar](38) not null,
								[LogKey] [varchar](100) not null,
								[ApiKey] [varchar](100) null,
								[Date] [datetime] not null,
								[Level] [int] null, 
								[Value] [decimal](15,4) null,
								[Text] [varchar](5000) null,
								
								[Tags] [varchar](1000) null,
								[Data] [text] null,
								[User] [varchar](500) null,
								[Psid] [varchar](50) null,
								[Ppid] [varchar](50) null,
								[MachineName] [nvarchar](100) null,
								
								[Host] [varchar](255) null,
								[Url] [varchar](2000) null,
								[HttpMethod] [varchar](10) null,
								[IpAddress] [varchar](40) null,
								
								[Source] [varchar](500) null,
								[StatusCode] [int] null,

								[Hash] [int] null,
								[Count] [int] not null default(1),

								CONSTRAINT [PK_{1}] PRIMARY KEY CLUSTERED ([Id] ASC)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
							) ON [PRIMARY]	

							CREATE UNIQUE INDEX UX_{1}_EventId ON [{0}].[{1}](EventId)	
						end";

			return string.Format(sql, Schema, Table);
		}

		protected virtual string GetMySqlEnsureRepository()
		{
			var sql = @"create table IF NOT EXISTS {0} (
							Id bigint not null AUTO_INCREMENT,
							EventId varchar(38) not null,
							LogKey varchar(100) not null,
							ApiKey varchar(100) null,
							Date datetime not null,
							Level int null, 
							Value decimal(15,4) null,
							Text varchar(5000) null,
		
							Tags varchar(1000) null,
							Data text null,
							User varchar(500) null,
							Psid varchar(50) null,
							Ppid varchar(50) null,
							MachineName nvarchar(100) null,
		
							Host varchar(255) null,
							Url varchar(2000) null,
							HttpMethod varchar(10) null,
							IpAddress varchar(40) null,
		
							Source varchar(500) null,
							StatusCode int null,

							Hash int null,
							Count int not null default 1,

							CONSTRAINT PK_{0} PRIMARY KEY CLUSTERED (Id ASC),
							UNIQUE INDEX UX_{0}_EventId (EventId)
						)";

			return string.Format(sql, Table);
		}

		protected virtual string GetMsSqlInsert()
		{
			var sql = @"if not exists (select 1 from [{0}].[{1}] where [EventId] = @EventId) begin
							insert into [{0}].[{1}] ([EventId], [LogKey], [ApiKey], [Date], [Level], [Value], [Text], [Tags], [Data], [MachineName], [Psid], [Ppid], [Host], [Url], [HttpMethod], [IpAddress], [User], [Source], [StatusCode], [Hash], [Count])
							values (@EventId, @LogKey, @ApiKey, @Date, @Level, @Value, @Text, @Tags, @Data, @MachineName, @Psid, @Ppid, @Host, @Url, @HttpMethod, @IpAddress, @User, @Source, @StatusCode, @Hash, @Count)
						end";

			return string.Format(sql, Schema, Table);
		}

		protected virtual string GetMySqlInsert()
		{
			var sql = @"insert ignore into {0} (EventId, LogKey, ApiKey, Date, Level, Value, Text, Tags, Data, MachineName, Psid, Ppid, Host, Url, HttpMethod, IpAddress, User, Source, StatusCode, Hash, Count)
							values (@EventId, @LogKey, @ApiKey, @Date, @Level, @Value, @Text, @Tags, @Data, @MachineName, @Psid, @Ppid, @Host, @Url, @HttpMethod, @IpAddress, @User, @Source, @StatusCode, @Hash, @Count)";

			return string.Format(sql, Table);
		}
	}
}
