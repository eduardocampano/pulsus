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
        private const string DefaultProviderName = "System.Data.SqlClient";

        protected static readonly object InitializedLock = new object();
        protected bool Initialized;
        protected IDbConnection Connection;

        public DatabaseTarget()
        {
            Schema = "dbo";
            Table = "LoggingEvents";
        }

        public string ConnectionName { get; set; }

        public string DatabaseName { get; set; }

        public string ProviderName { get; set; }
        public string ConnectionString { get; set; }

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

            Exception exception = null;

            try
            {
                EnsureConnection();
                Save(loggingEvents);
            }
            catch (Exception ex)
            {
                exception = ex;
                throw;
            }
            finally
            {
                if (!KeepConnection || exception != null)
                    CloseConnection();
            }
        }

        protected virtual void EnsureConnection()
        {
            if (Connection != null && Connection.State == ConnectionState.Open)
                return;

            var connection = GetConnection();
            if (connection == null)
                return;

            Connection = connection;

            if (!Initialized)
            {
                lock (InitializedLock)
                {
                    if (!Initialized)
                    {
                        EnsureRepository();
                        Initialized = true;
                    }
                }
            }
        }

        protected virtual void CloseConnection()
        {
            if (Connection != null)
            {
                Connection.Close();
                Connection = null;
                PulsusDebugger.Write(this, "Closed connection");
            }
        }

        public virtual IDbConnection GetConnection()
        {
            var connectionSettings = GetConnectionSettings();

            if (connectionSettings == null)
            {
                PulsusDebugger.Error(this, "Unable to resolve connection settings. Please check configuration.");
                return null;
            }

            var providerFactory = DbProviderFactories.GetFactory(connectionSettings.ProviderName);

            var connection = providerFactory.CreateConnection();
            if (connection == null)
            {
                PulsusDebugger.Error(this, "Unable to create connection for provider '{0}'.", connectionSettings.ProviderName);
                return null;
            }

            connection.ConnectionString = connectionSettings.ConnectionString;

            if (!string.IsNullOrEmpty(DatabaseName))
            {
                connection.ChangeDatabase(DatabaseName);
                PulsusDebugger.Write(this, "Changed database to '{0}'", DatabaseName);
            }

            connection.Open();
            PulsusDebugger.Write(this, "Opened connection to database");

            return connection;
        }

        protected virtual ConnectionSettings GetConnectionSettings()
        {
            if (!string.IsNullOrEmpty(ConnectionString))
            {
                PulsusDebugger.Write("Using ConnectionString provided by configuration");
                return new ConnectionSettings(ProviderName, ConnectionString);
            }

            if (!string.IsNullOrEmpty(ConnectionName))
            {
                var connectionStringSettings = ConfigurationManager.ConnectionStrings[ConnectionName];

                if (connectionStringSettings == null)
                {
                    PulsusDebugger.Error("Unable to find ConnectionName '{0}'", ConnectionName);
                    return null;
                }

                PulsusDebugger.Write("Using ConnectionName '{0}' provided by configuration", ConnectionName);
                return new ConnectionSettings(connectionStringSettings.ProviderName, connectionStringSettings.ConnectionString); 
            }

            if (ConfigurationManager.ConnectionStrings.Count > 0)
            {
                var connectionStringSettings = ConfigurationManager.ConnectionStrings[0];
                PulsusDebugger.Write("Using first connection in config file ConnectionName '{0}'", connectionStringSettings.Name);
                return new ConnectionSettings(connectionStringSettings.ProviderName, connectionStringSettings.ConnectionString); 
            }

            PulsusDebugger.Error("Unable to resolve database connection string. Please check configuration");
            return null;
        }

        protected virtual void EnsureRepository()
        {
            if (Connection == null || Connection.State != ConnectionState.Open)
            {
                PulsusDebugger.Error(this, "Cannot ensure repository, connection to database is not open");
                return;
            }

            var sql = IsMySqlConnection(Connection) ? GetMySqlEnsureRepository() : GetMsSqlEnsureRepository();
            Connection.Execute(sql, new { });
            PulsusDebugger.Write(this, "Repository initialized");
        }

        protected virtual void Save(LoggingEvent[] loggingEvents)
        {
            if (Connection == null || Connection.State != ConnectionState.Open)
            {
                PulsusDebugger.Error(this, "Cannot save events, connection to database is not open");
                return;
            }

            var sql = IsMySqlConnection(Connection) ? GetMySqlInsert() : GetMsSqlInsert();
            var serialized = Array.ConvertAll(loggingEvents, DatabaseLoggingEvent.Serialize);
            Connection.Execute(sql, serialized);
            PulsusDebugger.Write(this, "Saved {0} events", loggingEvents.Length);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && Connection != null)
            {
                Connection.Dispose();
                Connection = null;
                PulsusDebugger.Write(this, "Closed connection to database");
            }

            base.Dispose(disposing);
        }

        protected virtual string GetMsSqlEnsureRepository()
        {
            const string sql = @"if (not exists (select 1 from information_schema.tables where table_schema = '{0}' and table_name = '{1}')) begin
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
                                [CorrelationId] [varchar](50) null,
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
            const string sql = @"create table IF NOT EXISTS {0} (
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
                            CorrelationId varchar(50) null,
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
            const string sql = @"if not exists (select 1 from [{0}].[{1}] where [EventId] = @EventId) begin
                            insert into [{0}].[{1}] ([EventId], [LogKey], [ApiKey], [Date], [Level], [Value], [Text], [Tags], [Data], [MachineName], [CorrelationId], [Psid], [Ppid], [Host], [Url], [HttpMethod], [IpAddress], [User], [Source], [StatusCode], [Hash], [Count])
                            values (@EventId, @LogKey, @ApiKey, @Date, @Level, @Value, @Text, @Tags, @Data, @MachineName, @CorrelationId, @Psid, @Ppid, @Host, @Url, @HttpMethod, @IpAddress, @User, @Source, @StatusCode, @Hash, @Count)
                        end";

            return string.Format(sql, Schema, Table);
        }

        protected virtual string GetMySqlInsert()
        {
            const string sql = @"insert ignore into {0} (EventId, LogKey, ApiKey, Date, Level, Value, Text, Tags, Data, MachineName, CorrelationId, Psid, Ppid, Host, Url, HttpMethod, IpAddress, User, Source, StatusCode, Hash, Count)
                            values (@EventId, @LogKey, @ApiKey, @Date, @Level, @Value, @Text, @Tags, @Data, @MachineName, @CorrelationId, @Psid, @Ppid, @Host, @Url, @HttpMethod, @IpAddress, @User, @Source, @StatusCode, @Hash, @Count)";

            return string.Format(sql, Table);
        }

        protected virtual bool IsMySqlConnection(IDbConnection connection)
        {
            return connection.GetType().FullName.IndexOf("mysql", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        protected class ConnectionSettings
        {
            public ConnectionSettings(string connectionString) : this(DefaultProviderName, connectionString)
            {
            }

            public ConnectionSettings(string providerName, string connectionString)
            {
                if (string.IsNullOrEmpty(connectionString))
                    throw new ArgumentNullException("connectionString");

                ProviderName = string.IsNullOrEmpty(providerName) ? DefaultProviderName : providerName;
                ConnectionString = connectionString;
            }

            public string ProviderName { get; private set; }
            public string ConnectionString { get; private set; }

            
        }

        
    }
}
